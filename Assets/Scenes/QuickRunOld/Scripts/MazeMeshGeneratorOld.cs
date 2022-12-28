using System.Collections.Generic;
using UnityEngine;

public class MazeMeshGeneratorOld
{

    public float width;
    public float height;

    public MazeMeshGeneratorOld()
    {
        width = 3.75f;
        height = 3.5f;
    }

    //метод для MazeConstructor для создания сетки
    public Mesh FromData(int[,] data)
    {
        Mesh maze = new Mesh();

        /* Возвращаясь к FromData(), он отвечает за списки вершин, UV и треугольников, которые создаются вверху. На этот раз есть два списка треугольников. 
         * Объект Unity Mesh может хранить в себе несколько подсетей с разным материалом на каждом меше, в итоге каждый список треугольников будет определяться как отдельная подсеть. 
         * Вы объявляете две подсетки, чтобы можно было назначать различные материалы, один для пола а другой для стен.*/
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        maze.subMeshCount = 2;
        List<int> floorTriangles = new List<int>();
        List<int> wallTriangles = new List<int>();

        int rMax = data.GetUpperBound(0);
        int cMax = data.GetUpperBound(1);
        float halfH = height * .5f;

        /* После этого вы перебираете 2D-массив и строите квадраты для пола, стенок лабиринта и потолка в каждой ячейке. 
         * В то время как каждая ячейка нуждается в полу и потолке, существуют проверки соседних ячеек, чтобы увидеть, какие стены необходимы. 
         * Обратите внимание, как AddQuad () вызывается неоднократно, но всегда будет с другой матрицей преобразования и с совершенно другими списками треугольников, 
         * которые используются для стенок и полов. Также обратите внимание, что ширина и высота используются для определения расположения квадратов и их размера.*/
        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (data[i, j] != 1)
                {
                    // этаж
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, 0, i * width),
                        Quaternion.LookRotation(Vector3.up),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);

                    // потолок
                    AddQuad(Matrix4x4.TRS(
                        new Vector3(j * width, height, i * width),
                        Quaternion.LookRotation(Vector3.down),
                        new Vector3(width, width, 1)
                    ), ref newVertices, ref newUVs, ref floorTriangles);


                    // стены по бокам рядом с заблокированными ячейками сетки

                    if (i - 1 < 0 || data[i - 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i - .5f) * width),
                            Quaternion.LookRotation(Vector3.forward),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j + 1 > cMax || data[i, j + 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j + .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.left),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (j - 1 < 0 || data[i, j - 1] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3((j - .5f) * width, halfH, i * width),
                            Quaternion.LookRotation(Vector3.right),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }

                    if (i + 1 > rMax || data[i + 1, j] == 1)
                    {
                        AddQuad(Matrix4x4.TRS(
                            new Vector3(j * width, halfH, (i + .5f) * width),
                            Quaternion.LookRotation(Vector3.back),
                            new Vector3(width, height, 1)
                        ), ref newVertices, ref newUVs, ref wallTriangles);
                    }
                }
            }
        }

        maze.vertices = newVertices.ToArray();
        maze.uv = newUVs.ToArray();

        maze.SetTriangles(floorTriangles.ToArray(), 0);
        maze.SetTriangles(wallTriangles.ToArray(), 1);

        /*RecalculateNormals() подготавливает сетку для освещения.*/
        maze.RecalculateNormals();

        return maze;
    }

    /* 3 параметра AddQuad () — это список вершин, UV и треугольников, к которому нужно добавить значения. 
     * Первая строка метода получает индекс для начала; По мере добавления четырех квадратов, их индекс будет расти.*/
    
    /* Важно понимать, что первый параметр AddQuad () является матрицей преобразования, и эта часть может сбить вас с толку. 
     * По существу, параметры положение / вращение / масштаб могут быть сохранены в матрице, а затем применены к вершинам. 
     * Это то, что делают вызовы MultiplyPoint3x4 (). Таким образом, можно использовать один и тот же код для создания четырехугольника, полов, стен и т. д. 
     * Вам нужно только изменить используемую матрицу преобразования.*/
    private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
        ref List<Vector2> newUVs, ref List<int> newTriangles)
    {
        int index = newVertices.Count;

        // углы перед преобразованием
        Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
        Vector3 vert2 = new Vector3(-.5f, .5f, 0);
        Vector3 vert3 = new Vector3(.5f, .5f, 0);
        Vector3 vert4 = new Vector3(.5f, -.5f, 0);

        newVertices.Add(matrix.MultiplyPoint3x4(vert1));
        newVertices.Add(matrix.MultiplyPoint3x4(vert2));
        newVertices.Add(matrix.MultiplyPoint3x4(vert3));
        newVertices.Add(matrix.MultiplyPoint3x4(vert4));

        newUVs.Add(new Vector2(1, 0));
        newUVs.Add(new Vector2(1, 1));
        newUVs.Add(new Vector2(0, 1));
        newUVs.Add(new Vector2(0, 0));

        newTriangles.Add(index + 2);
        newTriangles.Add(index + 1);
        newTriangles.Add(index);

        newTriangles.Add(index + 3);
        newTriangles.Add(index + 2);
        newTriangles.Add(index);
    }
}