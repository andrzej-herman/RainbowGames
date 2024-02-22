using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RainbowSnake.GameLogic;
using RainbowSnake.Helpers;

namespace RainbowSnake;

public partial class MainWindow
{
    private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
    {
        { GridValue.Empty, Images.Empty },
        { GridValue.Snake, Images.Body },
        { GridValue.Food, Images.Food },
    };

    private readonly Dictionary<Direction, int> dirToRotation = new()
    {
        { Direction.Up, 0 },
        { Direction.Right, 90 },
        { Direction.Down, 180 },
        { Direction.Left, 270 },
    };
    
    #region WindowManagement

    private void Border_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    private void MinButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        MinButton.Source = Images.MinHover;
        MinButton.Width = 38;
        MinButton.Height = 27;
    }

    private void MinButton_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MinButton_OnMouseLeave(object sender, MouseEventArgs e)
    {
        MinButton.Source = Images.Min;
        MinButton.Width = 38;
        MinButton.Height = 27;
    }

    private void CloseButton_OnMouseEnter(object sender, MouseEventArgs e)
    {
        CloseButton.Source = Images.CloseHover;
        CloseButton.Width = 38;
        CloseButton.Height = 27;
    }

    private void CloseButton_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        this.Close();
    }

    private void CloseButton_OnMouseLeave(object sender, MouseEventArgs e)
    {
        CloseButton.Source = Images.Close;
        CloseButton.Width = 38;
        CloseButton.Height = 27;
    }

    #endregion
    
    
    private int speed;
    private const int rows = 23;
    private const int cols = 23;
    private readonly Image[,] gridImages;
    private GameState gameState;
    private bool gameRunning;
    
    public MainWindow()
    {
        InitializeComponent();
        gridImages = SetUpGrid();
        gameState = new GameState(rows, cols);
        speed = GetSpeed(gameState.Score);
    }

   
    
    private static int GetSpeed(int score)
    {
        return score switch
        {
            < 10 => 225,
            >= 10 and < 20 => 200,
            >= 20 and < 30 => 175,
            >= 30 and < 50 => 150,
            >= 50 and < 70 => 125,
            >= 70 and < 80 => 100,
            >= 80 and < 100 => 75,
            >= 100 and < 120 => 50,
            >= 120 => 25
        };
    }

    private static string GetSpeedValue(int speed)
    {
        return speed switch
        {
            225 => "10 km/h",
            200 => "20 km/h",
            175 => "30 km/h",
            150 => "40 km/h",
            125 => "50 km/h",
            100 => "60 km/h",
            75 => "70 km/h",
            50 => "80 km/h",
            25 => "MAX",
            _ => "MAX"
        };
    }

    private async Task GameLoop()
    {
        while (!gameState.GameOver)
        {
            speed = GetSpeed(gameState.Score);
            await Task.Delay(speed);
            gameState.Move();
            Draw();
        }
    }

    
    
    
    private Image[,] SetUpGrid()
    {
        var images = new Image[rows, cols];
        GameGrid.Rows = rows;
        GameGrid.Columns = cols;
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var image = new Image
                {
                    Source = Images.Empty, 
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };
                images[r, c] = image;
                GameGrid.Children.Add(image);
            }
        }

        return images;
    }

    private async Task RunGame()
    {
        Draw();
        ShowCountDown1();
        await ShowCountDown2();
        Overlay1.Visibility = Visibility.Hidden;
        Overlay2.Visibility = Visibility.Hidden;
        await GameLoop();
        await ShowGameOver();
        gameState = new GameState(rows, cols);
    }
    
    private void Draw()
    {
        DrawGrid();
        DrawSnakeHead();
        DrawSnakeTail();
        ScoreText.Text = $"Wynik: {gameState.Score} pkt.";
        SpeedText.Text = $"Prędkość węża: {GetSpeedValue(speed)}";
    }
    
    private async Task ShowCountDown2()
    {
        Overlay1.Visibility = Visibility.Hidden;
        for (var idx = 3; idx >= 1; idx--)
        {
            Countdown.Text = $"Start za : {idx.ToString()} sek.";
            await Task.Delay(1000);
        }
    }
    
    
    private void ShowCountDown1()
    {
        Overlay1.Visibility = Visibility.Hidden;
        Overlay2.Visibility = Visibility.Visible; 
    }
    
    private async Task ShowGameOver()
    {
        await DrawDeadSnake();
        await Task.Delay(1000);
        Overlay1.Visibility = Visibility.Visible;
    }

    private async void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Overlay1.Visibility == Visibility.Visible)
            e.Handled = true;
        
        if (gameRunning) return;
        gameRunning = true;
        await RunGame();
        gameRunning = false;
    }
    
    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (gameState.GameOver)
            return;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case Key.Left:
            case Key.A:
                gameState.ChangeDirection(Direction.Left);
                break;
            case Key.Right:
            case Key.D:
                gameState.ChangeDirection(Direction.Right);
                break;
            case Key.Up:
            case Key.W:
                gameState.ChangeDirection(Direction.Up);
                break;
            case Key.Down:
            case Key.S:
                gameState.ChangeDirection(Direction.Down);
                break;
            case Key.None:
                break;
            default:
                return;
        }
    }

    
    
    private void DrawGrid()
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var gridVal = gameState.Grid[r, c];
                gridImages[r, c].Source = gridValToImage[gridVal];
                gridImages[r, c].RenderTransform = Transform.Identity;
            }
        }
    }


    private void DrawSnakeHead()
    {
        var headPos = gameState.HeadPosition();
        var img = gridImages[headPos.Row, headPos.Col];
        img.Source = Images.Head;
        var rotation = dirToRotation[gameState.Dir];
        img.RenderTransform = new RotateTransform(rotation);
    }
    
    private void DrawSnakeTail()
    {
        var tailPos = gameState.TailPosition();
        var img = gridImages[tailPos.Row, tailPos.Col];
        img.Source = Images.Tail;
        var rotation = dirToRotation[gameState.Dir];
        img.RenderTransform = new RotateTransform(rotation);
    }

    private async Task DrawDeadSnake()
    {
        var positions = new List<Position>(gameState.SnakePosition());
        for (var idx = 0; idx < positions.Count; idx++)
        {
            var pos = positions[idx];
            var source = idx == 0 ? Images.DeadHead : Images.DeadBody;
            gridImages[pos.Row, pos.Col].Source = source;
            await Task.Delay(5);
        }
    }
}
