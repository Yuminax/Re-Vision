using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manager of a UI question result stats.
/// It's used to display the stats of a question at the end of the game.
/// </summary>
public class QuestionStatsUIManager : MonoBehaviour
{
    /// <summary>
    /// Reference on the text UI for the sentence
    /// </summary>
    [SerializeField]
    TextMeshProUGUI sentence;

    /// <summary>
    /// reference on the text UI for the number of wrong attempt
    /// </summary>
    [SerializeField]
    TextMeshProUGUI nbWrong;

    /// <summary>
    /// reference on the text UI for the minimal passed time on the question
    /// </summary>
    [SerializeField]
    TextMeshProUGUI minTime;


    private void Awake()
    {

    }
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Apply the specified values to the UI
    /// </summary>
    /// <param name="question">Question sentence</param>
    /// <param name="nbWrong">Number of fail to answer</param>
    /// <param name="time">Minimal time passed on the question</param>
    public void SetData(string question, int nbWrong, float time)
    {
        SetQuestionVal(question);
        SetNbWrongVal(nbWrong);
        SetMinTimeVal(time);
    }
    
    /// <summary>
    /// Apply the specified value to the UI on "question sentence"
    /// </summary>
    /// <param name="question">Question sentence</param>
    public void SetQuestionVal(string question)
    {
        sentence.text = question;
    }

    /// <summary>
    /// Apply the specified value to the UI on "Number of wrong answer"
    /// </summary>
    /// <param name="value">Number of fail to answer</param>
    public void SetNbWrongVal(int value)
    {
        nbWrong.text = value.ToString();
    }

    /// <summary>
    /// Apply the specified value to the UI on "Minimal time passed on the question"
    /// </summary>
    /// <param name="value">Minimal time passed on the question</param>
    public void SetMinTimeVal(float value)
    {
        minTime.text = value.ToString("F1");
    }        
}
