using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Transform spawn1Pos;
    [SerializeField]
    private Transform spawn2Pos;

    public static LevelManager singleton;
    /// <summary>
    /// Referenza al Pool manager
    /// </summary>
    PoolManager poolMng;
    /// <summary>
    /// Referenza al player
    /// </summary>
    Player player;
    /// <summary>
    /// Referenza all'Enemy Manager
    /// </summary>
    EnemyManager enemyMng;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

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
    public void Init()
    {
        singleton = this;

        //Init
        poolMng = GetComponent<PoolManager>();
        if (poolMng != null)
            poolMng.Init();

        enemyMng = GetComponent<EnemyManager>();
        if (enemyMng != null)
            enemyMng.Init();

        platformMng = GetComponent<PlatformManager>();
        if (platformMng != null)
            platformMng.Init();

        doorsButtonsMng = GetComponent<DoorsButtonsManager>();
        if (doorsButtonsMng != null)
            doorsButtonsMng.Init();

        checkpointMng = GetComponent<CheckpointManager>();
        if (checkpointMng != null)
            checkpointMng.Init();

        tokenMng = GetComponent<TokenManager>();
        if (tokenMng != null)
            tokenMng.Init();

        player = FindObjectOfType<Player>();
        if (player != null)
            player.Init(enemyMng, platformMng, checkpointMng);

        //Setup
        enemyMng.EnemiesSetup();

        //Iscrizione Eventi
        player.OnPlayerDeath += HandlePlayerDeath;
        tokenMng.FinishToken += HandleFinishToken;
    }
    #endregion

    #region Handlers
    private void HandlePlayerDeath()
    {
        Debug.Log("The Player is dead");
    }

    private void HandleFinishToken()
    {
        Debug.Log("All tokens have been taken");
    }
    #endregion

    #region Getter
    public EnemyManager GetEnemyManager()
    {
        return enemyMng;
    }
    #endregion
}
