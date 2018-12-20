using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParasiteController : MonoBehaviour
{
    [SerializeField]
    float parasiteRange;

    EnemyManager enemyMng;
    Player player;
    IEnemy parasiteEnemy;

    #region API
    /// <summary>
    /// Funzione di inizializzazione
    /// </summary>
    /// <param name="_player"></param>
    public void Init(Player _player)
    {
        enemyMng = LevelManager.singleton.GetEnemyManager();
        player = _player;
    }

    /// <summary>
    /// Funzione che controlla se si può possedere un nemico
    /// </summary>
    /// <returns></returns>
    public IEnemy CheckParasite()
    {
        IEnemy e = enemyMng.GetNearestStunnedEnemy(transform, parasiteRange);
        if (e != null)
        {
            Debug.Log("Posso entrare dentro " + e);
        }

        return e;
    }

    #region Setters
    /// <summary>
    /// Funzione che setta il nemico posseduto
    /// </summary>
    /// <param name="_enemy"></param>
    public void SetParasiteEnemy(IEnemy _enemy)
    {
        parasiteEnemy = _enemy;
    }
    #endregion

    #region Getters
    /// <summary>
    /// Funzione che ritorna il nemico posseduto
    /// </summary>
    /// <returns></returns>
    public IEnemy GetParasiteEnemy()
    {
        return parasiteEnemy;
    }
    #endregion
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, parasiteRange);
    }
}
