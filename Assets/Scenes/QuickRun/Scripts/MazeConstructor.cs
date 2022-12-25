//Конструктор лабиринта
using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    /*отображения окна отладки*/ 
    public bool showDebug;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    //свойства для хранения размеров и координат
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

    /* Свойство данных */
    public int[,] data
    {
        get; private set;
    }

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator(); //сохрание генератора сетки в новом поле

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
        //проверка размеров лабиринта, необходимы нечётные числа для коректной работы
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers are needed to generate a maze./Необходимы нечетные числа для генерации лабиринта.");
        }

        DisposeOldMaze(); //удаляет любой существующий, или старый лабиринт.

        data = dataGenerator.FromDimensions(sizeRows, sizeCols); //передаём размер сетки и сохраняем полученные данные

        FindStartPosition();
        FindGoalPosition();

        //хранят значения, используемые для создания этой сетки
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        /* DisplayMaze() вызывает MazeMeshGenerator.FromData(), вставляет этот вызов в середине создания нового GameObject, устанавливая тег Generated, 
         * добавляя MeshFilter и генерированный меш, добавляя MeshCollider для столкновения с лабиринтом, MeshRenderer и материалов для него.*/
        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }


    void OnGUI() // Отображения массива-лабиринта на экране
    {
        // Проверка включены ли отладочные дисплеи.
        if (!showDebug)
        {
            return;
        }

        /* Вы сможете инициализировать несколько событий локальных переменных: 
         * копию готового лабиринта, максимальную строку, столбец, а также строку для построения.*/
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        /* Два вложенных цикла перебирают строки и столбцы двумерного массива. Для каждой из строки или столбца данного массива, 
         * код будет проверяет сохраненное значение и добавляет либо «....», либо «==», это зависит от того, равняться ли значение нулю. 
         * Кроме того, код добавляет новую строку после итерации по всем столбцам в строке, так что каждая строка является новой строчкой.*/
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

        /* Распечатывает встроенную строчку. Использует совершенно новую систему графического интерфейса для дисплеев, видимых игроком, 
         * но более старая система используется для создания дисплеев быстрой отладки без особого труда.*/
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    private void DisplayMaze()
    {
        GameObject labirint = new GameObject();
        labirint.transform.position = Vector3.zero;
        labirint.name = "Procedural Maze";
        labirint.tag = "Generated";

        MeshFilter meshFilter = labirint.AddComponent<MeshFilter>();
        meshFilter.mesh = meshGenerator.FromData(data);

        MeshCollider meshCollider = labirint.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;

        MeshRenderer MeshRenderer = labirint.AddComponent<MeshRenderer>();
        MeshRenderer.materials = new Material[2] { mazeMat1, mazeMat2 };
    }

    public void DisposeOldMaze() //удаляет лабиринт (тег "Generated").
    {
        GameObject[] generated = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject gameObject in generated)
        {
            Destroy(gameObject);
        }
    }

    /* Начиная с мин значений перебирает данные лабиринта, пока не найдет валидное пространство. 
     * Эти координаты будут сохранены в качестве начальной позиции лабиринта. */
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
    /* Начиная с макс значений перебирает данные лабиринта, пока не найдет валидное пространство. 
     * Эти координаты будут сохранены в качестве конечной позиции лабиринта. */
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

    /* размещения объектов в начале */
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

    /* размещения объектов в конеце */
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


