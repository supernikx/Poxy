using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
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
    /// Inizializzazione elementi del livello (verrà chiamata dalla SM)
    /// </summary>
    public void Init()
    {
        singleton = this;

        poolMng = GetComponent<PoolManager>();
        if (poolMng != null)
            poolMng.Init();

        enemyMng = GetComponent<EnemyManager>();
        if (enemyMng != null)
            enemyMng.Init();

        player = FindObjectOfType<Player>();
        if (player != null)
            player.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    #region Getter
    public EnemyManager GetEnemyManager()
    {
        return enemyMng;
    }
    #endregion
}
