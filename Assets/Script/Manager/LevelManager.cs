using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region Delegates
    public delegate void GameDelegate();
    public static GameDelegate OnGamePause;
    public static GameDelegate OnGameUnPause;
    public static GameDelegate OnPlayerDeath;
    public static GameDelegate OnGameOver;
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
    /// Reference to Doors and Buttons Manager
    /// </summary>
    private DoorsButtonsManager doorsButtonsMng;
    /// <summary>
    /// Reference to Checkpoint Manager
    /// </summary>
    private CheckpointManager checkpointMng;
    /// <summary>
    /// Reference to Token Manager
    /// </summary>
    private TokenManager tokenMng;
    /// <summary>
    /// Reference allo sticky manager
    /// </summary>
    private StickyObjectManager stickyMng;

    #region API
    /// <summary>
    /// Inizializzazione elementi del livello (verrà chiamata dalla SM)
    /// </summary>
    public void Init(UI_ManagerBase _uiManager)
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

        tokenMng = GetComponent<TokenManager>();
        if (tokenMng != null)
            tokenMng.Init();

        enemyMng = GetComponent<EnemyManager>();
        if (enemyMng != null)
            enemyMng.Init();

        platformMng = GetComponent<PlatformManager>();
        if (platformMng != null)
            platformMng.Init(uiManager.GetGameplayManager());

        doorsButtonsMng = GetComponent<DoorsButtonsManager>();
        if (doorsButtonsMng != null)
            doorsButtonsMng.Init(tokenMng);

        checkpointMng = GetComponent<CheckpointManager>();
        if (checkpointMng != null)
            checkpointMng.Init();

        player = FindObjectOfType<Player>();
        if (player != null)
            player.Init(enemyMng, platformMng);

        //Setup
        enemyMng.EnemiesSetup();

        //Iscrizione Eventi
        PlayerInputManager.OnPausePressed += HandlePlayerPauseButtonPressed;
        player.OnPlayerDeath += HandlePlayerDeath;
        tokenMng.FinishToken += HandleFinishToken;
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
        player.GetLivesController().SetLives(0);
        if (player.OnPlayerDeath != null)
            player.OnPlayerDeath();
        if (OnGameUnPause != null)
            OnGameUnPause();
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
        player.GetMovementController().SetCanMove(false);
        player.GetShotController().SetCanShoot(false);
    }

    /// <summary>
    /// Funzioen che gestisce l'evento OnGameUnPause
    /// </summary>
    public void GameUnPause()
    {
        pause = false;
        Time.timeScale = 1f;
        player.GetMovementController().SetCanMove(true);
        player.GetShotController().SetCanShoot(true);
    }
    #endregion
    #endregion

    #region Handlers
    /// <summary>
    /// Funzione che gestisce l'evento player.OnPlayerDeath
    /// </summary>
    private void HandlePlayerDeath()
    {
        Debug.Log("The Player is dead");
        if (OnPlayerDeath != null)
            OnPlayerDeath();
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
    #endregion

    private void OnDisable()
    {
        PlayerInputManager.OnPausePressed -= HandlePlayerPauseButtonPressed;
        player.OnPlayerDeath -= HandlePlayerDeath;
        tokenMng.FinishToken -= HandleFinishToken;
        OnGamePause -= GamePause;
        OnGameUnPause -= GameUnPause;
    }
}
