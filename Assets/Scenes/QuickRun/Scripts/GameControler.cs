//Управление игрой
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))] // Атрибут RequireComponent гарантирует, что компонент MazeConstructor также будет добавлен при добавлении этого сценария в GameObject.

public class GameController : MonoBehaviour
{
	[SerializeField] private FpsMovement player;

	private MazeConstructor generator; //Закрытая переменная, в которой хранится ссылка, возвращаемая функцией GetComponent().

	/* Переменная для определения найдена ли цель лабиринта.*/
	private bool goalReached;

	/* MazeConstructor активируется так же, как и раньше, но сейчас параметр Start() использует новые методы, которые делают больше, чем просто вызов GenerateNewMaze().*/
	void Start()
	{
		generator = GetComponent<MazeConstructor>(); //Закрытая переменная, в которой хранится ссылка, возвращаемая функцией GetComponent().
		StartNewGame();
	}

	/* StartNewGame()используется для запуска всей игры с самого начала, в отличие от переключения между уровнями в игре. 
	 * Таймер устанавливается на начальные значения, счет сбрасывается, а затем создается лабиринт.*/
	private void StartNewGame()
	{
		StartNewMaze();
	}

	/* StartNewMaze()отвечает за переход на следующий уровень без начала всей игры. Кроме создания нового лабиринта, этот метод отправит игрока в начало уровня, сбросит цель. */
	private void StartNewMaze()
	{
		generator.GenerateNewMaze(23, 25, OnStartTrigger, OnGoalTrigger);

		float x = generator.startCol * generator.hallWidth;
		float y = 1;
		float z = generator.startRow * generator.hallWidth;

        player = Instantiate(player);
		player.name = "Player";
        player.transform.position = new Vector3(x, y, z);

		goalReached = false;
		player.enabled = true;
	}

	void Update()
	{
		if (!player.enabled)
		{
			return;
        }
	}

	/* OnGoalTrigger() это обратные вызовы, передаваемые в TriggerEventRouterin MazeConstructor.
	 * Событие OnGoalTrigger() этот триггер проверяет, что цель была найдена.*/
	private void OnGoalTrigger(GameObject trigger, GameObject other)
	{
		Debug.Log("Goal!");
		goalReached = true;

		Destroy(trigger);
    }

	/* OnStartTrigger() это обратные вызовы, передаваемые в TriggerEventRouterin MazeConstructor.
	 * Другое событие OnStartTrigger () проверяет, была ли цель достигнута, а затем выбрасывает игрока и запускает новый лабиринт. */
	private void OnStartTrigger(GameObject trigger, GameObject other)
	{
		if (goalReached)
		{
			Debug.Log("Finish!");
			player.enabled = false;

			Invoke("StartNewMaze", 4);
		}
	}
}