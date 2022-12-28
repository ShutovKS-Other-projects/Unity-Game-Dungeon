using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = System.Random;

public class MazeGeneration
{
        private int width;
        private int height;
        private Cell[,] cells;

        public MazeGeneration(int width, int height)
        {
            this.width  =width;
            this.height =height;

            // »нициализируем массив €чеек
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }

        // √енерируем лабиринт, использу€ случайный алгоритм поиска в глубину
        public void GenerateMaze()
        {
            Random rand = new Random();

            // —ледим за посещенными €чейками
            bool[,] visitedCells = new bool[width, height];

            // ¬ыбираем случайную начальную €чейку
            int startX = rand.Next(width);
            int startY = rand.Next(height);
            Cell currentCell = cells[startX, startY];
            visitedCells[startX, startY] = true;

            // —оздаем стек дл€ хранени€ посещенных €чеек
            Stack<Cell> stack = new Stack<Cell>();
            stack.Push(currentCell);

            // ѕока есть непросмотренные €чейки
            while (stack.Count > 0)
            {
                // ѕолучить список соседей текущей €чейки, которые не были посещены
                List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell, visitedCells);

                // ≈сли у текущей €чейки есть непосещенные соседи
                if (unvisitedNeighbors.Count > 0)
                {
                    // ¬ыбираем случайного непосещенного соседа
                    int index = rand.Next(unvisitedNeighbors.Count);
                    Cell neighbor = unvisitedNeighbors[index];

                    // ”дал€ем стену между текущей €чейкой и выбранным соседом
                    RemoveWall(currentCell, neighbor);

                    // ќтмечаем соседа как посещенного
                    visitedCells[neighbor.X, neighbor.Y] = true;

                    // ѕоместить текущую €чейку в стек
                    stack.Push(currentCell);

                    // ƒелаем выбранного соседа текущей €чейкой
                    currentCell = neighbor;
                }
                else
                {
                    // ≈сли у текущей €чейки нет непосещенных соседей, извлекаем ее из стека
                    currentCell = stack.Pop();
                }
            }
        }

        // ѕолучить список соседей указанной €чейки, которые не были посещены
        private List<Cell> GetUnvisitedNeighbors(Cell cell, bool[,] visitedCells)
        {
            List<Cell> unvisitedNeighbors = new List<Cell>();

            // ѕровер€ем €чейку слева
            if (cell.X > 0 && !visitedCells[cell.X - 1, cell.Y])
            {
                unvisitedNeighbors.Add(cells[cell.X - 1, cell.Y]);
            }
            // ѕровер€ем €чейку справа
            if (cell.X < width - 1 && !visitedCells[cell.X + 1, cell.Y])
            {
                unvisitedNeighbors.Add(cells[cell.X + 1, cell.Y]);
            }
            // ѕровер€ем €чейку выше
            if (cell.Y > 0 && !visitedCells[cell.X, cell.Y - 1])
            {
                unvisitedNeighbors.Add(cells[cell.X, cell.Y - 1]);
            }
            // ѕровер€ем €чейку ниже
            if (cell.Y < height - 1 && !visitedCells[cell.X, cell.Y + 1])
            {
                unvisitedNeighbors.Add(cells[cell.X, cell.Y + 1]);
            }
            return unvisitedNeighbors;
        }

        // ”бираем стену между двум€ указанными €чейками
        private void RemoveWall(Cell cell1, Cell cell2)
        {
            // ≈сли €чейки наход€тс€ в одном столбце
            if (cell1.X == cell2.X)
            {
                // ≈сли €чейка1 выше €чейки2
                if (cell1.Y < cell2.Y)
                {
                    cell1.Walls[2] = false;
                    cell2.Walls[0] = false;
                }
                // ≈сли €чейка1 ниже €чейки2
                else
                {
                    cell1.Walls[0] = false;
                    cell2.Walls[2] = false;
                }
            }
            // ≈сли €чейки наход€тс€ в одной строке
            else
            {
                // ≈сли €чейка1 находитс€ слева от €чейки2
                if (cell1.X < cell2.X)
                {
                    cell1.Walls[1] = false;
                    cell2.Walls[3] = false;
                }
                // ≈сли €чейка1 находитс€ справа от €чейки2
                else
                {
                    cell1.Walls[3] = false;
                    cell2.Walls[1] = false;
                }
            }
        }

        public string StringMaze()
        {
            string str = null;
            // ѕечатаем верхнюю строку лабиринта
            for (int x = 0; x < width; x++)
            {
                str +=("+--");
            }
            str +=("+") + "\n";

            // ѕечатаем строки лабиринта
            for (int y = 0; y < height; y++)
            {
                // ѕечать левой стены лабиринта
                str +=("|");

                // ѕечатаем €чейки лабиринта
                for (int x = 0; x < width; x++)
                {
                    // ѕечатаем €чейку
                    if (cells[x, y].Walls[0])
                    {
                        str +=("  ");
                    }
                    else

                    {
                        str +=("  ");
                    }

                    // ѕечатаем правую стенку €чейки
                    if (cells[x, y].Walls[1])
                    {
                        str +=("|");
                    }
                    else
                    {
                        str +=(" ");
                    }
                }
                str +=("") + "\n";

                // ѕечать нижней строки лабиринта
                if (y < height - 1)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (cells[x, y].Walls[2])
                        {
                            str +=("+--");
                        }
                        else
                        {
                            str +=("+  ");
                        }
                    }
                    str +=("+") + "\n";
                }
            }

            // ѕечать нижней строки лабиринта
            for (int x = 0; x < width; x++)
            {
                str +=("+--");
            }
            str +=("+") + "\n";
            return str;
        }
    }

class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool[] Walls { get; set; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Walls = new bool[4] { true, true, true, true };
    }
}