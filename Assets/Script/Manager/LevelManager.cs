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
    #endregion

    //Debug
    [SerializeField]
    private Transform spawn1Pos;
    [SerializeField]
    private Transform spawn2Pos;

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

    private bool pause;
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.transform.position = spawn1Pos.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.transform.position = spawn2Pos.position;
        }
    }

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

        tokenMng = GetComponent<TokenManager>();
        if (tokenMng != null)
            tokenMng.Init();

        enemyMng = GetComponent<EnemyManager>();
        if (enemyMng != null)
            enemyMng.Init();

        platformMng = GetComponent<PlatformManager>();
        if (platformMng != null)
            platformMng.Init();

        doorsButtonsMng = GetComponent<DoorsButtonsManager>();
        if (doorsButtonsMng != null)
            doorsButtonsMng.Init(tokenMng);

        checkpointMng = GetComponent<CheckpointManager>();
        if (checkpointMng != null)
            checkpointMng.Init();

        player = FindObjectOfType<Player>();
        if (player != null)
            player.Init(enemyMng, platformMng, checkpointMng);

        //Setup
        enemyMng.EnemiesSetup();

        //Iscrizione Eventi
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
        Application.Quit();
    }

    /// <summary>
    /// Funzione che sposta il player alla start position
    /// </summary>
    public void RestartGame()
    {
        player.transform.position = spawn1Pos.position;
        if (OnGameUnPause != null)
            OnGameUnPause();
    }

    #region Pause
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
    #endregion

    private void OnDisable()
    {
        player.OnPlayerDeath -= HandlePlayerDeath;
        tokenMng.FinishToken -= HandleFinishToken;
        OnGamePause -= GamePause;
        OnGameUnPause -= GameUnPause;
    }
}
