using System.Windows;
using System.Windows.Input;
using RainbowSnake.Helpers;


namespace RainbowSnake
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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
    }
}