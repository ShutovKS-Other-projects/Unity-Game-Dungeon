using System;

public class MazeModification
{
    MazeGeneration mazeGeneration = new MazeGeneration();
    private Random random = new Random();
    char[,] mazeChar;

    public char[,] Modification()
    {
        mazeGeneration.Generate();

        MazeConvertToChar();
        AddStart();
        AddEnd();
        AddMobe();
        AddChest();

        return mazeChar;
    }

    void MazeConvertToChar()
    {
        string mazeString = mazeGeneration.StringMaze();
        int y = 0, x = 0;
        mazeChar = new char[mazeGeneration.height * 2 + 1, mazeGeneration.width * 2 + 1];
        foreach (char ch in mazeString)
        {
            switch (ch)
            {
                case ' ':
                    {
                        mazeChar[y, x] = ' ';
                        x++;
                        continue;
                    }
                case '-':
                    {
                        mazeChar[y, x] = '-';
                        x++;
                        continue;
                    }
                case '|':
                    {
                        mazeChar[y, x] = '|';
                        x++;
                        continue;
                    }
                case '+':
                    {
                        mazeChar[y, x] = '+';
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
    }
    void AddStart()
    {
        bool start = false;
        while (!start)
        {
            int y = random.Next(mazeChar.GetLength(0));
            int x = random.Next(mazeChar.GetLength(1));
            if (y < mazeChar.GetLength(0) / 2 && x < mazeChar.GetLength(1) / 2)
            {
                if (mazeChar[x, y] == ' ')
                {
                    mazeChar[x, y] = 'S';
                    start = true;
                }
            }
        }
    }
    void AddEnd()
    {
        bool end = false;
        while (!end)
        {
            int y = random.Next(mazeChar.GetLength(0));
            int x = random.Next(mazeChar.GetLength(1));
            if (y > mazeChar.GetLength(0) / 2 && x > mazeChar.GetLength(1) / 2)
            {
                if (mazeChar[x, y] == ' ')
                {
                    mazeChar[x, y] = 'E';
                    end = true;
                }
            }
        }
    }
    void AddMobe()
    {
        int numeFloor = 0;
        foreach (char c in mazeChar)
        {
            if (c == ' ')
            {
                numeFloor++;
            }
        }
        numeFloor /= 10;

        for (int n = 0; n < numeFloor; n++)
        {
            int y = random.Next(5, mazeChar.GetLength(0));
            int x = random.Next(5, mazeChar.GetLength(1));
            if (mazeChar[x, y] == ' ')
            {
                mazeChar[x, y] = 'M';
            }
        }
    }
    void AddChest()
    {
        int numeFloor = 0;
        foreach (char c in mazeChar)
        {
            if (c == ' ')
            {
                numeFloor++;
            }
        }
        numeFloor /= 9;

        for (int n = 0; n < numeFloor; n++)
        {
            int y = random.Next(5, mazeChar.GetLength(0));
            int x = random.Next(5, mazeChar.GetLength(1));
            if (mazeChar[x, y] == ' ')
            {
                mazeChar[x, y] = 'C';
            }
        }
    }
}
