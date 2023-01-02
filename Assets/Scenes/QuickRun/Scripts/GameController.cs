using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class GameControler : MonoBehaviour
{
    [SerializeField] GameObject SkyBoxNight;
    [SerializeField] GameObject Player;
    public int widthMaze = 15;
    public int heightMaze = 15;

    //private bool GamePause = false;

    private void Awake()
    {
        Instantiate(SkyBoxNight);
        Player = Instantiate(Player, new Vector3(1f, 0, 1f), new Quaternion(0, 0, 0, 0));
    }

    //private void Update()
    //{
    //    if (Input.GetAxis("Cancel") != 0)
    //    {
    //        Application.Quit();
    //    }

    //    if (Input.GetAxis("Fire1") != 0)
    //    {
    //        PauseGame();
    //    }
    //}

    //void PauseGame()
    //{
    //if (!GamePause)
    //{
    //    GamePause = false;
    //    Time.timeScale = 0;
    //    return;
    //}
    //if (GamePause)
    //{
    //    GamePause = true;
    //    Time.timeScale = 1;
    //    return;
    //}
    //}
}