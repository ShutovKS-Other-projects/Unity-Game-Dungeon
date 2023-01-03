using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class GameControler : MonoBehaviour
{
    [SerializeField] GameObject SkyBoxNight;
    [SerializeField] GameObject Player;

    private void Awake()
    {
        Instantiate(SkyBoxNight);
        Player = Instantiate(Player, new Vector3(1f, 0, 1f), new Quaternion(0, 0, 0, 0));
    }

    private void Update()
    {
        if (Input.GetAxis("Cancel") != 0)
        {
            Application.Quit();
        }
    }
}