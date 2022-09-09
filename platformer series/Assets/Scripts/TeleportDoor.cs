using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    public GameObject connectedDoor;
    public bool teleported = false;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (teleported && Input.GetAxisRaw("Vertical") < 1)
        {
            teleported = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")  && Input.GetAxisRaw("Vertical") == 1)
        {
            if (Input.GetAxisRaw("Vertical") == 1 && !teleported)
            {
                player.transform.position = connectedDoor.transform.position;
                connectedDoor.GetComponent<TeleportDoor>().teleported = true;
            }
        }
    }
}
