using UnityEngine;

//����������� ���������
public class MazeConstructor : MonoBehaviour
{
    /* showDebug ����� �������������� ��� ����������� ���� �������.*/ 
    public bool showDebug;

    [SerializeField] private Material mazeMat1;    //��� � ������� ���������
    [SerializeField] private Material mazeMat2;    //����� ���������
    [SerializeField] private Material startMat;    //���� � ������
    [SerializeField] private Material treasureMat; //���� ����

    private MazeDataGenerator dataGenerator; // ���������� ��� �������� ���������� ������
    private MazeMeshGenerator meshGenerator; // �������� ���������� �����
    static public GameObject Player; //����� (�� ������������)

    //�������� ��� �������� �������� � ���������
    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    /* �������� ������. ���������� ������� (�� ���� ���������� �������� ��� ���������, 
     * �� ����� ���������� �������� ������) ������ ��� ��������� ������ ��� ������ ��� ����� ������. 
     * ����� �������, ������ ��������� �� ����� ���� �������� �����.*/
    public int[,] data
    {
        get; private set;
    }

    /* ������������� ������ � �������� 3 �� 3, ������� �������� ����. 1 ������ ��� ��� ������, � �� ����� ��� 0 �������� ������, 
     * ������� ��� ����� �� ��������� �������� ������ ������������ ��������.*/
    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator(); //�������� ���������� ����� � ����� ����
        Player = GameObject.Find("Player"); //������ ������ �� ������ ������ (�� ������������)

        // �� ��������� ������������ �����, ���������� ���� ������ ������
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
        //�������� �������� ���������, ���������� �������� �����
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers are needed to generate a maze./���������� �������� ����� ��� ��������� ���������.");
        }

        DisposeOldMaze(); //������� ����� ������������, ��� ������ ��������.

        data = dataGenerator.FromDimensions(sizeRows, sizeCols); //������� ������ ����� � ��������� ���������� ������

        FindStartPosition();
        FindGoalPosition();

        //������ ��������, ������������ ��� �������� ���� �����
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        /* DisplayMaze() �������� MazeMeshGenerator.FromData(), ��������� ���� ����� � �������� �������� ������ GameObject, ������������ ��� Generated, 
         * �������� MeshFilter � �������������� ���, �������� MeshCollider ��� ������������ � ����������, MeshRenderer � ���������� ��� ����.*/
        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }


    void OnGUI() // ��� ����������� ������ ��������� � ��������, ��� ��� ��������
    {
        // ���� ��� ���������, �������� �� ���������� �������.
        if (!showDebug)
        {
            return;
        }

        /* �� ������� ���������������� ��������� ������� ��������� ����������: 
         * ����� �������� ���������, ������������ ������, �������, � ����� ������ ��� ����������.*/
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        /* ��� ��������� ����� ���������� ������ � ������� ���������� �������. ��� ������ �� ������ ��� ������� ������� �������, 
         * ��� ����� ��������� ����������� �������� � ��������� ���� �....�, ���� �==�, ��� ������� �� ����, ��������� �� �������� ����. 
         * ����� ����, ��� ��������� ����� ������ ����� �������� �� ���� �������� � ������, ��� ��� ������ ������ �������� ����� ��������.*/
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

        /* ������������� ���������� �������. ���������� ���������� ����� ������� ������������ ���������� ��� ��������, ������� �������, 
         * �� ����� ������ ������� ������������ ��� �������� �������� ������� ������� ��� ������� �����.*/
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    /* DisplayMaze() �������� MazeMeshGenerator.FromData() (���������� ��������)*/
    private void DisplayMaze()
    {
        GameObject gameObject= new GameObject(); //������ ��������
        gameObject.transform.position = Vector3.zero; //���������� �������� �� (0,0,0) �������
        gameObject.name = "Procedural Maze"; //����� ���
        gameObject.tag = "Generated"; //��������� ���

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = meshGenerator.FromData(data); //��������� MeshFilter � �������������� ���.

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh; //��������� MeshCollider ��� ������������ � ����������.

        MeshRenderer MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshRenderer.materials = new Material[2] { mazeMat1, mazeMat2 }; //��������� MeshRenderer � ���������� ��� ����.
    }

    public void DisposeOldMaze() //������� ����� ������������, ��� ������ ��������. �� �������� ����� ��� ������� � ����� Generated � ��������� ��.
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject gameObjectin in objects)
        {
            Destroy(gameObject);
        }
    }

    /* ���� ��� ���������� � 0,0 � ���������� ������ ���������, ���� �� ������ �������� ������������. 
     * ����� ���� ��� ���������� ����� ��������� � �������� ��������� ������� ���������.*/
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
    //������ �� �� ����� ��� FindStartPosition(), ������ ������� � ������������ �������� � ���������� �������� ��������.
    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop top to bottom, right to left
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

    /* ���������� �������� �� ����� � ��������� �������. 
     * �� ��������� ���������� ��� �������, ������ ����� �������� ��������������� ��������, � ����� ��������� ������� TriggerEventRouter (�� ���������� ������). 
     * ���� ��������� ��������� ������� ��������� ������, ����� ���-�� ������ � �������� ��������.*/
    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject gameObject= GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        gameObject.name = "Start Trigger";
        gameObject.tag = "Generated";

        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = gameObject.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    /*���������� �������� �� ����� � �������� �������
     * �� ��������� ���������� ��� �������, ������ ����� �������� ��������������� ��������, � ����� ��������� ������� TriggerEventRouter (�� ���������� ������). 
     * ���� ��������� ��������� ������� ��������� ������, ����� ���-�� ������ � �������� ��������.*/
    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject gameObject= GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        gameObject.name = "Treasure";
        gameObject.tag = "Generated";

        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = gameObject.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }
}


