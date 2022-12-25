//���������� �����
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))] // ������� RequireComponent �����������, ��� ��������� MazeConstructor ����� ����� �������� ��� ���������� ����� �������� � GameObject.

public class GameController : MonoBehaviour
{
	[SerializeField] private FpsMovement player;

	private MazeConstructor generator; //�������� ����������, � ������� �������� ������, ������������ �������� GetComponent().

	/* ���������� ��� ����������� ������� �� ���� ���������.*/
	private bool goalReached;

	/* MazeConstructor ������������ ��� ��, ��� � ������, �� ������ �������� Start() ���������� ����� ������, ������� ������ ������, ��� ������ ����� GenerateNewMaze().*/
	void Start()
	{
		generator = GetComponent<MazeConstructor>(); //�������� ����������, � ������� �������� ������, ������������ �������� GetComponent().
		StartNewGame();
	}

	/* StartNewGame()������������ ��� ������� ���� ���� � ������ ������, � ������� �� ������������ ����� �������� � ����. 
	 * ������ ��������������� �� ��������� ��������, ���� ������������, � ����� ��������� ��������.*/
	private void StartNewGame()
	{
		StartNewMaze();
	}

	/* StartNewMaze()�������� �� ������� �� ��������� ������� ��� ������ ���� ����. ����� �������� ������ ���������, ���� ����� �������� ������ � ������ ������, ������� ����. */
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

	/* OnGoalTrigger() ��� �������� ������, ������������ � TriggerEventRouterin MazeConstructor.
	 * ������� OnGoalTrigger() ���� ������� ���������, ��� ���� ���� �������.*/
	private void OnGoalTrigger(GameObject trigger, GameObject other)
	{
		Debug.Log("Goal!");
		goalReached = true;

		Destroy(trigger);
    }

	/* OnStartTrigger() ��� �������� ������, ������������ � TriggerEventRouterin MazeConstructor.
	 * ������ ������� OnStartTrigger () ���������, ���� �� ���� ����������, � ����� ����������� ������ � ��������� ����� ��������. */
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