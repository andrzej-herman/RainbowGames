namespace RainbowSnake.GameLogic;

public class GameState
{
	private int Rows { get; }
	private int Cols { get; }
	public GridValue[,] Grid { get; }
	public Direction Dir { get; private set; }
	public int Score { get; private set; }
	public bool GameOver { get; private set; }
	
	private readonly LinkedList<Direction> dirChanges = [];
	private readonly LinkedList<Position> snakePositions = [];
	private readonly Random random = new Random();
	

	public GameState(int rows, int cols)
	{
		Rows = rows;
		Cols = cols;
		Grid = new GridValue[rows, cols];
		Dir = Direction.Right;
		AddSnake();
		AddFood();
	}
	
	private void AddSnake()
	{
		var r = Rows / 2;
		for (var c = 1; c <= 3; c++)
		{
			Grid[r, c] = GridValue.Snake;
			snakePositions.AddFirst(new Position(r, c));
		}
	}

	private IEnumerable<Position> EmptyPositions()
	{
		for (var r = 0; r < Rows; r++)
		{
			for (var c = 0; c < Cols; c++)
			{
				if (Grid[r, c] == GridValue.Empty)
					yield return new Position(r, c);
			}
		}
	}

	private void AddFood()
	{
		var empties = new List<Position>(EmptyPositions());
		if (empties.Count == 0)
			return;
		
		var position = empties[random.Next(empties.Count)];
		Grid[position.Row, position.Col] = GridValue.Food;
	}
	
	public Position HeadPosition() => snakePositions?.First?.Value;
	public Position TailPosition() => snakePositions?.Last?.Value;
	public IEnumerable<Position> SnakePosition() => snakePositions;
	
	private void AddHead(Position position)
	{
		snakePositions.AddFirst(position);
		Grid[position.Row, position.Col] = GridValue.Snake;
	}
	
	private void RemoveTail()
	{
		var tail = snakePositions?.Last?.Value;
		Grid[tail!.Row, tail!.Col] = GridValue.Empty;
		snakePositions?.RemoveLast();
	}

	private Direction GetLastDirection()
	{
		return dirChanges.Count == 0 ? Dir : dirChanges.Last?.Value;
	}

	private bool CanChangeDirection(Direction newDirection)
	{
		if (dirChanges.Count == 2)
			return false;

		var lastDirection = GetLastDirection();
		return newDirection != lastDirection && newDirection != lastDirection.Opposite();
	}	
	
	public void ChangeDirection(Direction direction)
	{
		if (CanChangeDirection(direction))
			dirChanges.AddLast(direction);
	}
	
	private bool OutsideGrid(Position position) =>
		position.Row < 0 || position.Row >= Rows || position.Col < 0 || position.Col >= Cols;
	
	private GridValue WillHit(Position position)
	{
		if (OutsideGrid(position))
			return GridValue.Outside;

		// here to eventually change if tail hit is game over
		return position == TailPosition() ? GridValue.Empty : Grid[position.Row, position.Col];
	}

	public void Move()
	{
		if (dirChanges.Count > 0)
		{
			Dir = dirChanges.First?.Value;
			dirChanges.RemoveFirst();
		}
		
		var newHeadPosition = HeadPosition().Translate(Dir);
		var hit = WillHit(newHeadPosition);
		switch (hit)
		{
			case GridValue.Outside:
			case GridValue.Snake:
				GameOver = true;
				break;
			case GridValue.Empty:
				RemoveTail();
				AddHead(newHeadPosition);
				break;
			case GridValue.Food:
				AddHead(newHeadPosition);
				Score++;
				AddFood();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}