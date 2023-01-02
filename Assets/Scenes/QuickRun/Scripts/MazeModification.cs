using System;

public class MazeModification
{
    MazeGeneration mazeGeneration = new MazeGeneration();

    Random random = new Random();

    private string mazeString;

    public int[,] Modification()
    {
        mazeGeneration.Generate();
        mazeString = mazeGeneration.StringMaze();

        int n = 0;
        foreach (char ch in mazeString)
        {
            if (ch == '\n')
                n++;
        }
        int[,] mazeInt = new int[n,mazeString.Length/n-1];

        int y = 0, x = 0;
        foreach (char ch in mazeString)
        {
            switch(ch)
            {
                case ' ':
                    {
                        if (random.Next(n) != 0)
                        {
                            mazeInt[y, x] = 0;
                        }
                        else
                        {
                            switch (random.Next(2))
                            {
                                case 0:
                                    {
                                        mazeInt[y, x] = 10;
                                        continue;
                                    }
                                case 1:
                                    {
                                        mazeInt[y, x] = 11;
                                        continue;
                                    }
                            }
                        }
                        x++;
                        continue;
                    }
                case '-':
                    {
                        mazeInt[y, x] = -1;
                        x++;
                        continue;
                    }
                case '|':
                    {
                        mazeInt[y, x] = 1;
                        x++;
                        continue;
                    }
                case '+':
                    {
                        mazeInt[y, x] = 2;
                        x++;
                        continue;
                    }
                case '\n':
                    {
                        y++;
                        x = 0;
                        continue;
                    }
            }
        }
        return mazeInt;
    }
}
