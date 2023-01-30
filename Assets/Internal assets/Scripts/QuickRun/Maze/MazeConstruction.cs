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
                Vector3 position = new Vector3(x, 0, y) * 3;
                ConstructionFloor(position);
                switch (maze[y, x])
                {
                    case ' ':
                        break;
                    case '-':
                        ConstructionWallHorizontal(position);
                        break;
                    case '|':
                        ConstructionWallVertical(position);
                        break;
                    case '+':
                        ConstructionWall(position);
                        break;
                    case 'M':
                        ConstructionMobe(position);
                        break;
                    case 'S':
                        ConstructionStart(position);
                        break;
                    case 'E':
                        ConstructionEnd(position);
                        break;
                    case 'C':
                        ConstructionChest(position);
                        break;
                }
            }
        }
    }

    void ConstructionWall(Vector3 position)
    {
        ConstructionWallHorizontal(position);
        ConstructionWallVertical(position);
    }
    void ConstructionWallHorizontal(Vector3 position)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = position;
        sector.transform.localScale = new Vector3(3f, 8f, 0.2f);
    }
    void ConstructionWallVertical(Vector3 position)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = position;
        sector.transform.localScale = new Vector3(0.2f, 8f, 6f);
    }
    void ConstructionFloor(Vector3 position)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.localScale = new Vector3(3f, 0.1f, 3f);
        sector.transform.position = position;
        sector.GetComponent<MeshRenderer>().material.color = Color.gray;

        GameObject sector2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector2.transform.localScale = new Vector3(3f, 0.1f, 3f);
        sector2.transform.position = position + new Vector3(0, 4f, 0);
        sector2.GetComponent<MeshRenderer>().material.color = Color.gray;
    }
    void ConstructionStart(Vector3 position)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = position;
        sector.transform.localScale = new Vector3(0.335f, 0.1f, 0.335f);
        sector.GetComponent<MeshRenderer>().material.color = Color.green;
        sector.name = $"Start";
    }
    void ConstructionEnd(Vector3 position)
    {
        GameObject sector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sector.transform.position = position + new Vector3(0, 0.1f, 0);
        sector.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sector.GetComponent<MeshRenderer>().material.color = Color.red;
        sector.name = $"End";
    }
    void ConstructionMobe(Vector3 position)
    {
        GameObject mobe = Instantiate(Mobe[random.Next(Mobe.Length)]);
        mobe.transform.position = position;
        mobe.tag = "Mobe";
    }
    void ConstructionChest(Vector3 position)
    {
        //GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //chest.transform.position = position;
        //chest.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //chest.GetComponent<MeshRenderer>().material.color = Color.yellow;
        //chest.name = $"Chest";
    }
}