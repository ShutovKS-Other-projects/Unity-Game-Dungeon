using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MazeConstruction : MonoBehaviour
{
    GameControler gameControler = new GameControler();

    [SerializeField] GameObject Floor;
    [SerializeField] GameObject Wall;

    private int width;
    private int height;
    private string maze;

    private void Awake()
    {
        width = gameControler.widthMaze;
        height = gameControler.heightMaze;

        MazeGeneration mazeGeneration = new MazeGeneration(width, height);

        // √енерируем лабиринт, использу€ случайный алгоритм поиска в глубину
        mazeGeneration.GenerateMaze();
        maze = mazeGeneration.StringMaze();

    }

    private void Start()
    {
        MezeConstruct();
    }

    void MezeConstruct()
    {
        GameObject sector;
        int y = 0; int x = 0;
        for (int i = 0; i < maze.Length; i++)
        {
            switch (maze[i])
            {
                case '\n':
                    {
                        y++;
                        x = 0;
                        continue;
                    }
                case ' ':
                    {
                        ConstructionFloor();
                        x++;
                        continue;
                    }
                default:
                    {
                        ConstructionWall();
                        x++;
                        continue;
                    }
            }
        }

        void ConstructionFloor()
        {
            sector = Instantiate(Floor, new Vector3(x - 0.25f, 0, y - 0.25f), new Quaternion(0, 0, 0, 0));
            sector.name = $"sector({y};{x})-[Floor1]";
            sector = Instantiate(Floor, new Vector3(x - 0.25f, 0, y + 0.25f), new Quaternion(0, 0, 0, 0));
            sector.name = $"sector({y};{x})-[Floor2]";
            sector = Instantiate(Floor, new Vector3(x + 0.25f, 0, y - 0.25f), new Quaternion(0, 0, 0, 0));
            sector.name = $"sector({y};{x})-[Floor3]";
            sector = Instantiate(Floor, new Vector3(x + 0.25f, 0, y + 0.25f), new Quaternion(0, 0, 0, 0));
            sector.name = $"sector({y};{x})-[Floor4]";
        }

        void ConstructionWall()
        {
            sector = Instantiate(Wall, new Vector3(x, 0, y), new Quaternion(0, 0, 0, 0));
            sector.name = $"sector({y};{x})-[Wall]";
        }
    }
}
