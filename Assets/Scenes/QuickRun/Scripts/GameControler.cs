using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = System.Random;


public class GameControler : MonoBehaviour
{
    [SerializeField] GameObject SkyBoxNight;
    [SerializeField] GameObject Player;
    public int widthMaze = 10;
    public int heightMaze = 10;

    private void Awake()
    {
        Instantiate(SkyBoxNight);
    }

    private void Start()
    {
        Player = Instantiate(Player, new Vector3(1f, 0, 1f), new Quaternion(0, 0, 0, 0));
    }
}