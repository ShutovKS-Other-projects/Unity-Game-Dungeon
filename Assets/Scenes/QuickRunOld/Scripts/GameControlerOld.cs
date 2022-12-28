//”правление игрой
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructorOld))]

public class GameControllerOld : MonoBehaviour
{
	[SerializeField] private FpsMovement player;

	private MazeConstructorOld generator;

	private bool goalReached;

	void Start()
	{
		generator = GetComponent<MazeConstructorOld>();
		StartNewGame();
	}

	private void StartNewGame()
	{
		StartNewMaze();
	}

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

	private void OnGoalTrigger(GameObject trigger, GameObject other)
	{
		Debug.Log("Goal!");
		goalReached = true;

		Destroy(trigger);
    }

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