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
        Transform mazeParent = new GameObject("Maze Hierarhy").transform;

        MazeModification mazeModification = new MazeModification();
        char[,] maze = mazeModification.Modification();

        for (int y = 0; y < maze.GetLength(0); y++)
        {
            for (int x = 0; x < maze.GetLength(1); x++)
            {
                Vector3 position = new Vector3(x, 0, y) * 3;

                ConstructionFoundations(position, mazeParent, new Vector3(3f, 0.1f, 3f));
                //ConstructionFoundations(position + new Vector3(0f, 4f, 0f), mazeParent, new Vector3(3f, 0.1f, 3f));
                switch (maze[y, x])
                {
                    case ' ':
                        break;
                    case '-':
                        //ConstructionFoundations(position, mazeParent, new Vector3(3f, 8f, 0.2f)); //перепроверить размеры
                        break;
                    case '|':
                        //ConstructionFoundations(position, mazeParent, new Vector3(0.2f, 8f, 6f)); //перепроверить размеры
                        break;
                    case '+':
                        //ConstructionFoundations(position, mazeParent, new Vector3(3f, 8f, 0.2f));
                        //ConstructionFoundations(position, mazeParent, new Vector3(0.2f, 8f, 6f));
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

        void ConstructionFoundations(Vector3 position, Transform mazeParent, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject sector = Instantiate(obj, position, Quaternion.identity, mazeParent); ;
            sector.transform.localScale = scale;
            Destroy(obj);
        }
        void ConstructionStart(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject sector = Instantiate(obj, position, Quaternion.identity); ;
            sector.transform.localScale = new Vector3(0.335f, 0.1f, 0.335f);
            sector.GetComponent<MeshRenderer>().material.color = Color.green;
            sector.name = $"Start";
            Destroy(obj);
        }
        void ConstructionEnd(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject sector = Instantiate(obj, position, Quaternion.identity); ;
            sector.transform.position += new Vector3(0, 0.1f, 0);
            sector.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sector.GetComponent<MeshRenderer>().material.color = Color.red;
            sector.name = $"End";
            Destroy(obj);
        }
        void ConstructionMobe(Vector3 position)
        {
            GameObject mobe = Instantiate(Mobe[random.Next(Mobe.Length)]);
            mobe.AddComponent<InteractableDeadMobe>();
            mobe.transform.position = position;
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
}