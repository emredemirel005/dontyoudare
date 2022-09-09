using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public float maxSpeed;
    [Range(0,5)] public float duration = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        var rb = collision.attachedRigidbody;
        if (rb == null) return;

        var player = rb.GetComponent<Player>();
        if (player == null) return;
        player.StartCoroutine(PlayerModifier(player, duration));


    }

    IEnumerator PlayerModifier(Player player, float lifetime)
    {
        
        var initialSpeed = player.movementSpeed;
        player.movementSpeed = maxSpeed;
        yield return new WaitForSeconds(lifetime);
        player.movementSpeed = initialSpeed;
    }
}
