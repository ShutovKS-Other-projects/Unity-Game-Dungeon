using System;

public class MazeModification
{
    GameControler  gameControler;
    MazeGeneration mazeGeneration;
    
    Random random = new Random();

    private string mazeString;

    public int[,] Modification()
    {
        mazeGeneration = new MazeGeneration();

        mazeGeneration.Generate();
        mazeString = mazeGeneration.StringMaze();

        int[,] mazeInt = new int[mazeGeneration.height * 2 + 1, mazeGeneration.width * 2 + 1];

        int n = 0;
        foreach (char ch in mazeString)
        {
            if (ch == '\n')
                n++;
        }

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

                            mazeInt[y, x] = random.Next(11,13);
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
