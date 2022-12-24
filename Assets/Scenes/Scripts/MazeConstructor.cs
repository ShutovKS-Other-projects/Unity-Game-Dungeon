using UnityEngine;

//Конструктор лабиринта
public class MazeConstructor : MonoBehaviour
{
    /* Все эти поля доступны вам в Инспекторе. showDebug будет использоваться 
     * для отображения окна отладки, в то время как различные ссылки на материалы 
     * являются материалами для генерированных моделей. Между прочим, атрибут SerializeField 
     * отображает поле в Инспекторе, даже если переменная является закрытой для доступа к коду.*/
    public bool showDebug;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    private MazeDataGenerator dataGenerator; // переменная для хранения генератора данных
    private MazeMeshGenerator meshGenerator; // хранение генератора сетки

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

    /* Далее идет свойство данных. Декларации доступа (то есть объявление свойства как открытого, 
     * но затем назначение частного набора) делает его доступным только для чтения вне этого класса. 
     * Таким образом, данные лабиринта не могут быть изменены извне.*/
    public int[,] data
    {
        get; private set;
    }

    //3
    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator(); //сохрание генератора сетки в новом поле

        // по умолчанию используются стены, окружающие одну пустую ячейку
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
            Debug.LogError("Odd numbers work better for dungeon size./Нечетные числа лучше подходят для размера подземелья.");
        }

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols); //передаём размер сетки и сохраняем полученные данные

        FindStartPosition();
        FindGoalPosition();

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        /* DisplayMaze () не только вызывает MazeMeshGenerator.FromData (), но и вставляет этот вызов в середине создания нового GameObject, 
         * устанавливая тег Generated, добавляя MeshFilter и генерированный меш, добавляя MeshCollider для столкновения с лабиринтом, 
         * ну и конечно добавление MeshRenderer и материалов для него.*/
        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }


    void OnGUI() // Для отображения данных лабиринта и проверки, как они выглядят
    {
        // Этот код проверяет, включены ли отладочные дисплеи.
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

        /* Два вложенных цикла перебирают строки и столбцы двумерного массива. 
         * Для каждой из строки или столбца данного массива, код будет проверяет сохраненное значение 
         * и добавляет либо «….», либо «==», это зависит от того, равняться ли значение нулю. 
         * Кроме того, код добавляет новую строку после итерации по всем столбцам в строке, 
         * так что каждая строка является новой строчкой.*/
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

        /* Наконец то добрались до Label(), который распечатывает встроенную строчку. 
         * Этот лейбел использует совершенно новую систему графического интерфейса 
         * для дисплеев, видимых игроком, но более старая система используется 
         * для создания дисплеев быстрой отладки без особого труда.*/
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] { mazeMat1, mazeMat2 };
    }

    public void DisposeOldMaze() //удаляет любой существующий, или старый лабиринт. Он попросту найдёт все объекты с тегом Generated и уничтожит их.
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    /* Этот код начинается с 0,0 и перебирает данные лабиринта, пока не найдет валидное пространство. 
     * После чего эти координаты будут сохранены в качестве начальной позиции лабиринта.*/
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
    //делает то же самое что FindStartPosition(), только начиная с максимальных значений и заканчивая обратным отсчетом.
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

    /*размещения объектов на сцене в начальной позиции. 
     * Их коллайдер установлен как триггер, сперва будет применен соответствующий материал, а затем добавится событие TriggerEventRouter (из стартового пакета). 
     * Этот компонент принимает функцию обратного вызова, когда что-то входит в значение триггера.*/
    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    /*размещения объектов на сцене в конечной позиции
     * Их коллайдер установлен как триггер, сперва будет применен соответствующий материал, а затем добавится событие TriggerEventRouter (из стартового пакета). 
     * Этот компонент принимает функцию обратного вызова, когда что-то входит в значение триггера.*/
    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        go.name = "Treasure";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }
}


