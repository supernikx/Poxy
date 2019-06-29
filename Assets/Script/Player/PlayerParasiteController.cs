using System;
using UnityEngine;

public class PlayerParasiteController : MonoBehaviour
{
    #region Actions
    public static Action OnPlayerParasite;
    public static Action OnPlayerParasiteEnd;
    #endregion

    [SerializeField]
    float parasiteRange;

    EnemyManager enemyMng;
    PlatformManager platformMng;
    Player player;
    // sostituire con un solo IController
    /*
    IEnemy parasiteEnemy;
    LaunchingPlatform parasitePlatform*/
    IControllable parasiteTarget;

    #region API
    /// <summary>
    /// Funzione di inizializzazione
    /// </summary>
    /// <param name="_player"></param>
    public void Init(Player _player, EnemyManager _enemyMng, PlatformManager _platformMng)
    {
        enemyMng = _enemyMng;
        platformMng = _platformMng;
        player = _player;
    }

    /// <summary>
    /// Funzione che controlla se si può possedere un nemico
    /// </summary>
    /// <returns></returns>
    public IControllable CheckParasite()
    {
        IControllable e = enemyMng.GetNearestStunnedEnemy(transform, parasiteRange);
        IControllable p = platformMng.GetNearestLaunchingPlatform(transform, parasiteRange);
        if (e == null && p == null)
        {
            return null;
        }
        else if (e != null && p == null)
        {
            return e;
        }
        else if (e == null && p != null)
        {
            return p;
        }
        else if (e != null && p != null)
        {
            float distanceE = Vector3.Distance(transform.position, e.gameObject.transform.position);
            float distanceP = Vector3.Distance(transform.position, p.gameObject.transform.position);

            return (distanceE <= distanceP) ? e : p;
        }
        return null;
    }

    #region Setters
    /// <summary>
    /// Funzione che setta il nemico posseduto
    /// </summary>
    /// <param name="_enemy"></param>
    public void SetParasite(IControllable _parasite)
    {
        if (parasiteTarget != null && _parasite == null)
        {
            if (OnPlayerParasiteEnd != null)
                OnPlayerParasiteEnd();
        }
        else if (parasiteTarget == null && _parasite != null)
        {
            if (OnPlayerParasite != null)
                OnPlayerParasite();
        }

        parasiteTarget = _parasite;
    }
    #endregion

    #region Getters
    /// <summary>
    /// Funzione che ritorna il nemico posseduto
    /// </summary>
    /// <returns></returns>
    public IControllable GetParasite()
    {
        return parasiteTarget;
    }

    /// <summary>
    /// Funzione che ritorna il range di parasite del player
    /// </summary>
    /// <returns></returns>
    public float GetRange()
    {
        return parasiteRange;
    }
    #endregion
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, parasiteRange);
    }
}
