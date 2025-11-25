using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JetSystems
{
	public class UIManager : MonoBehaviour
	{
        public enum Orientation { Portrait, Landscape }
        public Orientation orientation;

        public enum GameState { MENU, GAME, LEVELCOMPLETE, GAMEOVER}
        public static GameState gameState;

        #region Static Variables

        public static int COINS;
        public static UIManager instance;

        #endregion

        #region Delegates

        public delegate void SetMenuDelegate();
        public static SetMenuDelegate setMenuDelegate;

        public delegate void OnMenuSet();
        public static OnMenuSet onMenuSet;

        public delegate void SetGameDelegate();
        public static SetGameDelegate setGameDelegate;

        public delegate void OnGameSet();
        public static OnGameSet onGameSet;

        public delegate void SetLevelCompleteDelegate(int starsCount = 3);
        public static SetLevelCompleteDelegate setLevelCompleteDelegate;

        public delegate void OnLevelCompleteSet(int starsCount = 3);
        public static OnLevelCompleteSet onLevelCompleteSet;

        public delegate void SetGameoverDelegate();
        public static SetGameoverDelegate setGameoverDelegate;

        public delegate void OnGameoverSet();
        public static OnGameoverSet onGameoverSet;

        public delegate void UpdateProgressBarDelegate(float value);
        public static UpdateProgressBarDelegate updateProgressBarDelegate;

        public delegate void OnNextLevelButtonPressed();
        public static OnNextLevelButtonPressed onNextLevelButtonPressed;

        public delegate void OnRetryButtonPressed();
        public static OnRetryButtonPressed onRetryButtonPressed;

        #endregion

        // Canvas Groups
        public CanvasGroup MENU;
        public CanvasGroup GAME;
        public CanvasGroup LEVELCOMPLETE;
        public CanvasGroup GAMEOVER;
        public CanvasGroup[] canvases;

        // Menu UI
        public Text menuCoinsText;

        // Game UI
        public Slider progressBar;
        public Text gameCoinsText;

        // Level Complete UI
        public Text levelCompleteCoinsText;
        public string URL = "https://play.google.com/store/apps/details?id=com.treasureplay.treasureplayapp&hl=en";
        private void Awake()
        {
            if (instance == null)
                instance = this;

            // Get the coins amount
            COINS = 0;
            UpdateCoins();
        }

        // Start is called before the first frame update
        void Start()
		{
            // Store the canvases
            canvases = new CanvasGroup[] { MENU, GAME, LEVELCOMPLETE, GAMEOVER };

            // Configure the delegates
            ConfigureDelegates();

            // Set the menu at start
            SetMenu();
		}

        private void ConfigureDelegates()
        {
            // Basic events
            setMenuDelegate += SetMenu;
            setGameDelegate += SetGame;
            setLevelCompleteDelegate += SetLevelComplete;
            setGameoverDelegate += SetGameover;

            // Progress bar events
            updateProgressBarDelegate += UpdateProgressBar;
        }

        private void OnDestroy()
        {

            // Basic events
            setMenuDelegate -= SetMenu;
            setGameDelegate -= SetGame;
            setLevelCompleteDelegate -= SetLevelComplete;
            setGameoverDelegate -= SetGameover;

            // Progress bar events
            updateProgressBarDelegate -= UpdateProgressBar;
        }

        // Update is called once per frame
        void Update()
		{
            if (Input.GetKeyDown(KeyCode.C))
                SetLevelComplete();
		}

        public void SetMenu()
        {
            
            gameState = GameState.MENU;
            Utils.HideAllCGs(canvases, MENU);

            // Invoke the delegate
            onMenuSet?.Invoke();
            
        }

        public void SetGame()
        {
            gameState = GameState.GAME;
            Utils.HideAllCGs(canvases, GAME);

            // Invoke the delegate
            onGameSet?.Invoke();

            // Reset the progress bar
            progressBar.value = 0;

            Audio_Manager.instance.Play("Play");
        }

        public void SetLevelComplete(int starsCount = 3)
        {
            gameState = GameState.LEVELCOMPLETE;
            Utils.HideAllCGs(canvases, LEVELCOMPLETE);

            // Invoke the delegate
            onLevelCompleteSet?.Invoke(starsCount);
            // Audio_Manager.instance.play("Level_Complete");         
           
        }

        public void SetGameover()
        {
            gameState = GameState.GAMEOVER;
            Utils.HideAllCGs(canvases, GAMEOVER);

            // Invoke the delegate
            onGameoverSet?.Invoke();
            Audio_Manager.instance.Play("Game_Over");           
        }

        public void RetryButtonCallback()
        {
            SetMenu();
           
            // Invoke the retry button delegate
            onRetryButtonPressed?.Invoke();
            SceneManager.LoadScene(0);
            

        }

        public void GotoStoreButtonCallBack()
        {
            GoToStore(URL);
        }

        public void UpdateProgressBar(float value)
        {
            progressBar.value = value;
        }

        private void UpdateCoins()
        {
            menuCoinsText.text = Utils.FormatAmountString(COINS);
            gameCoinsText.text = menuCoinsText.text;
            levelCompleteCoinsText.text = menuCoinsText.text;
        }

        #region Static Methods

        public static void AddCoins(int amount)
        {
            // Increase the amount of coins
            COINS += amount;

            // Update the coins
            instance.UpdateCoins();

            // Save the amount of coins
            PlayerPrefsManager.SaveCoins(COINS);
        }

        public static bool IsGame()
        {
            return gameState == GameState.GAME;
        }

        public static bool IsLevelComplete()
        {
            return gameState == GameState.LEVELCOMPLETE;
        }

        public static bool IsGameover()
        {
            return gameState == GameState.GAMEOVER;
        }

        #endregion

        [DllImport("__Internal")]
        private static extern void OpenStore(string url);

        public void GoToStore(string url)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenStore(url);
#else
            Application.OpenURL(url);
#endif
        }
    }


}