
//Ћогика генерации данных, в этом нам поможет MazeConstructor
using System.Collections.Generic;
using UnityEngine;

public class MazeDataGeneratorOld
{
    public float placementThreshold; //веро€тность наличи€ пустого места

    /* placementThreshold будет использоватьс€ алгоритмом генерации данных, чтобы определить, €вл€етс€ ли пространство пустым.*/
    public MazeDataGeneratorOld()
    {
        placementThreshold = .1f;       
    }

    public int[,] FromDimensions(int sizeRows, int sizeCols)
    {
        int[,] maze = new int[sizeRows, sizeCols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                /* ƒл€ определенной €чейки меша, код сперва проверит, находитс€ ли текуща€ €чейка снаружи меша 
                 * (то есть, существует ли какой-либо индекс внутри границ конкретного массива). 
                 * ≈сли это так, назначьте 1 дл€ этой стены.*/
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    maze[i, j] = 1;
                }

                /* «атем код должен проверить, дел€тс€ ли координаты равномерно на 2, дл€ дальнейшей работы с любой другой €чейкой. —уществует дополнительна€ 
                 * проверка по значению placeThreshold, описанному ранее, чтобы случайно не пропустить эту €чейку и продолжить итерацию по массиву.*/
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        /* “акже, код вбивает значение 1 как текущей €чейке, так и рандомной соседней €чейке.  од использует серию троичных операторов 
                         * дл€ случайного добавлени€ 0, 1 или -1 к индексу массива, таким образом он получит индекс напр€мую от соседней €чейки.*/
                        maze[i, j] = 1;

                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        maze[i + a, j + b] = 1;
                    }
                }
            }
        }

        return maze;
    }
}
