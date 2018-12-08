using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Referenza al Pool manager
    /// </summary>
    PoolManager poolMng;
    /// <summary>
    /// Referenza al player
    /// </summary>
    Player player;
    /// <summary>
    /// Reference to enemy
    /// </summary>
    Enemy enemy;

    /// <summary>
    /// Inizializzazione elementi del livello (verrà chiamata dalla SM)
    /// </summary>
    void Start()
    {
        poolMng = GetComponent<PoolManager>();
        if (poolMng != null)
        {
            poolMng.Init();
        }

        player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.Init();
        }

        enemy = FindObjectOfType<Enemy>();
        if (enemy != null)
        {
            enemy.Init();
        }
    }
}
