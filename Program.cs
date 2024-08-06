
internal class Program
{
    private static void Main(string[] args)
    {
        Thread inputListening = new Thread(ListenKey);
        inputListening.Start();

        do
        {
            SetupGame();
            DrawGameScreen();
            SnakeMove();
            CreateFood();

            Task.Delay(delayTime).Wait();
        }
        while (playing);
    }
    static bool playing = true;
    static string up = "UP";
    static string down = "DOWN";
    static string left = "LEFT";
    static string right = "RIGHT";
    static int delayTime = 500;
    static int minDelayTime = 50;
    static int speed = 1;
    static int _rows = 20;
    static int _cols = 40;
    static string[,] gameScreen = new string[_rows, _cols];
    static int headRow = 1;
    static int headCol = 1;
    static Point snakeHead = new Point(headRow, headCol);
    static bool isFoodExist = false;
    static int foodRow;
    static int foodCol;
    static Point food = new Point(foodRow, foodCol);
    static int score = 0;
    static Point[] bodySnake = new Point[1] { snakeHead };
    
    class Point
    {
        public int row { get; set; }
        public int col { get; set; }
        public string direction { get; set; }

        public Point(int _row, int _col)
        {
            this.row = _row;
            this.col = _col;
        }

        public Point(int _row, int _col, string _direction)
        {
            this.row = _row;
            this.col = _col;
            this.direction = _direction;
        }

        public void ChangeDirection(string _direction)
        {
            this.direction = _direction;
        }
    }

    // Cấu hình game
    static void SetupGame()
    {   
        // Màn hình game
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                if (row == 0 || row == _rows - 1 || col == 0 || col == _cols - 1)
                {
                    gameScreen[row, col] = "#";
                }
                else
                {
                    gameScreen[row, col] = " ";
                }
            }
        }
        // Đầu rắn
        gameScreen[snakeHead.row, snakeHead.col] = "@";


        // Thân rắn
        for (int i = 0; i < bodySnake.Length; i++)
        {   
            gameScreen[bodySnake[i].row, bodySnake[i].col] = "X";
        }



        // Thức ăn
        gameScreen[food.row, food.col] = "o";
    }

    // Màn hình game
    static void DrawGameScreen()
    {
        Console.Clear();
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                Console.Write($"{gameScreen[row, col]} ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"Score: {score}");
    }

    // Di chuyển 
    static void SnakeMove()
    {   
        // Di chuyển thân rắn
        for (int i = bodySnake.Length - 1; i > 0; i--)
        {
            bodySnake[i] = new Point(bodySnake[i - 1].row, bodySnake[i - 1].col, bodySnake[i - 1].direction);
        }

        // Di chuyển đầu rắn
        switch (snakeHead.direction)
        {
            case "up":
                if (snakeHead.row == 1)
                {
                    snakeHead.row = _rows - 2;
                }
                else
                {
                    snakeHead.row--;
                }
                break;

            case "down":
                if (snakeHead.row == _rows - 2)
                {
                    snakeHead.row = 1;
                }
                else
                {
                    snakeHead.row++;
                }
                break;

            case "right":
                if (snakeHead.col == _cols - 2)
                {
                    snakeHead.col = 1;
                }
                else
                {
                    snakeHead.col++;
                }
                break;

            case "left":
                if (snakeHead.col == 1)
                {
                    snakeHead.col = _cols - 2;
                }
                else
                {
                    snakeHead.col--;
                }
                break;
        }
        GameOver();
        EatFood();
    }

    // Kết nối với bàn phím
    static void ListenKey()
    {   
        // Bắt hoạt động của người dùng 
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (snakeHead.direction != down)
                    {
                        snakeHead.ChangeDirection(up);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (snakeHead.direction != up)
                    {
                        snakeHead.ChangeDirection(down);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (snakeHead.direction != left)
                    {
                        snakeHead.ChangeDirection(right);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (snakeHead.direction != right)
                    {
                        snakeHead.ChangeDirection(left);
                    }
                    break;
            }
        }
        while (true);
    }

    // Tạo thức ăn
    static void CreateFood()
    {
        if (!isFoodExist)
        {
            int row, col;
            bool check = false;

            while (!check)
            {
                Random random = new Random();
                row = random.Next(1, _rows - 1);
                col = random.Next(1, _cols - 1);

                // Tránh thức ăn spawn trùng với con rắn
                for (int i = 0; i < bodySnake.Length; i++)
                {
                    if (row != snakeHead.row && col != snakeHead.col)
                    {
                        check = true;
                    }
                }
                food = new Point(row, col);
                isFoodExist = true;
            }

        }
    }

    // Ăn thức ăn
    static void EatFood()
    {
        if (snakeHead.row == food.row && snakeHead.col == food.col)
        {
            isFoodExist = false;
            score++;

            // Tăng tốc độ khi ăn thức ăn
            if (delayTime > minDelayTime)
            {
                delayTime -= 50;
                speed++;
            }

            // Tăng chiều dài con rắn
            Point tempPoint = bodySnake[bodySnake.Length - 1];
            Array.Resize(ref bodySnake, bodySnake.Length + 1);
            bodySnake[bodySnake.Length - 1] = tempPoint;
        }
    }

    // Điều kiện dừng game
    static void GameOver()
    {
        for (int i = 1; i < bodySnake.Length; i++)
        {
            if (snakeHead.row == bodySnake[i].row && snakeHead.col == bodySnake[i].col)
            {
                playing = false;
                Console.WriteLine("GAME OVER!!");
            }
        }
    }
}