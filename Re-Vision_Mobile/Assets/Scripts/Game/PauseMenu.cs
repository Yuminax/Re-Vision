using UnityEngine;
using UnityEngine.UI;

namespace ReVision.Game
{
    /// <summary>
    /// Class for the In Game pause menu
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Is the game can be paused (and the menu opened)
        /// </summary>
        public bool CanPaused { get; set; } = true;

        /// <summary>
        /// Is the game actually paused
        /// </summary>
        public bool IsPaused { get; private set; } = false;
        /// <summary>
        /// Reference to the pause menu panel
        /// </summary>
        /// <remarks>This reference is getting thourgh the componenent of the player <see cref="GameObject.Find(PauseMenuPanel)"/></remarks>
        private GameObject PauseMenuPanel;
        /// <summary>
        /// Reference to the "resume level" button
        /// </summary>
        /// <remarks>This reference is getting thourgh the componenent of the player <see cref="GameObject.Find(ResumeButton)"/></remarks>
        private Button ResumeButton;
        /// <summary>
        /// Reference to the "restart level" button
        /// </summary>
        /// <remarks>This reference is getting thourgh the componenent of the player <see cref="GameObject.Find(RestartButton)"/></remarks>
        private Button RestartButton;
        /// <summary>
        /// Reference to the "quit level" button
        /// </summary>
        /// <remarks>This reference is getting thourgh the componenent of the player <see cref="GameObject.Find(QuitButton)"/></remarks>
        private Button QuitButton;

        //EVENT
        public delegate void RestartEvent();
        /// <summary>
        /// Event triggered when the user wants to restart the level
        /// </summary>
        public event RestartEvent OnRestartEvent;
        public delegate void QuitEvent();
        /// <summary>
        /// Event triggered when the user wants to quit the level
        /// </summary>
        public event QuitEvent OnQuitEvent;

        public delegate void PauseEvent(bool isPaused);
        /// <summary>
        /// Event triggered when the user wants to quit the level
        /// </summary>
        public event PauseEvent OnPauseStateChange;

        void Awake()
        {
            PauseMenuPanel = GameObject.Find("PauseMenuPanel");
            ResumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
            //RestartButton = GameObject.Find("RestartButton").GetComponent<Button>();
            QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        }

        void OnEnable()
        {
            ResumeButton.onClick.AddListener(() => OnResume());
            //RestartButton.onClick.AddListener(() => OnRestart());
            QuitButton.onClick.AddListener(() => OnQuit());
        }

        void Start()
        {
            PauseMenuPanel.SetActive(IsPaused);
        }

        // Update is called once per frame
        void Update()
        {
            if (CanPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                OnPause(!IsPaused);
            }
        }

        void OnDisable()
        {
            ResumeButton.onClick.RemoveAllListeners();
            //RestartButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Action when the game is paused
        /// </summary>
        /// <param name="pause"></param>
        public void OnPause(bool pause)
        {
            if (!CanPaused)
                return;

            // Update the pause + menu display states
            IsPaused = pause;

            PauseMenuPanel.SetActive(IsPaused);

            if (IsPaused)
            {
                // the game time will stop movements
                Time.timeScale = 0;
                Debug.Log("[PAUSE MENU] game paused");
            }
            else
            {
                // the game time will restart as normaly
                Time.timeScale = 1;
                Debug.Log("[PAUSE MENU] game resumed");
            }

            OnPauseStateChange?.Invoke(pause);
        }

        /// <summary>
        /// Action when the user wants to resume the level
        /// </summary>
        public void OnResume()
        {
            OnPause(false);
        }

        /// <summary>
        /// Action when the user wants to restart the level
        /// </summary>
        public void OnRestart()
        {
            Debug.Log("[PAUSE MENU] asked for a restart");
            OnPause(false);
            OnRestartEvent?.Invoke();
        }


        /// <summary>
        /// Action when the user wants to quit the level
        /// </summary>
        public void OnQuit()
        {
            Debug.Log("[PAUSE MENU] asked quitting level");
            OnPause(false);
            OnQuitEvent?.Invoke();
        }
    }
}