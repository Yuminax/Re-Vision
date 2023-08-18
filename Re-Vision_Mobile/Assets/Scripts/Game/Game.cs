using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ReVision.Data;
using ReVision.Enum;
using ReVision.Game;
using System.Linq;
using ReVision.MyDebug;
using System;
using Random = UnityEngine.Random;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

namespace ReVision.Game
{
    /// <summary>
    /// Main script for the game logic.
    /// It is used for the scene "Scene".
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class Game : MonoBehaviour
    {
        #region Scoring constants
        /// <summary> Score earned if a question is correctly answered at the 1st attempt </summary>
        private const int SCORE_FIRST_ATTEMPT = 1;
        /// <summary> Score earned if a question is correctly answered at the 2nd attempt </summary>
        private const int SCORE_SECOND_ATTEMPT = -1;
        /// <summary> Score earned if a question is correctly answered at the 3th attempt </summary>
        private const int SCORE_THIRD_ATTEMPT = -2;
        /// <summary> Score earned if a question is correctly answered at the 4th or more attempt </summary>
        private const int SCORE_FOURTH_OR_MORE_ATTEMPT = -4;
        /// <summary> Score earned if a question is correctly answered within the <see cref="SECONDS_QUICKLY_RESPONSE"/> seconds </summary>
        private const int SCORE_QUICKLY_RESPONSE = 3;
        /// <summary> Maximal time allowed to earn the <see cref="SCORE_QUICKLY_RESPONSE"/> score </summary>
        private const int SECONDS_QUICKLY_RESPONSE = 5;
        #endregion Scoring constants

        #region Terrain configuration constants
        /// <summary> Distance between each X coordinate points of the terrain </summary>
        private const int DIST_BETWEEN_X_POINT = 20;
        /// <summary> Range (from -Y to +Y) where the Y coordinate point of the terrain can be </summary>
        private const int PLUS_MINUS_Y_POINT = 5;
        /// <summary> Number of coordinate that will be load when terrain is initialized (not include finish flag + question checkpoints) </summary>
        /// <remarks> It's like the number of points that will add after the finish flag </remarks>
        private const int NB_DEFAULT_POINT = 5;
        /// <summary> Location of the first point used to generate the terrain (should be outside the player camera) </summary>
        private const int START_X_POINT = -100;
        /// <summary> The depth used to simulate "grass + dirt" of the terrain (should be below the player camera) </summary>
        private const int DEPTH_TERRAIN_POINT = 20;
        #endregion Terrain constants

        #region Question configuration constants
        /// <summary> Number of question that will be take for the run </summary>
        private const int NB_BASE_QUESTION = 10;
        /// <summary>
        /// The question will recome one more time after each of theses values
        /// </summary>
        /// <example>For example, if the value is {1, 3} the question will recome<br>
        /// - 1 time if 1 wrong answer
        /// - 2 times if 3 wrong answer
        /// </example>
        private readonly int[] READD_ONE_QUESTION_AFTER_WRONG = { 1, 3 };
        #endregion Question configuration constants

        #region Default ZIP data constants
        /// <summary> </summary>
        private const string DEFAULT_ZIP_DDL_URL = "https://drive.google.com/uc?export=download&id=18g-Beoo52G1K6ERbacv_vlj0bytdiOQ2";
        /// <summary> </summary>
        private const string DEFAULT_UNZIP_PATH = "/defaults_example/";
        /// <summary> The path of the data when download defaults image from <see cref="DEFAULT_ZIP_DDL_URL"/> </summary>
        /// <remarks> Order is important, it is used in <see cref="LoadPlayer"/> and <see cref="LoadQuizz"/></remarks>
        private readonly string[] DEFAULT_RELATIVE_IMAGE_PATHS = {
            DEFAULT_UNZIP_PATH + "Skin/body.png", // Player body skin
            DEFAULT_UNZIP_PATH + "Skin/head.png", // Player head skin
            DEFAULT_UNZIP_PATH + "Skin/flag.png", // Player flag skin
            DEFAULT_UNZIP_PATH + "Question/swiss.png", // Question swiss capital
            DEFAULT_UNZIP_PATH + "Question/ReVisionLogo.png", // Question App name
            DEFAULT_UNZIP_PATH + "Question/exemple.png", // Generated
        };
        #endregion Default ZIP data constats

        /// <summary>
        /// Used to display some debug informations
        /// </summary>
        private DebugMode debug;

        /// <summary>
        /// Current quizz used for the run
        /// </summary>
        private Quizz quizz;

        /// <summary>
        /// The questions that will be displayed at each "question checkpoints" (real order)
        /// </summary>
        /// <exemple>Index 0 = 1st question, index 1 = 2nd question, ...</exemple>
        /// <remarks>A question can be present multiple times; that just means the question will be displayed multiple times</remarks>
        private List<Question> questions = new List<Question>();

        /// <summary>
        /// The index of the current <see cref="questions"/> that will be shown to the player
        /// </summary>
        private int currentQuestionIndex = 0;

        /// <summary>
        /// Group of questions with their statistics
        /// </summary>
        private Dictionary<Question, QuestionStats> questionsStats = new Dictionary<Question, QuestionStats>();

        /// <summary>
        /// Current game state
        /// </summary>
        private GameState state;
        /// <summary>
        /// Memory of the previous state, to detect when gamestate change
        /// </summary>
        private GameState previousState;

        /// <summary>
        /// List of coordinates used to draw the terrain
        /// </summary>
        private List<Vector2> terrainPoint = new List<Vector2>();


        #region UI components
        /// <summary> UI that contains the "Question state" </summary>
        private GameObject question;
        /// <summary> UI that contains the "End game state" </summary>
        private GameObject gameover;
        /// <summary> HUD when "Moving state" </summary>
        private GameObject progressHUD;
        /// <summary> Pause button container </summary>
        private GameObject pauseButton;
        /// <summary>
        /// Reference to the pause menu component
        /// </summary>
        /// <remarks>This reference is getting thourgh the componenent of the player <see cref="GameObject.Find(PauseMenuCanvas)"/>.</remarks>
        private PauseMenu pauseMenu;
        /// <summary> Text UI of the HUD, to display the progression (question N / M) </summary>
        private TextMeshProUGUI progressText;
        #endregion UI components

        #region Attached scripts
        /// <summary> Script attached to the player </summary>
        private PlayerController player;
        /// <summary> Script attached to the question UI </summary>
        private QuestionController questionController;
        /// <summary> Script attached to the terrain </summary>
        private TerrainController terrainController;
        #endregion Attached scripts

        // Start is called before the first frame update
        void Start()
        {
            // Download defaults images to prove the download system works
            if (!Directory.Exists(Utils.MergeWithPersistantPath(DEFAULT_UNZIP_PATH)))
            {
                Debug.Log("Downloading defaults images");

                StartCoroutine(Utils.DownloadZipContent(DEFAULT_ZIP_DDL_URL, DEFAULT_UNZIP_PATH,
                    () => { Debug.Log("Success download defaults images"); },
                    () => { Debug.Log("Failed download defaults images"); })
                );
            }

            // Use start, to be sure the player awake is firstly done
            // https://gamedevbeginner.com/start-vs-awake-in-unity/
            // The player GameObject should have already been initialized with his script

            // Retrive the gameobjects and components to the references variables
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            question = GameObject.Find("Question");
            pauseButton = GameObject.Find("PauseButton");
            progressHUD = GameObject.Find("ProgressHUD");
            progressText = progressHUD.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            questionController = question.GetComponent<QuestionController>();
            gameover = GameObject.Find("FinalStats");
            GameObject dirt = GameObject.Find("Terrain/Dirt");
            terrainController = dirt.GetComponent<TerrainController>();
            pauseMenu = GameObject.Find("PauseMenuCanvas").GetComponent<PauseMenu>();

            debug = new DebugMode(GameObject.Find("Debug"));
            debug.Enabled = true;

            // Init
            LoadScene();

            // Init state done, next state is "Move"
            state = GameState.Move;
        }

        // Update is called once per frame
        void Update()
        {
            // Perform the actions according the current Game State

            if (state == previousState)
                return;

            previousState = state;

            switch (state)
            {
                case GameState.Move:
                    DebugMode.Log("Move state");
                    player.IsMoving = true;
                    break;

                case GameState.Question:
                    DebugMode.Log("Question state");
                    player.IsMoving = false;
                    LoadCurrentQuestion(questions[currentQuestionIndex]);
                    DisplayQuestion(true);
                    questionController.StartTime();
                    break;

                case GameState.PostQuestion:
                    DebugMode.Log("PostQuestion state");
                    DisplayQuestion(false);
                    PostQuestion();
                    state = GameState.Move;
                    break;

                case GameState.End:
                    DebugMode.Log("End state");
                    player.IsMoving = false;
                    GameOver();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Action called by the <see cref="FlagColliderController"/>, when a checkpoint is reached.
        /// It only change the game state according the type of checkpoint
        /// </summary>
        /// <param name="type">Type of reached checkpoint</param>
        public void OnHitFlag(Checkpoint type)
        {
            switch (type)
            {
                case Checkpoint.Start:
                    DebugMode.Log("Start checkpoint");
                    break;
                case Checkpoint.Question:
                    DebugMode.Log("Question checkpoint");
                    state = GameState.Question;
                    break;
                case Checkpoint.End:
                    DebugMode.Log("End checkpoint");
                    state = GameState.End;
                    break;
                default:
                    DebugMode.Log("Unhandled checkpoint");
                    break;
            }
        }

        /// <summary>
        /// Action called by the <see cref="QuestionController"/> after the correct answer is selected
        /// </summary>
        public void OnEndQuestion()
        {
            DebugMode.Log("Nb Attempt : " + questionController.NbAttempt);
            state = GameState.PostQuestion;
        }

        /// <summary>
        /// Load the data and configuration to start the run. It's like a "game init".
        /// </summary>
        private void LoadScene()
        {
            state = previousState = GameState.Load;

            GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(() => { GoToMainMenu(); });
            pauseMenu.OnQuitEvent += GoToMainMenu;
            pauseMenu.OnPauseStateChange += OnPauseStateChange;

            DisplayQuestion(false);
            DisplayGameOver(false);
            GenerateTerrain();

            // LoadPlayer & Quizz, if arguments is null --> use generated/defaults inside this methods
            LoadPlayer(DataPersistent.Instance?.playerForGame);
            LoadQuizz(DataPersistent.Instance?.quizzForGame);
            LoadBaseQuestion();
            ReplaceFinishFlagAtEnd();

            UpdateQuestionProgressHUD();
        }

        /// <summary>
        /// Load the player datas
        /// </summary>
        /// <param name="source">The data to set</param>
        /// <remarks>
        /// The <see cref="PlayerController.SetSkin(Player)"/> use default skin if data path can't be found.<br>
        /// If the source is null, use the <see cref="DEFAULT_RELATIVE_IMAGE_PATHS"/> data, if no data in the path, use default skin.
        /// </remarks>
        private void LoadPlayer(Player source)
        {
            Player data = source;

            if (data == null)
            {
                // Load a default skin
                data = new Player();
                data.BodyPath = DEFAULT_RELATIVE_IMAGE_PATHS[0];
                data.HeadPath = DEFAULT_RELATIVE_IMAGE_PATHS[1];
                data.FlagPath = DEFAULT_RELATIVE_IMAGE_PATHS[2];
                data.Title = "Default title";
            }

            player.SetSkin(data);
        }


        /// <summary>
        /// Load the data of the quizz
        /// </summary>
        /// <param name="source">Source quizz</param>
        /// <remarks>
        /// If the source is null, will load a default generated quizz.<br>
        /// Images of questions will try to use <see cref="DEFAULT_RELATIVE_IMAGE_PATHS"/> or default resources.
        /// </remarks>
        private void LoadQuizz(Quizz source)
        {
            if (source != null)
            {
                quizz = source;
                return;
            }

            // Load a default quizz
            quizz = new Quizz();
            quizz.Questions = new List<Question>() {
                new Question() {
                    Sentence = "What is the capital of Switzerland ?",
                    Answers = new List<Answer>() {
                        new Answer() { Sentence = "Geneva", IsCorrect = false },
                        new Answer() { Sentence = "Zurich", IsCorrect = false },
                        new Answer() { Sentence = "Bern", IsCorrect = true },
                        new Answer() { Sentence = "Lausanne", IsCorrect = false }
                    },
                    ImagePath = DEFAULT_RELATIVE_IMAGE_PATHS[3],
                },

                new Question() {
                    Sentence = "What is the name of this game ?",
                    Answers = new List<Answer>() {
                        new Answer() { Sentence = "Bored Question", IsCorrect = false },
                        new Answer() { Sentence = "Re-Vision", IsCorrect = true },
                        new Answer() { Sentence = "Improve My Skills", IsCorrect = false },
                        new Answer() { Sentence = "Knowledge improve", IsCorrect = false }
                    },
                    ImagePath = DEFAULT_RELATIVE_IMAGE_PATHS[4],
                },
            };

            for (int i = 0; i < 10; i++)
            {
                quizz.Questions.Add(new Question()
                {
                    Sentence = "Generated " + i,
                    Answers = new List<Answer>() {
                        new Answer() { Sentence = "Correct", IsCorrect = true },
                        new Answer() { Sentence = "Incorrect", IsCorrect = false },
                        new Answer() { Sentence = "False", IsCorrect = false },
                        new Answer() { Sentence = "None", IsCorrect = false }
                    },
                    ImagePath = DEFAULT_RELATIVE_IMAGE_PATHS[5],
                });
            }

        }


        /// <summary>
        /// Load the questions that will be used for the run.
        /// </summary>
        private void LoadBaseQuestion()
        {
            // keep question with the lowest score and randomize the order
            questions = quizz.Questions
                .OrderBy(q => q.Score)
                .Take(NB_BASE_QUESTION)
                .OrderBy(q => Random.Range(0, 100))
                .ToList();

            // Add a new question checkpoint
            for (int i = 1; i <= questions.Count; i++)
                AddQuestionFlag(i);

            // Setup for stats
            foreach (var question in questions)
                questionsStats.Add(question, new QuestionStats());

        }

        /// <summary>
        /// Add a question to the list of next questions (<see cref="questions"/>) + add a question flag on the terrain
        /// </summary>
        /// <param name="question">Question to add</param>
        /// <param name="index">At which index the question need to be inserted</param>
        private void AddQuestion(Question question, int index)
        {
            questions.Insert(index, question);
            AddQuestionFlag(questions.Count);
        }

        /// <summary>
        /// Add a question at the end of the list of next questions (<see cref="questions"/>) + add a question flag on the terrain
        /// </summary>
        /// <param name="question">Question to add</param>
        /// <remarks>To place the question at a specified index, use <see cref="AddQuestion(Question, int)"/></remarks>
        private void AddQuestionAtEnd(Question question)
        {
            AddQuestion(question, questions.Count);
        }

        /// <summary>
        /// Add a question at a random place in the list of next questions (<see cref="questions"/>) + add a question flag on the terrain.<br></br>
        /// The place is within the range [<see cref="currentQuestionIndex"/>, max[
        /// </summary>
        /// <param name="question">Question to add</param>
        /// <remarks>To place the question at a specified index, use <see cref="AddQuestion(Question, int)"/></remarks>
        private void AddQuestionAtRandomIndex(Question question)
        {
            int index = Random.Range(currentQuestionIndex, questions.Count);
            DebugMode.Log(string.Format("[{0}] inserted at {1}", question.Sentence, index));
            AddQuestion(question, index);
        }

        /// <summary>
        /// Add a question checkpoint on the terrain at the specified index
        /// </summary>
        /// <param name="index">Index where place the checkpoint</param>
        private void AddQuestionFlag(int index)
        {
            if (terrainPoint.Count <= index)
            {
                DebugMode.Log("For now, there is not enough point to add a flag");
                return;
            }

            GameObject flag = Instantiate(Resources.Load("Prefabs/QuestionFlag", typeof(GameObject))) as GameObject;
            flag.transform.position = terrainPoint[index];
            flag.transform.parent = GameObject.Find("Terrain/Flag").transform;
        }

        /// <summary>
        /// Load the specified question to the "question UI"
        /// </summary>
        /// <param name="question">Question to load</param>
        private void LoadCurrentQuestion(Question question)
        {
            DebugMode.Log("Load UI of the question [" + question.Sentence + "]");
            questionController.CurrentQuestion = question;
        }

        /// <summary>
        /// Generate a new terrain coordinate which can be append at the end of <see cref="terrainPoint"/>
        /// </summary>
        /// <returns></returns>
        private Vector2 GenerateNewTerrainPoint()
        {
            return new Vector2(terrainPoint[^1].x + DIST_BETWEEN_X_POINT, Random.Range(-PLUS_MINUS_Y_POINT, PLUS_MINUS_Y_POINT));
            // [^1] == [terrainPoint.Count - 1]
        }


        /// <summary>
        /// Generate a new terrain structure and place the final flag checkpoint
        /// </summary>
        private void GenerateTerrain()
        {
            // Generate the structure coordinate
            terrainPoint.Add(new Vector2(0, 0));

            for (int i = 0; i < NB_DEFAULT_POINT + NB_BASE_QUESTION + 1; i++) // + 1 for final flag
            {
                terrainPoint.Add(GenerateNewTerrainPoint());
            }

            DebugMode.Log("Terrain generated");

            // Display
            foreach (Vector2 point in terrainPoint)
            {
                // UI Debug points (to see coordinate location)
                debug.UI_TerrainPoint(point);
            }

            // Place the coordinate to the terrain GameObject
            terrainController.InitLine(terrainPoint, DEPTH_TERRAIN_POINT, START_X_POINT);

            // place the "End checkpoint"
            GameObject flag = Instantiate(Resources.Load("Prefabs/FinalFlag", typeof(GameObject))) as GameObject;
            flag.name = "FinalFlag"; // removing "(Clone)" from the name
            flag.transform.position = terrainPoint[0];
            flag.transform.parent = GameObject.Find("Terrain/Flag").transform;

            DebugMode.Log("Terrain displayed");
        }

        /// <summary>
        /// Switch the display of the question UI
        /// </summary>
        /// <param name="state">is the UI displayed</param>
        private void DisplayQuestion(bool state)
        {
            question.SetActive(state);
            progressHUD.SetActive(!state);
        }

        /// <summary>
        /// Switch the display of the game over (end game) UI
        /// </summary>
        /// <param name="state">is the UI displayed</param>
        private void DisplayGameOver(bool state)
        {
            gameover.SetActive(state);
        }

        /// <summary>
        /// Actions + configuration when the game reach the state "post question"
        /// </summary>
        private void PostQuestion()
        {
            // Compute the statistiques of the question
            var stats = questionsStats[questionController.CurrentQuestion];
            stats.NbAttempt++;

            int nbAttempt = questionController.NbAttempt;
            stats.NbWrongAnswer += nbAttempt - 1;

            float time = questionController.ElapsedTime;
            stats.MinTime = Mathf.Min(stats.MinTime, time);
            DebugMode.Log("time to answer : " + time);

            int score = EarnedScore(nbAttempt, time);
            stats.EarnedScore += score;

            // TODO Update Score in DB

            // Add if required :
            //  - new questions checkpoints on the terrain
            //  - replace the current question to incomming questions
            //  - move the final checkpoint at the end (after the new question checkpoints)
            for (int toAdd = GetQtyReAddQuestion(nbAttempt); toAdd > 0; toAdd--)
            {
                terrainPoint.Add(GenerateNewTerrainPoint());

                debug.UI_TerrainPoint(terrainPoint[^1]);

                terrainController.AddPoint(terrainPoint[^1]);

                AddQuestionAtRandomIndex(questionController.CurrentQuestion);
                ReplaceFinishFlagAtEnd();
            }

            // Pass to the next question
            currentQuestionIndex++;

            UpdateQuestionProgressHUD();
        }

        /// <summary>
        /// Move the final flag (which trigg the end of the run) at the end (just after all question checkpoint)
        /// </summary>
        private void ReplaceFinishFlagAtEnd()
        {
            GameObject.Find("Terrain/Flag/FinalFlag").transform.position = terrainPoint[questions.Count + 1];
        }

        /// <summary>
        /// Calculate the earned score according the number of attempts of the question
        /// </summary>
        /// <param name="nbAttempt">Number of attempts</param>
        /// <param name="seconds">effective seconds passed to answer the question</param>
        /// <returns>The additional score earned</returns>
        private int EarnedScore(int nbAttempt, float? seconds = null)
        {
            int score = SCORE_FIRST_ATTEMPT;

            if (nbAttempt == 2)
                score = SCORE_SECOND_ATTEMPT;
            else if (nbAttempt == 3)
                score = SCORE_THIRD_ATTEMPT;
            else if (nbAttempt >= 4)
                score = SCORE_FOURTH_OR_MORE_ATTEMPT;
            else if (seconds?.CompareTo(SECONDS_QUICKLY_RESPONSE) < 0)
                score = SCORE_QUICKLY_RESPONSE;

            return score;
        }

        /// <summary>
        /// Get the quantity of time the question need to be replaced for the run, according the number of attemps
        /// </summary>
        /// <param name="nbAttempt">Number of attempt (correct answer included)</param>
        /// <returns>How many time the question should reappear</returns>
        private int GetQtyReAddQuestion(int nbAttempt)
        {
            int qty = 0;

            READD_ONE_QUESTION_AFTER_WRONG.ToList().ForEach(nb =>
            {
                if (nbAttempt > nb)
                    qty++;
            });

            return qty;
        }

        /// <summary>
        /// Setup the data and display the final UI at the end of the game
        /// </summary>
        private void GameOver()
        {
            DebugMode.Log("Chargement de l'UI de fin de partie");

            // The game is over, we can't do a pause
            pauseMenu.CanPaused = false;
            pauseButton.SetActive(false);

            DisplayGameOver(true);

            // Setup the statistics data
            foreach (var question in questions.Distinct())
            {
                var stats = questionsStats[question];
                GameObject questionStats = Instantiate(Resources.Load("Prefabs/QuestionStat", typeof(GameObject))) as GameObject;
                QuestionStatsUIManager script = questionStats.GetComponent<QuestionStatsUIManager>();
                script.SetData(question.Sentence, stats.NbWrongAnswer, stats.MinTime);
                questionStats.transform.SetParent(GameObject.Find("LayoutQuestionStats").transform, false);
            }

            // Is the player did some mistakes ?
            bool hasMistakes = questionsStats.Any(q => q.Value.NbWrongAnswer > 0);

            // TODO : Update in DB : run finished + is a run without mistakes ?
        }

        /// <summary>
        /// Action when pause asked to put the game in pause state
        /// </summary>
        public void OnPause()
        {
            pauseMenu.OnPause(true);
        }

        /// <summary>
        /// Change to the scene "Main Menu".
        /// </summary>
        private void GoToMainMenu()
        {
            Debug.Log("Go to main menu");
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Action triggered when the pause menu is opened or closed.
        /// </summary>
        private void OnPauseStateChange(bool isPaused)
        {
            if (isPaused)
            {
                // Nothing
                // If audio, pause it here
            }
            else
            {
                // Nothing
                // If audio, replay it here
            }
        }

        /// <summary>
        /// Update the HUD used for the question progression with current questions
        /// </summary>
        private void UpdateQuestionProgressHUD()
        {
            progressText.text = String.Format("Question {0} / {1}", currentQuestionIndex, questions.Count);
        }
    }
}