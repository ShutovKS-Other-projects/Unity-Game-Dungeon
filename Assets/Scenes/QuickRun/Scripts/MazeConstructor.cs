using UnityEngine;

//Конструктор лабиринта
public class MazeConstructor : MonoBehaviour
{
    /* showDebug будет использоваться для отображения окна отладки.*/ 
    public bool showDebug;

    [SerializeField] private Material mazeMat1;    //Пол и потолок лабиринта
    [SerializeField] private Material mazeMat2;    //Стены лабиринта
    [SerializeField] private Material startMat;    //Блок в начале
    [SerializeField] private Material treasureMat; //Блок цели

    private MazeDataGenerator dataGenerator; // переменная для хранения генератора данных
    private MazeMeshGenerator meshGenerator; // хранение генератора сетки
    static public GameObject Player; //Игрок (не используется)

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

    /* Свойство данных. Декларации доступа (то есть объявление свойства как открытого, 
     * но затем назначение частного набора) делает его доступным только для чтения вне этого класса. 
     * Таким образом, данные лабиринта не могут быть изменены извне.*/
    public int[,] data
    {
        get; private set;
    }

    /* Инициализация данных с массивом 3 на 3, который окружает ноль. 1 значит что это «стена», в то время как 0 означает «пусто», 
     * поэтому эта сетка по умолчанию является просто замурованной комнатой.*/
    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator(); //сохрание генератора сетки в новом поле
        Player = GameObject.Find("Player"); //Создаём ссылку на префаб игрока (не используется)

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
        //проверка размеров лабиринта, необходимы нечётные числа
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

    /* DisplayMaze() вызывает MazeMeshGenerator.FromData() (переписать описание)*/
    private void DisplayMaze()
    {
        GameObject gameObject= new GameObject(); //создаём лабиринт
        gameObject.transform.position = Vector3.zero; //перемещаем лабиринт на (0,0,0) позицию
        gameObject.name = "Procedural Maze"; //задаём имя
        gameObject.tag = "Generated"; //добавляем тег

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = meshGenerator.FromData(data); //Добавляем MeshFilter и генерированный меш.

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh; //Добавляем MeshCollider для столкновения с лабиринтом.

        MeshRenderer MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshRenderer.materials = new Material[2] { mazeMat1, mazeMat2 }; //Добавляем MeshRenderer и материалов для него.
    }

    public void DisposeOldMaze() //удаляет любой существующий, или старый лабиринт. Он попросту найдёт все объекты с тегом Generated и уничтожит их.
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject gameObjectin in objects)
        {
            Destroy(gameObject);
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

    /* размещения объектов на сцене в начальной позиции. 
     * Их коллайдер установлен как триггер, сперва будет применен соответствующий материал, а затем добавится событие TriggerEventRouter (из стартового пакета). 
     * Этот компонент принимает функцию обратного вызова, когда что-то входит в значение триггера.*/
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

    /*размещения объектов на сцене в конечной позиции
     * Их коллайдер установлен как триггер, сперва будет применен соответствующий материал, а затем добавится событие TriggerEventRouter (из стартового пакета). 
     * Этот компонент принимает функцию обратного вызова, когда что-то входит в значение триггера.*/
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


