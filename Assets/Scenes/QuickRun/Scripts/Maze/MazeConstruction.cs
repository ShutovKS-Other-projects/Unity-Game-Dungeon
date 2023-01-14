using UnityEngine;

public class MazeConstruction : MonoBehaviour
{
    System.Random random = new System.Random();

    public static int width = 10;
    public static int height = 10;


    [SerializeField] GameObject[] Mobe;

    private void Awake()
    {
        Construction();
    }

    void Construction()
    {
        MazeModification mazeModification = new MazeModification();
        char[,] maze = mazeModification.Modification();

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                ConstructionFloor(x, y);
                switch (maze[y,x])
                {
                    case ' ':
                            break;
                    case '-':
                            ConstructionWallHorizontal(x, y);
                            break;
                    case '|':
                            ConstructionWallVertical(x, y);
                            break;
                    case '+':
                            ConstructionWall(x, y);
                            break;
                    case 'M':
                            ConstructionMobe(x, y);
                            break;
                    case 'S':
                            ConstructionStart(x, y);
                            break;
                    case 'E':
                            ConstructionEnd(x, y);
                            break;
                    case 'C':
                            ConstructionChest(x, y);
                            break;
                }
            }
        }
    }

    void ConstructionWall(float x, float y)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, 0.25f, y);
        sector.transform.localScale = new Vector3(0.1f, 1.5f, 2f);
        sector.name = $"sector({y};{x})-[Wall]";
        sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, 0.25f, y);
        sector.transform.localScale = new Vector3(1f, 1.5f, 0.1f);
        sector.name = $"sector({y};{x})-[WallH]";
    }
    void ConstructionWallHorizontal(float x, float y)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, 0.25f, y);
        sector.transform.localScale = new Vector3(1f, 1.5f, 0.1f);
        sector.name = $"sector({y};{x})-[WallH]";
    }
    void ConstructionWallVertical(float x, float y)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, 0.25f, y);
        sector.transform.localScale = new Vector3(0.1f, 1.5f, 2f);
        sector.name = $"sector({y};{x})-[WallV]";
    }
    void ConstructionFloor(float x, float y)
    {
        float n = 0.335f;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sector.transform.position = new Vector3(x - n * (1 - i), -0.45f, y - n * (1 - j));
                sector.transform.localScale = new Vector3(0.335f, 0.1f, 0.335f);
                sector.name = $"sector({y};{x})-[Floor({i};{j}]";

            }
        }
    }
    void ConstructionStart(float x, float y)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, -0.45f, y);
        sector.transform.localScale = new Vector3(0.335f, 0.1f, 0.335f);
        sector.GetComponent<MeshRenderer>().material.color = Color.green;
        sector.name = $"Start";
    }
    void ConstructionEnd(float x, float y)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = new Vector3(x, 0, y);
        sector.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sector.GetComponent<MeshRenderer>().material.color = Color.red;
        sector.name = $"End";
    }
    void ConstructionMobe(float x, float y)
    {
        GameObject mobe = Instantiate(Mobe[random.Next(Mobe.Length)]);
        mobe.transform.position = new Vector3(x, 0, y);
        mobe.name = $"Mobe";
    }
    void ConstructionChest(float x, float y)
    {

    }
    void Construction(float x, float y)
    {

    }
}