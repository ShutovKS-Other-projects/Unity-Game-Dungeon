// онструктор лабиринта
using UnityEngine;

public class MazeConstructorOld : MonoBehaviour
{
    public bool showDebug;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    private MazeDataGeneratorOld dataGeneratorOld;
    private MazeMeshGeneratorOld meshGeneratorOld;

    public float  hallWidth  { get; private set; }
    public float  hallHeight { get; private set; }
    public int    startRow   { get; private set; }
    public int    startCol   { get; private set; }
    public int    goalRow    { get; private set; }
    public int    goalCol    { get; private set; }
    public int[,] data       { get; private set; } //—войство данных

    void Awake()
    {
        dataGeneratorOld = new MazeDataGeneratorOld();
        meshGeneratorOld = new MazeMeshGeneratorOld();

        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
    TriggerEventHandler startCallback = null, TriggerEventHandler goalCallback = null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers are needed to generate a maze./Ќеобходимы нечетные числа дл€ генерации лабиринта.");
        }

        DisposeOldMaze();

        data = dataGeneratorOld.FromDimensions(sizeRows, sizeCols); //передаЄм размер сетки и сохран€ем полученные данные

        FindStartPosition();
        FindGoalPosition();

        hallWidth = meshGeneratorOld.width;
        hallHeight = meshGeneratorOld.height;

        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }


    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        /* –аспечатывает встроенную строчку. »спользует совершенно новую систему графического интерфейса дл€ дисплеев, видимых игроком, 
         * но более стара€ система используетс€ дл€ создани€ дисплеев быстрой отладки без особого труда.*/
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    private void DisplayMaze()
    {
        GameObject labirint = new GameObject();
        labirint.transform.position = Vector3.zero;
        labirint.name = "Procedural Maze";
        labirint.tag = "Generated";

        MeshFilter meshFilter = labirint.AddComponent<MeshFilter>();
        meshFilter.mesh = meshGeneratorOld.FromData(data);

        MeshCollider meshCollider = labirint.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;

        MeshRenderer MeshRenderer = labirint.AddComponent<MeshRenderer>();
        MeshRenderer.materials = new Material[2] { mazeMat1, mazeMat2 };
    }

    public void DisposeOldMaze()
    {
        GameObject[] generated = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject gameObject in generated)
        {
            Destroy(gameObject);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    goalRow = i;
                    goalCol = j;
                    return;
                }
            }
        }
    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject gameObject= GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        gameObject.name = "Start Trigger";
        gameObject.tag = "Generated";

        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter triggerEventRouter = gameObject.AddComponent<TriggerEventRouter>();
        triggerEventRouter.callback = callback;
    }

    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject gameObject= GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        gameObject.name = "Treasure";
        gameObject.tag = "Generated";

        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter triggerEventRouter = gameObject.AddComponent<TriggerEventRouter>();
        triggerEventRouter.callback = callback;
    }
}


