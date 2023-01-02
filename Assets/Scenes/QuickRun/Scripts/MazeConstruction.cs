using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MazeConstruction : MonoBehaviour
{
    MazeModification mazeModification = new MazeModification();

    [SerializeField] GameObject Floor;
    [SerializeField] GameObject Wall;
    [SerializeField] GameObject WallHorizontal;
    [SerializeField] GameObject WallVertical;

    private int[,] maze;

    private void Awake()
    {
        Construction();
    }

    void Construction()
    {
        maze = mazeModification.Modification();
        int y, x;
        for (y = 0; y < maze.GetLength(0); y++)
        {
            for (x = 0; x < maze.GetLength(1); x++)
            {
                switch (maze[y, x])
                {
                    case 0:
                        {
                            ConstructionFloor(x, y);
                            continue;
                        }
                    case -1:
                        {
                            ConstructionWallHorizontal(x, y);
                            continue;
                        }
                    case 1:
                        {
                            ConstructionWallVertical(x, y);
                            continue;
                        }
                    case 2:
                        {
                            ConstructionWall(x, y);
                            continue;
                        }
                }
            }
        }
    }

    void ConstructionFloor(float x, float y)
    {
        float n = 0.335f;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject sector = Instantiate(Floor, new Vector3(x - n * (1 - i), 0, y - n * (1 - j)), new Quaternion(0, 0, 0, 0));
                sector.name = $"sector({y};{x})-[Floor({i};{j}]";
            }
        }
    }

    void ConstructionWall(float x, float y)
    {
        GameObject sector = Instantiate(Wall, new Vector3(x, 0, y), new Quaternion(0, 0, 0, 0));
        sector.name = $"sector({y};{x})-[Wall1]";
    }
    void ConstructionWallHorizontal(float x, float y)
    {
        GameObject sector = Instantiate(WallHorizontal, new Vector3(x, 0, y), new Quaternion(0, 0, 0, 0));
        sector.name = $"sector({y};{x})-[Wall1]";
    }
    void ConstructionWallVertical(float x, float y)
    {
        GameObject sector = Instantiate(WallVertical, new Vector3(x, 0, y), new Quaternion(0, 0, 0, 0));
        sector.name = $"sector({y};{x})-[Wall1]";
    }
}