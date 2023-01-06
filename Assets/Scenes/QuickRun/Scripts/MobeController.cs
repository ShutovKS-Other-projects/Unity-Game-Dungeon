using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    public MobeStatistics mobeStatistics;

    private GameObject Player;
    public bool attack = false;

    public float speed = 0f;
    public float xp = 100f;
    public float force = 5f;

    private void Start()
    {
        mobeStatistics = new MobeStatistics();
        Player = GameObject.Find("Player(Clone)");
    }

    private void Update()
    {
        Ray ray = new(new Vector3(transform.position.x, transform.position.y+0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        speed = 0f;
        if (raycastHit.collider.gameObject == Player)
        {
            float distance = Vector3.Distance(transform.position, Player.transform.position);

            if (distance > 5f)
            {
                transform.LookAt(Player.transform.position);
                transform.Translate(new Vector3(0f, 0f, 0.75f) * Time.deltaTime);
                speed = 1f;
                return;
            }
            if (distance < 0.75f)
            {
                transform.LookAt(Player.transform.position);
                transform.Translate(new Vector3(0f, 0f, -0.25f) * Time.deltaTime);
                speed = -1f;
                return;
            }
        }

        if(xp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other == GameObject.Find("mixamorig:RightHand") || other == GameObject.Find("mixamorig:LeftHand"))
            xp -= 200f /*playerStatistics.force*/;
    }
}
