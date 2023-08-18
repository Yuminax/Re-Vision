using System.Collections;
using System.Collections.Generic;
using TMPro;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEngine.UI;
using ReVision.MyDebug;

namespace ReVision.Game
{
    /// <summary>
    /// Class used to control the Question UI of the game
    /// </summary>
    public class QuestionController : MonoBehaviour
    {
        /// <summary>
        /// Number of answer buttons per questions.
        /// </summary>
        /// <remarks>Used to had the reference on the button answer in the UI</remarks>
        private const int NB_BTN_ANSWERS = 4;

        /// <summary>
        /// Number of attempt done on the question
        /// </summary>
        public int NbAttempt { get; set; }


        /// <summary>
        /// Allow the timer to count (if <see langword="false"/>, it's like a pause of the timer)
        /// </summary>
        private bool TimerEnable { get; set; } = false;
        /// <summary>
        /// The time passed on the question
        /// </summary>
        public float ElapsedTime { get; private set; }

        private Question currentQuestion;

        /// <summary>
        /// The question attached to the UI. Set a new question will update the UI in consequence
        /// </summary>
        public Question CurrentQuestion
        {
            get { return currentQuestion; }
            set
            {
                currentQuestion = value;
                SetupNewQuestion(value);
            }
        }

        /// <summary>
        /// Reference on the level component
        /// </summary>
        /// <remarks>This reference is getting through the componenent of the player <see cref="GameObject.Find(_scripts)"/></remarks>
        protected Game game;

        /// <summary>
        /// Reference on the text UI to display the question sentence
        /// </summary>
        private TextMeshProUGUI txtSentenceUI;
        
        /// <summary>
        /// List of buttons used to perform his answer choice
        /// </summary>
        private List<Button> btnAnswersUi = new List<Button>();

        /// <summary>
        /// List of text UI linked to the button. Used to display the answer sentence
        /// </summary>
        private List<TextMeshProUGUI> txtAnswersUI = new List<TextMeshProUGUI>();

        /// <summary>
        /// Image UI linked to the question
        /// </summary>
        [SerializeField]
        private Image image;


        private void Awake()
        {
            // Init
            game = GameObject.Find("_scripts").GetComponent<Game>();

            txtSentenceUI = GameObject.Find("TxtSentence").GetComponent<TextMeshProUGUI>();

            for (int i = 1; i <= NB_BTN_ANSWERS; i++)
            {
                btnAnswersUi.Add(GameObject.Find("BtnAnswer" + i).GetComponent<Button>());
                txtAnswersUI.Add(GameObject.Find("BtnAnswer" + i + "/TxtAnswer").GetComponent<TextMeshProUGUI>());
            }

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (TimerEnable)
            {
                ElapsedTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Restart the timer.
        /// </summary>
        /// <remarks>
        /// Will set <see cref="TimerEnable"/> to <see langword="true"/>
        /// and the <see cref="ElapsedTime"/> to 0
        /// </remarks>
        public void StartTime()
        {
            TimerEnable = true;
            ElapsedTime = 0;
        }

        /// <summary>
        /// Setup the UI according the question. The answer order will have a random position
        /// </summary>
        /// <param name="question">Question data to set</param>
        /// <remarks><see cref="Question.Answers"/> size must be 4. else an exception will be throw</remarks>
        private void SetupNewQuestion(Question question)
        {
            Assert.IsTrue(question.Answers.Count == NB_BTN_ANSWERS, "The question must have " + NB_BTN_ANSWERS + " answers");

            NbAttempt = 0;

            txtSentenceUI.text = question.Sentence;

            var answers = question.Answers.OrderBy(a => Random.Range(0, NB_BTN_ANSWERS)).ToList();
            for (int i = 0; i < NB_BTN_ANSWERS; i++)
            {
                txtAnswersUI[i].text = answers[i].Sentence;
                
                // Update button actions to have the correct answer information
                btnAnswersUi[i].onClick.RemoveAllListeners();
                int index = i; // Why this variable ? : Need to store the value to beeing able to pass it to the lambda expression,
                               // else it will always be the last value for each button (like a static)
                btnAnswersUi[i].onClick.AddListener(() => OnAnswerClick(answers[index]));
                
            }

            image.sprite = Utils.LoadFromPeristantOrResource(question.ImagePath, "");
            image.enabled = image.sprite != null;
        }


        /// <summary>
        /// Action when a answer button is pressed. If it's the correct answer, it retrive the information to <see cref="Game.OnEndQuestion"/>
        /// </summary>
        /// <param name="answer"></param>
        private void OnAnswerClick(Answer answer)
        {
            NbAttempt++;
            
            DebugMode.Log("Click on : " + answer.Sentence);
            if (answer.IsCorrect)
            {
                TimerEnable = false;
                DebugMode.Log("Correct");
                game.OnEndQuestion();
            }
            else
            {
                DebugMode.Log("Wrong");
            }
        }
    }
}