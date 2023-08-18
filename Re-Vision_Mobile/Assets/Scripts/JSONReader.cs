using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset textJSON;

    [System.Serializable]
    public class Quizz
    {
        public string name;
        public string theme;
        public int id;
    }


    [System.Serializable]
    public class Questions2
    {
        public Quizz[] questions;
    }


    public Questions2 myQuizz = new Questions2();
    
    // Start is called before the first frame update
    void Start()
    {
        myQuizz = JsonUtility.FromJson<Questions2>(textJSON.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
