using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    
    private SpringJoint2D spring;
    public GameObject eye;

    // Start is called before the first frame update
    void Start()
    {
        if (eye == null) return;
        eye.SetActive(false);
        spring = GetComponent<SpringJoint2D>();
        GameObject backpack = GameObject.FindWithTag("Backpack");
        spring.connectedBody = backpack.GetComponent<Rigidbody2D>();
        spring.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !spring.enabled )
        {
            eye.SetActive(true);
            spring.enabled = true;
        }
        
        
    }
}
