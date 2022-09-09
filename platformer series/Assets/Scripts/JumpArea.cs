using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArea : MonoBehaviour
{
    public float bounceForce = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rb = collision.attachedRigidbody;
        if (rb == null) return;

        var player = rb.GetComponent<PlayerController>();
        if (player == null) return;
        AddVelocity(rb);

        //collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        
    }

    void AddVelocity(Rigidbody2D rb) => rb.velocity = new Vector2(rb.velocity.x, bounceForce);

    
}
