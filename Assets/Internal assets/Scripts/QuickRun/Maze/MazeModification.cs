using System;

public class MazeModification
{
    private MazeGeneration _mazeGeneration = new MazeGeneration();
    private Random _random = new Random();
    private char[,] _mazeChar;

    public char[,] Modification()
    {
        _mazeGeneration.Generate();

        ConvertMazeToChar();
        AddStart();
        AddEnd();
        AddMob();
        AddChest();

        return _mazeChar;
    }

    private void ConvertMazeToChar()
    {
        string mazeString = _mazeGeneration.StringMaze();
        int y = 0, x = 0;
        _mazeChar = new char[_mazeGeneration.height * 2 + 1, _mazeGeneration.width * 2 + 1];
        foreach (char ch in mazeString)
        {
            switch (ch)
            {
                case ' ':
                    _mazeChar[y, x] = ' ';
                    x++;
                    break;
                case '-':
                    _mazeChar[y, x] = '-';
                    x++;
                    break;
                case '|':
                    _mazeChar[y, x] = '|';
                    x++;
                    break;
                case '+':
                    _mazeChar[y, x] = '+';
                    x++;
                    break;
                case '\n':
                    y++;
                    x = 0;
                    break;
            }
        }
    }

    private void AddStart()
    {
        while (true)
        {
            int y = _random.Next(_mazeChar.GetLength(0));
            int x = _random.Next(_mazeChar.GetLength(1));
            if (y < _mazeChar.GetLength(0) / 2 && x < _mazeChar.GetLength(1) / 2)
            {
                if (_mazeChar[x, y] == ' ')
                {
                    _mazeChar[x, y] = 'S';
                    break;
                }
            }
        }
    }

    private void AddEnd()
    {
        while (true)
        {
            int y = _random.Next(_mazeChar.GetLength(0));
            int x = _random.Next(_mazeChar.GetLength(1));
            if (y > _mazeChar.GetLength(0) / 2 && x > _mazeChar.GetLength(1) / 2)
            {
                if (_mazeChar[x, y] == ' ')
                {
                    _mazeChar[x, y] = 'E';
                    break;
                }
            }
        }
    }
    private void AddMob()
    {
        int numFloor = 0;
        foreach (char c in _mazeChar)
        {
            if (c == ' ')
            {
                numFloor++;
            }
        }
        numFloor /= 10;

        for (int n = 0; n < numFloor; n++)
        {
            int y = _random.Next(5, _mazeChar.GetLength(0));
            int x = _random.Next(5, _mazeChar.GetLength(1));
            if (_mazeChar[x, y] == ' ')
            {
                _mazeChar[x, y] = 'M';
            }
        }
    }
    private void AddChest()
    {
        int numFloor = 0;
        foreach (char c in _mazeChar)
        {
            if (c == ' ')
            {
                numFloor++;
            }
        }
        numFloor /= 9;
        for (int n = 0; n < numFloor; n++)
        {
            int y = _random.Next(5, _mazeChar.GetLength(0));
            int x = _random.Next(5, _mazeChar.GetLength(1));
            if (_mazeChar[x, y] == ' ')
            {
                _mazeChar[x, y] = 'C';
            }
        }
    }
}
