using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region Delegates
    public static Action OnGamePause;
    public static Action OnGameUnPause;
    public static Action OnPlayerDeath;
    public static Action OnPlayerEndLevel;
    #endregion

    public static LevelManager instance;
    /// <summary>
    /// Referenza al Pool manager
    /// </summary>
    private PoolManager poolMng;
    /// <summary>
    /// Referenza al player
    /// </summary>
    private Player player;
    /// <summary>
    /// Referenza all'Enemy Manager
    /// </summary>
    private EnemyManager enemyMng;
    /// <summary>
    /// Riferimento all'ui gameplay Manager
    /// </summary>
    private UI_GameplayManager uiManager;
    /// <summary>
    /// Reference to Platform Manager
    /// </summary>
    private PlatformManager platformMng;
    /// <summary>
    /// Reference to Checkpoint Manager
    /// </summary>
    private CheckpointManager checkpointMng;
    /// <summary>
    /// Reference allo sticky manager
    /// </summary>
    private StickyObjectManager stickyMng;
    /// <summary>
    /// Reference al poop controller
    /// </summary>
    private PoopController poopCtrl;
    /// <summary>
    /// Reference al camera manager
    /// </summary>
    private CameraManager cameraMng;
    /// <summary>
    /// Reference al camera manager
    /// </summary>
    private DoorsButtonsManager doorsMng;
    /// <summary>
    /// Reference al speedrun manager
    /// </summary>
    private SpeedrunManager speedMng;
    /// <summary>
    /// Reference al tutorial manager
    /// </summary>
    private TutorialManager tutorialMng;
    /// <summary>
    /// Reference al token manager
    /// </summary>
    private TokenManager tokenMng;
    /// <summary>
    /// Reference al video stream
    /// </summary>
    private StreamVideo videoStream;

    #region API
    /// <summary>
    /// Inizializzazione elementi del livello (verrà chiamata dalla SM)
    /// </summary>
    public void Init(UI_ManagerBase _uiManager, bool _speedrunMode)
    {
        instance = this;
        pause = false;
        uiManager = _uiManager.GetGameplayManager();

        //Init
        poolMng = GetComponent<PoolManager>();
        if (poolMng != null)
            poolMng.Init();

        stickyMng = GetComponent<StickyObjectManager>();
        if (stickyMng != null)
            stickyMng.Init();

        enemyMng = GetComponent<EnemyManager>();
        if (enemyMng != null)
            enemyMng.Init();

        platformMng = GetComponent<PlatformManager>();
        if (platformMng != null)
            platformMng.Init(uiManager.GetGameplayManager());

        speedMng = GetComponent<SpeedrunManager>();
        if (speedMng != null)
            speedMng.Init(_speedrunMode);

        checkpointMng = GetComponent<CheckpointManager>();
        if (checkpointMng != null)
            checkpointMng.Init(speedMng);

        poopCtrl = FindObjectOfType<PoopController>();
        if (poopCtrl != null)
            poopCtrl.Init();

        tokenMng = GetComponent<TokenManager>();
        if (tokenMng != null)
            tokenMng.Init(speedMng);

        player = FindObjectOfType<Player>();
        if (player != null)
            player.Init(enemyMng, platformMng, tokenMng);

        tutorialMng = GetComponent<TutorialManager>();
        if (tutorialMng != null)
            tutorialMng.Init(enemyMng, player);

        cameraMng = GetComponent<CameraManager>();
        if (cameraMng != null)
            cameraMng.Init();

        doorsMng = GetComponent<DoorsButtonsManager>();
        if (doorsMng != null)
            doorsMng.Init();

        videoStream = FindObjectOfType<StreamVideo>();

        //Setup
        enemyMng.EnemiesSetup();

        //Iscrizione Eventi
        PlayerInputManager.OnPausePressed += HandlePlayerPauseButtonPressed;
        player.OnPlayerDeath += HandlePlayerDeath;
        OnGamePause += GamePause;
        OnGameUnPause += GameUnPause;
    }

    /// <summary>
    /// Funzione che chiude il gioco
    /// </summary>
    public void QuitGame()
    {
        GameManager.QuitGame();
    }

    /// <summary>
    /// Funzione che sposta il player alla start position
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (GameManager.Exist())
            GameManager.RestartCurrentLevel();
    }

    /// <summary>
    /// Funzione che fa tornare al menu principale
    /// </summary>
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Exist())
            GameManager.BackToMenu();
    }

    #region Pause
    private bool pause;
    /// <summary>
    /// Funzione che gestisce l'evento PlayerInputManager.OnPausePressed 
    /// </summary>
    private void HandlePlayerPauseButtonPressed()
    {
        if (!pause)
        {
            if (OnGamePause != null)
                OnGamePause();
        }
        else if (pause)
        {
            if (OnGameUnPause != null)
                OnGameUnPause();
        }
    }

    /// <summary>
    /// Funzioen che gestisce l'evento OnGamePause
    /// </summary>
    public void GamePause()
    {
        pause = true;
        Time.timeScale = 0f;
        PlayerInputManager.SetCanReadGameplayInput(false);
        if (SpeedrunManager.PauseTimer != null)
            SpeedrunManager.PauseTimer();
    }

    /// <summary>
    /// Funzioen che gestisce l'evento OnGameUnPause
    /// </summary>
    public void GameUnPause()
    {
        pause = false;
        Time.timeScale = 1f;
        PlayerInputManager.SetCanReadGameplayInput(true);
        if (SpeedrunManager.ResumeTimer != null)
            SpeedrunManager.ResumeTimer();
    }
    #endregion
    #endregion

    #region Handlers
    /// <summary>
    /// Funzione che gestisce l'evento player.OnPlayerDeath
    /// </summary>
    private void HandlePlayerDeath()
    {
        if (speedMng.GetIsActive() && GameManager.Exist())
            GameManager.RestartCurrentLevel();
        else
        {
            if (GameManager.Exist() && player.GetLivesController().GetLives() == 0)
            {
                Time.timeScale = 0f;
                uiManager.ToggleMenu(MenuType.GameOver);
            }
            else
            {
                if (OnPlayerDeath != null)
                    OnPlayerDeath();
            }
        }
    }

    /// <summary>
    /// Funzione che gestisce l'evento tokenMng.FinishToken
    /// </summary>
    private void HandleFinishToken()
    {
        Debug.Log("All tokens have been taken");
    }
    #endregion

    #region Getter
    /// <summary>
    /// Funzione che ritorna l'enemy manager
    /// </summary>
    /// <returns></returns>
    public EnemyManager GetEnemyManager()
    {
        return enemyMng;
    }

    /// <summary>
    /// Funzione che ritorna l'ui gameplay manager
    /// </summary>
    /// <returns></returns>
    public UI_GameplayManager GetUIGameplayManager()
    {
        return uiManager;
    }

    /// <summary>
    /// Funzione che ritorna il checkpoint manager
    /// </summary>
    /// <returns></returns>
    public CheckpointManager GetCheckpointManager()
    {
        return checkpointMng;
    }

    /// <summary>
    /// Funzione che ritorna il camera manager
    /// </summary>
    /// <returns></returns>
    public CameraManager GetCameraManager()
    {
        return cameraMng;
    }

    /// <summary>
    /// Funzione che ritorna il riferimento al player
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Funzione che ritorna lo speedrun manager
    /// </summary>
    /// <returns></returns>
    public SpeedrunManager GetSpeedrunManager()
    {
        return speedMng;
    }

    /// <summary>
    /// Funzione che ritorna il video stream
    /// </summary>
    /// <returns></returns>
    public StreamVideo GetVideoStream()
    {
        return videoStream; 
    }

    /// <summary>
    /// Funzione che ritorna se esiste o no
    /// </summary>
    /// <returns></returns>
    public static bool Exist()
    {
        return instance != null;
    }
    #endregion

    private void OnDisable()
    {
        PlayerInputManager.OnPausePressed -= HandlePlayerPauseButtonPressed;
        player.OnPlayerDeath -= HandlePlayerDeath;
        OnGamePause -= GamePause;
        OnGameUnPause -= GameUnPause;
    }
}
