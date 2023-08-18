using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONControlle : MonoBehaviour
{

    public string jsonURL ;

    // Start is called before the first frame update
    void Start()
    {
        //processJsonData(jsonURL);
        //StartCoroutine(getData());
    }


    IEnumerator getData()
    {
        Debug.Log("Processing Data, wait");

        WWW www = new WWW(jsonURL);
        yield return www;
        if(www.error == null)
        {
            processJsonData(www.text);
        }
        else
        {
            Debug.Log("Something went wrong");
        }
    }

    public void setPath(string path)
    {
        jsonURL = path;
        StartCoroutine(getData());
    }

public void processJsonData(string url)
    {
        JSONData jsnData = JsonUtility.FromJson<JSONData>(url);
        Debug.Log(jsnData.name);

        foreach(QuestionsList x in jsnData.questions)
        {
            Debug.Log("question: " + x.question);
            Debug.Log("answer1: " + x.answers1);
            Debug.Log("image: " + x.image);
            Debug.Log("wich one correcte: "+ x.correct);
        }

        foreach (RewardsList x in jsnData.rewards)
        {
            Debug.Log("type: " + x.type);
            Debug.Log("criteria: " + x.criteria);
        }
    }
}
