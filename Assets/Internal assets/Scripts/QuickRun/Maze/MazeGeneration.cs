using System.Collections.Generic;
using Random = System.Random;

public class MazeGeneration
{
    public int width;
    public int height;
    private Cell[,] cells;

    public MazeGeneration()
    {
        this.width = MazeConstruction.width;
        this.height = MazeConstruction.height;

        cells = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }
    }

    public void Generate()
    {
        Random rand = new Random();

        // Keeps track of visited cells
        bool[,] visitedCells = new bool[width, height];

        // Choose a random starting cell
        int startX = rand.Next(width);
        int startY = rand.Next(height);
        Cell currentCell = cells[startX, startY];
        visitedCells[startX, startY] = true;

        // Create a stack for backtracking
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(currentCell);

        // While the stack is not empty
        while (stack.Count > 0)
        {
            // Get unvisited neighbors of the current cell
            List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell, visitedCells);
            // If there are unvisited neighbors
            if (unvisitedNeighbors.Count > 0)
            {
                // Choose a random unvisited neighbor
                int index = rand.Next(unvisitedNeighbors.Count);
                Cell neighbor = unvisitedNeighbors[index];

                // Remove the wall between the current cell and the neighbor
                currentCell.RemoveWall(neighbor);

                // Mark the neighbor as visited
                visitedCells[neighbor.X, neighbor.Y] = true;

                // Add the current cell to the stack
                stack.Push(currentCell);

                // Make the neighbor the current cell
                currentCell = neighbor;
            }
            else
            {
                // If there are no unvisited neighbors, backtrack by popping a cell from the stack
                currentCell = stack.Pop();
            }
        }
    }

        public string StringMaze()
    {
        string mazeString = null;
        mazeString += ("+");
        for (int x = 0; x < width; x++)
        {
            if (cells[x, 0].Walls[1])
            {
                mazeString += ("-+");
            }
            else
            {
                mazeString += ("--");
            }
        }
        mazeString +="\n";

        for (int y = 0; y < height; y++)
        {
            mazeString += ("|");

            for (int x = 0; x < width; x++)
            {
                if (cells[x, y].Walls[0])
                    mazeString += (" ");
                else
                    mazeString += (" ");

                if (cells[x, y].Walls[1])
                    mazeString += ("|");
                else
                    mazeString += (" ");
            }
            mazeString += ("") + "\n";

            if (y < height - 1)
            {
                if (cells[0, y].Walls[2])
                    mazeString += ("+-");
                else
                    mazeString += ("| ");
                for (int x = 1; x < width; x++)
                {
                    if (cells[x, y].Walls[2])
                        mazeString += ("--");
                    else
                        mazeString += ("- ");
                }
                if (cells[width-1, y].Walls[2])
                    mazeString += ("+") + "\n";
                else
                    mazeString += ("|") + "\n";
            }
        }

        mazeString += ("+");
        for (int x = 0; x < width; x++)
        {
            if (cells[x, height-1].Walls[1])
                mazeString += ("-+");
            else
                mazeString += ("--");
        }
        mazeString +="\n";
        return mazeString;
    }

    // Gets the unvisited neighbors of a cell
    private List<Cell> GetUnvisitedNeighbors(Cell cell, bool[,] visitedCells)
    {
        List<Cell> unvisitedNeighbors = new List<Cell>();
        // Check the cell to the left
        if (cell.X > 0 && !visitedCells[cell.X - 1, cell.Y])
        {
            unvisitedNeighbors.Add(cells[cell.X - 1, cell.Y]);
        }
        // Check the cell to the right
        if (cell.X < width - 1 && !visitedCells[cell.X + 1, cell.Y])
        {
            unvisitedNeighbors.Add(cells[cell.X + 1, cell.Y]);
        }
        // Check the cell above
        if (cell.Y > 0 && !visitedCells[cell.X, cell.Y - 1])
        {
            unvisitedNeighbors.Add(cells[cell.X, cell.Y - 1]);
        }
        // Check the cell below
        if (cell.Y < height - 1 && !visitedCells[cell.X, cell.Y + 1])
        {
            unvisitedNeighbors.Add(cells[cell.X, cell.Y + 1]);
        }

        return unvisitedNeighbors;
    }
    public class Cell
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool[] Walls { get; private set; }
        public bool Visited { get; set; }
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            Walls = new bool[] { true, true, true, true };
            Visited = false;
        }

        public void RemoveWall(Cell neighbor)
        {
            // If the neighbor is to the left of this cell
            if (X - 1 == neighbor.X)
            {
                Walls[3] = false;
                neighbor.Walls[1] = false;
            }
            // If the neighbor is to the right of this cell
            else if (X + 1 == neighbor.X)
            {
                Walls[1] = false;
                neighbor.Walls[3] = false;
            }
            // If the neighbor is above this cell
            else if (Y - 1 == neighbor.Y)
            {
                Walls[0] = false;
                neighbor.Walls[2] = false;
            }
            // If the neighbor is below this cell
            else if (Y + 1 == neighbor.Y)
            {
                Walls[2] = false;
                neighbor.Walls[0] = false;
            }
        }
    }
}