using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

class Program
{
    static public List<Point> Wall = new List<Point>();

    static void InitWall()
    {
        for (int i = 0; i <= 20; i++)
        {
            Wall.Add(new Point(i, 0, '#'));
            Wall.Add(new Point(i, 20, '#'));
            Wall.Add(new Point(0, i, '#'));
            Wall.Add(new Point(20, i, '#'));
        }
    }

    static void PrintWall()
    {
        for (int i = 0; i < Wall.Count; i++)
        {
            Wall[i].Draw();
        }
    }

    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        // 벽을 초기화 합니다.
        InitWall();
        PrintWall();

        // 뱀의 초기 위치와 방향을 설정하고, 그립니다.
        Point p = new Point(4, 5, '*');
        Snake snake = new Snake(p, 4, Direction.RIGHT);
        snake.Draw();

        Console.SetCursorPosition(50, 20);
        Console.WriteLine($"현재 길이: {snake.Body.Count} / 먹은 음식의 수: {snake.EatingCount}");

        // 음식의 위치를 무작위로 생성하고, 그립니다.
        FoodCreator foodCreator = new FoodCreator(20, 20, '$');
        Point food = foodCreator.CreateFood(snake);
        food.Draw();

        // 게임 루프: 이 루프는 게임이 끝날 때까지 계속 실행됩니다.
        while (true)
        {
            PrintWall();

            // 키 입력이 있는 경우에만 방향을 변경합니다.
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.W:
                    snake.Dir = Direction.UP;
                    break;
                case ConsoleKey.A:
                    snake.Dir = Direction.LEFT;
                    break;
                case ConsoleKey.S:
                    snake.Dir = Direction.DOWN;
                    break;
                case ConsoleKey.D:
                    snake.Dir = Direction.RIGHT;
                    break;
                default:
                    break;
            }
            // 뱀이 이동하고, 음식을 먹었는지, 벽이나 자신의 몸에 부딪혔는지 등을 확인하고 처리하는 로직을 작성하세요.
            // 이동, 음식 먹기, 충돌 처리 등의 로직을 완성하세요.
            snake.Move();
            if (Eat(snake, food))
            {
                snake.Increase();
                food = foodCreator.CreateFood(snake);
                food.Draw();
            }
            if (CollisionCheck(snake))
            {
                Console.SetCursorPosition(50, 21);
                Console.WriteLine("Game Over!");
                Thread.Sleep(10000);
                break;
            }

            Thread.Sleep(100); // 게임 속도 조절 (이 값을 변경하면 게임의 속도가 바뀝니다)

            Console.Clear();

            // 뱀의 상태를 출력합니다 (예: 현재 길이, 먹은 음식의 수 등)
            foreach (Point bodyPart in snake.Body)
            {
                bodyPart.Draw();
            }
            food.Draw();


            Console.SetCursorPosition(50, 20);
            Console.WriteLine($"현재 길이: {snake.Body.Count} / 먹은 음식의 수: {snake.EatingCount}");
        }

    }

    private static bool Eat(Snake snake, Point food)
    {
        List<Point> body = snake.Body;
        for (int i = 0; i < body.Count; i++)
        {
            if (body[i].x == food.x && body[i].y == food.y)
            {
                return true;
            }
        }

        return false;
    }

    private static bool CollisionCheck(Snake snake)
    {
        // 몸에 부딪힌 경우
        Point head = snake.Body[0];
        for (int i = 1; i < snake.Body.Count; i++)
        {
            if (head.x == snake.Body[i].x && head.y == snake.Body[i].y)
            {
                return true;
            }
        }


        bool hasNegativePoints = snake.Body.Any(p => p.x <= 0 || p.x >= 20 || p.y <= 0 || p.y >= 20);
        if (hasNegativePoints)
            return true;

        return false;
    }
}

public class FoodCreator
{
    private Random random = new Random();

    public int XLimit { get; set; }
    public int YLimit { get; set; }
    public char Sym { get; set; }

    public FoodCreator(int maxX, int maxY, char sym)
    {
        XLimit = maxX;
        YLimit = maxY;
        Sym = sym;
    }

    public Point CreateFood(Snake snake)
    {
        int x;
        int y;
        while (true)
        {
            x = random.Next(1, XLimit);
            y = random.Next(1, YLimit);

            if (snake.Body.Where(item => item.x == x && item.y == y).Count() == 0)
                break;
        }
        return new Point(x, y, Sym);
    }
}

public class Snake
{
    public List<Point> Body = new List<Point>();
    public Direction Dir { get; set; }

    public int EatingCount { get; set; }

    public Snake(Point p, int length, Direction dir)
    {
        Point deepCopy = new Point(p.x, p.y, p.sym);

        for (int i = 0; i < length; i++)
        {
            Body.Add(deepCopy);

            switch (dir)
            {
                case Direction.LEFT:
                    // p.x++;
                    deepCopy = new Point(deepCopy.x + 1, deepCopy.y, p.sym);
                    break;
                case Direction.RIGHT:
                    // p.x--;
                    deepCopy = new Point(deepCopy.x - 1, deepCopy.y, p.sym);
                    break;
                case Direction.UP:
                    // p.y++;
                    deepCopy = new Point(deepCopy.x, deepCopy.y + 1, p.sym);
                    break;
                case Direction.DOWN:
                    // p.y--;
                    deepCopy = new Point(deepCopy.x, deepCopy.y - 1, p.sym);
                    break;
            }
        }
        Dir = dir;
    }

    public void Move()
    {
        // 머리를 1칸 증가시키고
        Point head = Body[0];
        Point newHead = new Point(head.x, head.y, head.sym);
        switch (Dir)
        {
            case Direction.LEFT:
                newHead.x--;
                break;
            case Direction.RIGHT:
                newHead.x++;
                break;
            case Direction.UP:
                newHead.y--;
                break;
            case Direction.DOWN:
                newHead.y++;
                break;
        }
        Body.Insert(0, newHead);

        // 꼬리를 제거하고
        Body.RemoveAt(Body.Count - 1);
    }

    public void Draw()
    {
        foreach (Point point in Body)
        {
            point.Draw();
        }
    }

    public void Increase()
    {
        EatingCount++;

        Point last = Body[Body.Count - 1];
        Point afterLast = Body[Body.Count - 2];

        if (last.x == afterLast.x && last.y - 1 == afterLast.y)
        {
            // last 아래 생성
            Point point = new Point(last.x, last.y + 1, '*');
            Body.Add(point);
        }
        else if (last.x == afterLast.x && last.y + 1 == afterLast.y)
        {
            // last 위에 생성
            Point point = new Point(last.x, last.y - 1, '*');
            Body.Add(point);
        }
        else if (last.y == afterLast.y && last.x - 1 == afterLast.x)
        {
            // last 오른쪽 생성
            Point point = new Point(last.x + 1, last.y, '*');
            Body.Add(point);
        }
        else if (last.y == afterLast.y && last.x + 1 == afterLast.x)
        {
            // last 왼쪽 생성
            Point point = new Point(last.x - 1, last.y, '*');
            Body.Add(point);
        }
    }
}

public class Point
{
    public int x { get; set; }
    public int y { get; set; }
    public char sym { get; set; }

    // Point 클래스 생성자
    public Point(int _x, int _y, char _sym)
    {
        x = _x;
        y = _y;
        sym = _sym;
    }

    // 점을 그리는 메서드
    public void Draw()
    {
        Console.SetCursorPosition(x, y);
        Console.Write(sym);
    }

    // 점을 지우는 메서드
    public void Clear()
    {
        sym = ' ';
        Draw();
    }

    // 두 점이 같은지 비교하는 메서드
    public bool IsHit(Point p)
    {
        return p.x == x && p.y == y;
    }
}
// 방향을 표현하는 열거형입니다.
public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}