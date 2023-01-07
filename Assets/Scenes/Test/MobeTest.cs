using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobeTest : MonoBehaviour
{
    private GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player");
    }

    private void Update()
    {
        Ray ray = new(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Player.transform.position - transform.position);
        Physics.Raycast(ray, out RaycastHit raycastHit, 7.5f);
        if (raycastHit.collider.gameObject == Player)
        {
            transform.LookAt(Player.transform.position);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "mixamorig:RightHand" || other.gameObject.name == "mixamorig:LeftHand")
        {
            Destroy(gameObject);
        }
        Debug.Log(other.gameObject);
    }
}
