using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D parentCollider;
    private Animator anim;
    public bool closed;
    public string nextLevel;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        parentCollider = GetComponentInParent<BoxCollider2D>();
        closed = true;
    }

    private void Update()
    {
        if (parentCollider.enabled == true) return;

        else
        {
            if (closed)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    closed = false;
                    anim.Play("opening");
                    StartCoroutine(ChangeScene(1.5f));
                }

            }
            else if (!closed && Input.GetKeyDown(KeyCode.E))
            {
                closed = true;
                anim.Play("closing");
            }
        }

    }

    private IEnumerator ChangeScene(float sceneChangeTime) {
        yield return new WaitForSeconds(sceneChangeTime);
        SceneManager.LoadScene(nextLevel);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        anim.Play("opening");
    //        SceneManager.LoadScene(nextLevel);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //        anim.Play("closing");
    //}
}
