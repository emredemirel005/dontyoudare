using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<SceneController>();

            if (instance != null)
                return instance;

            Create();

            return instance;
        }
    }

    public static bool Transitioning
    {
        get { return Instance.m_Transitioning; }
    }

    protected static SceneController instance;

    public static SceneController Create()
    {
        GameObject sceneControllerGameObject = new GameObject("SceneController");
        instance = sceneControllerGameObject.AddComponent<SceneController>();

        return instance;
    }


    protected Scene m_CurrentZoneScene;
    protected Player m_PlayerInput;
    protected bool m_Transitioning;

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        m_PlayerInput = FindObjectOfType<Player>();


    }

}
