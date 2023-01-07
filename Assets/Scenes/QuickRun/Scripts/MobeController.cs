using System;
using Unity.VisualScripting;
using UnityEngine;

public class MobeController : MonoBehaviour
{
    private MobeAnimatorController mobeAnimatorController;
    public MobeStatistics mobeStatistics;

    private GameObject Player;

    private void Start()
    {
        mobeAnimatorController = new MobeAnimatorController();
        mobeStatistics = new MobeStatistics();
        Player = GameObject.Find("Player(Clone)");
    }

    private void Update()
    {
        Ray ray = new(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        if (raycastHit.collider.gameObject == Player)
        {
            transform.LookAt(Player.transform.position);
            switch (MathF.Round(Vector3.Distance(transform.position, Player.transform.position), 1))
            {
                case > 0.75f:
                    {
                        transform.Translate(new Vector3(0f, 0f, 0.5f) * Time.deltaTime);
                        return;
                    }
                case < 0.25f:
                    {
                        transform.Translate(new Vector3(0f, 0f, -0.25f) * Time.deltaTime);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
        {
            Destroy(gameObject, 5f);
        }
        Debug.Log(other.gameObject);
    }
}
