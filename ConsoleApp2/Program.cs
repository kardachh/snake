﻿using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading;

class Point
{
    public int x;
    public int y;
    public char sym;
    public Point()
    {
    }
    public Point(int x, int y, char sym)
    {
        this.x = x;
        this.y = y;
        this.sym = sym;
    }
    public void Draw()
    {
        Console.SetCursorPosition(x, y);
        Console.Write(sym);
    }
    public Point(Point p)
    {
        x = p.x;
        y = p.y;
        sym = p.sym;
    }
    public void Move(int offset, Direction direction)
    {
        if (direction == Direction.RIGHT) { x = x + offset; }
        else if (direction == Direction.LEFT) { x = x - offset; }
        else if (direction == Direction.UP) { y = y - offset; }
        else if (direction == Direction.DOWN) { y = y + offset; }
    }
    public void Clear()
    {
        sym = ' ';
        Draw();
    }
    public bool IsHit(Point p)
    {
        return p.x == this.x && p.y == this.y;

    }
}
class Figure
{
    protected List<Point> pList;
    public void Draw()
    {
        foreach (Point p in pList)
            p.Draw();
    }
    internal bool IsHit(Figure figure)
    {
        foreach (var p in pList)
        {
            if (figure.IsHit(p))
                return true;
        }
        return false;
    }
    private bool IsHit(Point point)
    {
        foreach (var p in pList)
        {
            if (p.IsHit(point))
                return true;
        }
        return false;
    }
}
class HorizontalLine : Figure
{
    public HorizontalLine(int xLeft, int xRight, int y, char sym)
    {
        pList = new List<Point>();
        for (int x = xLeft; x <= xRight; x++)
        {
            pList.Add(new Point(x, y, sym));
        }
    }
}
class VerticalLine : Figure
{
    public VerticalLine(int x, int yup, int yDown, char sym)
    {
        pList = new List<Point>();
        for (int y = yup; y <= yDown; y++)
        {
            pList.Add(new Point(x, y, sym));
        }
    }
}
class Square : Figure
{
    public Square(int xUpLeft, int yUpLeft, int xDownRight, int yDownRight, char Sym)
    {
        pList = new List<Point>();
        for (int x = xUpLeft; x <= xDownRight; x++)
        {
            pList.Add(new Point(x, yUpLeft, Sym));
            pList.Add(new Point(x, yDownRight, Sym));
        }
        for (int y = yUpLeft; y <= yDownRight; y++)
        {
            pList.Add(new Point(xUpLeft, y, Sym));
            pList.Add(new Point(xDownRight, y, Sym));
        }
    }
}
enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}
class Snake : Figure
{
    private Direction direction;
    public Snake(Point tail, int lenghth, Direction direction)
    {
        pList = new List<Point>();
        for (int i = 0; i < lenghth; i++)
        {
            Point p = new Point(tail);
            p.Move(30, direction);
            pList.Add(p);
        }
    }
    internal void Move()
    {
        Point tail = pList.First();
        pList.Remove(tail);
        Point head = GetNextPoint();
        pList.Add(head);
        tail.Clear();
        head.Draw();
    }
    public Point GetNextPoint()
    {
        Point head = pList.Last();
        Point nextPoint = new Point(head);
        nextPoint.Move(1, direction);
        return nextPoint;
    }
    public void HandleKey(ConsoleKey key)
    {
        if (key == ConsoleKey.LeftArrow) direction = Direction.LEFT;
        else if (key == ConsoleKey.RightArrow) direction = Direction.RIGHT;
        else if (key == ConsoleKey.UpArrow) direction = Direction.UP;
        else if (key == ConsoleKey.DownArrow) direction = Direction.DOWN;
    }
    internal bool Eat(Point food)
    {
        Point head = GetNextPoint();
        if (head.IsHit(food))
        {
            food.sym = head.sym;
            pList.Add(food);
            return true;
        }
        else return false;
    }
    internal bool Pregr(Point pr)
    {
        Point head = GetNextPoint();
        if (head.IsHit(pr))
        {
            return true;
        }
        else return false;
    }
    public bool IsHit()
    {
        for (int i = pList.Count - 1; --i >= 0;)
        {
            if (pList[i] == pList.Last())
            {
                return true;
            }
        }
        return false;
    }
}
class Walls
{
    List<Figure> wallList;

    public Walls(int mapWidth, int mapHeight)
    {
        wallList = new List<Figure>();

        HorizontalLine upLine = new HorizontalLine(0, 78, 0, '#');
        HorizontalLine downLine = new HorizontalLine(0, 78, 34, '#');
        VerticalLine leftLine = new VerticalLine(0, 0, 34, '#');
        VerticalLine rightLine = new VerticalLine(78, 0, 34, '#');

        wallList.Add(upLine);
        wallList.Add(downLine);
        wallList.Add(leftLine);
        wallList.Add(rightLine);
    }

    internal bool IsHit(Figure figure)
    {
        foreach (var wall in wallList)
        {
            if (wall.IsHit(figure))
            {
                return true;
            }
        }
        return false;
    }

    public void Draw()
    {
        foreach (var wall in wallList)
        {
            wall.Draw();
        }
    }
}
class FoodCreator
{
    // создает координаты игрового поля и символ для изображения еды
    int mapWidth;
    int mapHeight;
    char sym;
    // создание экземпляра класс генератора сл. чисел
    Random random = new Random();
    // конструктор класс с параметрами
    public FoodCreator(int mapWidth, int mapHeight, char sym)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.sym = sym;
    }
    // метод создания сл. точки размещения еды на игровом поле
    public Point CreateFood()
    {
        int x = random.Next(2, mapWidth - 2);
        int y = random.Next(2, mapHeight - 3);
        return new Point(x, y, sym);
    }
}
class Pregrada
{
    Random random = new Random();
    List<Figure> Stenki;
    public Pregrada(int mapWidth, int mapHeight)
    {
        Stenki = new List<Figure>();
        int k = random.Next(5, 8);
        for (int i = 0; i < k; i++)
        {
            int x = random.Next(10, mapWidth - 10);
            int y1 = random.Next(10, mapHeight - 10);
            int y2 = random.Next(10, mapHeight - 10);
            VerticalLine randomVLine = new VerticalLine(x, y1, y2, 'X');
            Stenki.Add(randomVLine);
        }
    }
    internal bool IsHit(Figure figure)
    {
        foreach (var st in Stenki)
        {
            if (st.IsHit(figure))
            {
                return true;
            }
        }
        return false;
    }
    public void Draw()
    {
        foreach (var st in Stenki)
        {
            st.Draw();
        }
    }

}
class Program
{
    static void Main()
    {
        Console.WriteLine("Приятной игры!");
        Thread.Sleep(3000);
        Console.Clear();
        int k = 0;
        Console.SetWindowSize(80, 35);
        Console.SetBufferSize(80, 35);
        Walls walls = new Walls(80, 35);
        walls.Draw();
        Point p = new Point(25, 15, 'O');
        Snake snake = new Snake(p, 4, Direction.RIGHT);
        snake.Draw();
        Console.ForegroundColor = ConsoleColor.Red;
        FoodCreator foodCreator = new FoodCreator(80, 35, '@');
        Point food = foodCreator.CreateFood();
        food.Draw();
        Console.ForegroundColor = ConsoleColor.White;
        Pregrada pregrad = new Pregrada(80, 35);
        pregrad.Draw();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(1, 1);
        Console.WriteLine("Ваш счет: " + k);
        Console.ForegroundColor = ConsoleColor.Red;
        while (true)
        {
            if (walls.IsHit(snake) || snake.IsHit() || pregrad.IsHit(snake))
            {
                break;
            }
            if (snake.Eat(food))
            {
                snake.Draw();
                food = foodCreator.CreateFood();
                food.Draw();
                k++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(1, 1);
                Console.WriteLine("Ваш счет: " + k);
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else { snake.Move(); }
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                snake.HandleKey(key.Key);
            }
            Thread.Sleep(200);
        }
    }
}