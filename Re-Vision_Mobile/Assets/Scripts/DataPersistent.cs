using ReVision.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistent : MonoBehaviour
{
    public static DataPersistent Instance;

    public string user;

    public Quizz quizzForGame;
    public Player playerForGame;

    private void Awake()
    {
        //singleton
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
