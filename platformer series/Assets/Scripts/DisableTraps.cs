using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTraps : MonoBehaviour
{
    public GameObject secretPath;
    public GameObject enableMovingPlatform;

    private void Start()
    {
        enableMovingPlatform.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            secretPath.SetActive(false);
            enableMovingPlatform.SetActive(true);
        }
    }
}
