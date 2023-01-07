using System;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    public MobeStatistics mobeStatistics = new MobeStatistics();

    private GameObject Player;


    public bool attack = false;
    public float speed = 0f;
    public float xp = 100f;
    public float force = 7f;
    public bool dead = false;

    private void Start()
    {
        Player = GameObject.Find("Player(Clone)");
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        Ray ray = new(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        if (raycastHit.collider.gameObject == Player)
        {
            transform.LookAt(Player.transform.position);
            float distance = MathF.Round(Vector3.Distance(transform.position, Player.transform.position), 2);
            if (distance > 0.5f)
            {
                transform.Translate(new Vector3(0f, 0f, 0.5f) * Time.deltaTime);
                speed = 1f;
                return;
            }
            if (distance < 0.5f)
            {
                transform.Translate(new Vector3(0f, 0f, -0.25f) * Time.deltaTime);
                speed = -1f;
                return;
            }
            if (distance == 0.5f)
            {
                speed = 0f;
                return;
            }
        }

        if(xp <= 0)
        {
            dead = true;
            Destroy(gameObject, 5f);
            Debug.Log("Destroy");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
        {
            xp -= 200f;
        }
        Debug.Log(other.gameObject);
    }
}
