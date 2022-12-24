using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))] // јтрибут RequireComponent гарантирует, что компонент MazeConstructor также будет добавлен при добавлении этого сценари€ в GameObject.

//”правление игрой
public class GameController : MonoBehaviour
{
    /* —начала вы внедрили сериализованные пол€ дл€ каждого объекта в сцене.*/
    [SerializeField] private FpsMovement player;
    [SerializeField] private Text timeLabel;
    [SerializeField] private Text scoreLabel;

    private MazeConstructor generator;

    /* Ќесколько личных переменных были добавлены, чтобы отследить таймер игры, счет, и узнать была ли найдена цель лабиринта.*/
    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;

    private int score;
    private bool goalReached;

    /* MazeConstructor активируетс€ так же, как и раньше, но сейчас параметр Start() использует новые методы, которые делают больше, чем просто вызов GenerateNewMaze().*/
    void Start()
    {
        generator = GetComponent<MazeConstructor>(); //«акрыта€ переменна€, в которой хранитс€ ссылка, возвращаема€ функцией GetComponent().
        StartNewGame();
    }

    /* StartNewGame()используетс€ дл€ запуска всей игры с самого начала, в отличие от переключени€ между уровн€ми в игре. 
     * “аймер устанавливаетс€ на начальные значени€, счет сбрасываетс€, а затем создаетс€ лабиринт.*/
    private void StartNewGame()
    {
        timeLimit = 80;
        reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

    /* StartNewMaze()отвечает за переход на следующий уровень без начала всей игры.  роме создани€ нового лабиринта, этот метод отправит игрока в начало уровн€, сбросит цель и сократит врем€. */
    private void StartNewMaze()
    {
        generator.GenerateNewMaze(13, 15, OnStartTrigger, OnGoalTrigger);

        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = false;
        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    /* Update()смотрит за активностью игрока, после чего обновл€ет оставшеес€ минуты до завершени€ уровн€.  огда врем€ истекло, игрока выкинет и запуститс€ нова€ игра. */
    void Update()
    {
        if (!player.enabled)
        {
            return;
        }

        int timeUsed = (int)(DateTime.Now - startTime).TotalSeconds;
        int timeLeft = timeLimit - timeUsed;

        if (timeLeft > 0)
        {
            timeLabel.text = timeLeft.ToString();
        }
        else
        {
            timeLabel.text = "TIME UP";
            player.enabled = false;

            Invoke("StartNewGame", 4);
        }
    }

    /* OnGoalTrigger()и OnStartTrigger() это обратные вызовы, передаваемые в TriggerEventRouterin MazeConstructor. 
     * —обытие OnGoalTrigger() этот триггер провер€ет, что цель была найдена, а затем увеличивает счет.
     * ƒругое событие OnStartTrigger () провер€ет, была ли цель достигнута, а затем выбрасывает игрока и запускает новый лабиринт. */
    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();

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