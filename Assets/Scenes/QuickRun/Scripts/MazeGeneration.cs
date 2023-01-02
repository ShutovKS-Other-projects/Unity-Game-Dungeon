using System.Collections.Generic;
using Random = System.Random;

public class MazeGeneration
{
    GameControler gameControler = new GameControler();

    private int width;
    private int height;
    private Cell[,] cells;

    public MazeGeneration()
    {
        this.width = gameControler.widthMaze;
        this.height = gameControler.heightMaze;

        // �������������� ������ �����
        cells = new Cell[gameControler.widthMaze, gameControler.heightMaze];
        for (int x = 0; x < gameControler.widthMaze; x++)
        {
            for (int y = 0; y < gameControler.heightMaze; y++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }
    }

    // ���������� ��������, ��������� ��������� �������� ������ � �������
    public void Generate()
    {
        Random rand = new Random();

        // ������ �� ����������� ��������
        bool[,] visitedCells = new bool[width, height];

        // �������� ��������� ��������� ������
        int startX = rand.Next(width);
        int startY = rand.Next(height);
        Cell currentCell = cells[startX, startY];
        visitedCells[startX, startY] = true;

        // ������� ���� ��� �������� ���������� �����
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(currentCell);

        // ���� ���� ��������������� ������
        while (stack.Count > 0)
        {
            // �������� ������ ������� ������� ������, ������� �� ���� ��������
            List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell, visitedCells);

            // ���� � ������� ������ ���� ������������ ������
            if (unvisitedNeighbors.Count > 0)
            {
                // �������� ���������� ������������� ������
                int index = rand.Next(unvisitedNeighbors.Count);
                Cell neighbor = unvisitedNeighbors[index];

                // ������� ����� ����� ������� ������� � ��������� �������
                RemoveWall(currentCell, neighbor);

                // �������� ������ ��� �����������
                visitedCells[neighbor.X, neighbor.Y] = true;

                // ��������� ������� ������ � ����
                stack.Push(currentCell);

                // ������ ���������� ������ ������� �������
                currentCell = neighbor;
            }
            else
            {
                // ���� � ������� ������ ��� ������������ �������, ��������� �� �� �����
                currentCell = stack.Pop();
            }
        }
    }

    // �������� ������ ������� ��������� ������, ������� �� ���� ��������
    private List<Cell> GetUnvisitedNeighbors(Cell cell, bool[,] visitedCells)
    {
        List<Cell> unvisitedNeighbors = new List<Cell>();

        // ��������� ������ �����
        if (cell.X > 0 && !visitedCells[cell.X - 1, cell.Y])
        {
            unvisitedNeighbors.Add(cells[cell.X - 1, cell.Y]);
        }
        // ��������� ������ ������
        if (cell.X < width - 1 && !visitedCells[cell.X + 1, cell.Y])
        {
            unvisitedNeighbors.Add(cells[cell.X + 1, cell.Y]);
        }
        // ��������� ������ ����
        if (cell.Y > 0 && !visitedCells[cell.X, cell.Y - 1])
        {
            unvisitedNeighbors.Add(cells[cell.X, cell.Y - 1]);
        }
        // ��������� ������ ����
        if (cell.Y < height - 1 && !visitedCells[cell.X, cell.Y + 1])
        {
            unvisitedNeighbors.Add(cells[cell.X, cell.Y + 1]);
        }
        return unvisitedNeighbors;
    }

    // ������� ����� ����� ����� ���������� ��������
    private void RemoveWall(Cell cell1, Cell cell2)
    {
        // ���� ������ ��������� � ����� �������
        if (cell1.X == cell2.X)
        {
            // ���� ������1 ���� ������2
            if (cell1.Y < cell2.Y)
            {
                cell1.Walls[2] = false;
                cell2.Walls[0] = false;
            }
            // ���� ������1 ���� ������2
            else
            {
                cell1.Walls[0] = false;
                cell2.Walls[2] = false;
            }
        }
        // ���� ������ ��������� � ����� ������
        else
        {
            // ���� ������1 ��������� ����� �� ������2
            if (cell1.X < cell2.X)
            {
                cell1.Walls[1] = false;
                cell2.Walls[3] = false;
            }
            // ���� ������1 ��������� ������ �� ������2
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
        // �������� ������� ������ ���������
        str += ("+");
        for (int x = 0; x < width; x++)
        {
            if (cells[x, 0].Walls[1])
            {
                str += ("-+");
            }
            else
            {
                str += ("--");
            }
        }
        str +="\n";

        // �������� ������ ���������
        for (int y = 0; y < height; y++)
        {
            // ������ ����� ����� ���������
            str += ("|");

            // �������� ������ ���������
            for (int x = 0; x < width; x++)
            {
                // �������� ������
                if (cells[x, y].Walls[0])
                {
                    str += (" ");
                }
                else

                {
                    str += (" ");
                }

                // �������� ������ ������ ������
                if (cells[x, y].Walls[1])
                {
                    str += ("|");
                }
                else
                {
                    str += (" ");
                }
            }
            str += ("") + "\n";

            // ������ ������ ������ ���������
            if (y < height - 1)
            {
                if (cells[0, y].Walls[2])
                {
                    str += ("+-");
                }
                else
                {
                    str += ("| ");
                }
                for (int x = 1; x < width; x++)
                {
                    if (cells[x, y].Walls[2])
                    {
                        str += ("--");
                    }
                    else
                    {
                        str += ("- ");
                    }
                }
                if (cells[width-1, y].Walls[2])
                {
                    str += ("+") + "\n";
                }
                else
                {
                    str += ("|") + "\n";
                }
            }
        }

        // ������ ������ ������ ���������
        str += ("+");
        for (int x = 0; x < width; x++)
        {
            if (cells[x, height-1].Walls[1])
            {
                str += ("-+");
            }
            else
            {
                str += ("--");
            }
        }
        str +="\n";
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