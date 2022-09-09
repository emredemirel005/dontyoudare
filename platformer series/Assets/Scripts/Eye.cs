using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public GameObject target;
    void Start()
    {
        if (target == null) return;
        target = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        EyeFollow();
    }

    void EyeFollow()
    {
        Vector3 targetPos = target.transform.position;
        Vector2 direction = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);

        transform.up = direction;
    }
}
