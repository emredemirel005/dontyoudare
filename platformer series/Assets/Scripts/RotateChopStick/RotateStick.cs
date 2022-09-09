using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class RotateStick : MonoBehaviour
{

    [Range(100,200)] public int speed;
    [SerializeField] private bool clockwise;


    private void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if (clockwise)
            RotateClockwise(speed);
        else
            RotateCtrClockwise(speed);
    }

    public void RotateClockwise(float speed)
    {
        transform.Rotate(new Vector3(0f, 0f, -(speed * Time.deltaTime)));
    }

    public void RotateCtrClockwise(float speed)
    {
        transform.Rotate(new Vector3(0f, 0f, (speed * Time.deltaTime)));
    }


}
