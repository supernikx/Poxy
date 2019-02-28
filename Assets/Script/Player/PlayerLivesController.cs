using UnityEngine;
using System;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    #region Delegates
    public delegate Vector3 LivesEvent();
    public LivesEvent LoseLife;
    public Action OnPlayerDeath;
    #endregion

    [Header("Lives Settings")]
    [SerializeField]
    private int lives;
    [SerializeField]
    private int currentLives;

    private CheckpointManager checkpointMng;
    private EnemyManager enemyMng;

    #region API
    public void Init(CheckpointManager _checkpointMng, EnemyManager _enemyMng)
    {
        checkpointMng = _checkpointMng;
        enemyMng = _enemyMng;
        currentLives = lives;

        LoseLife += HandleLoseLife;
    }
    #endregion

    #region Handlers
    public Vector3 HandleLoseLife()
    {
        CheckpointBase _checkpoint;
        currentLives--;
        
        if (currentLives <= 0)
        {
            _checkpoint = checkpointMng.GetInitialCheckpoint();
            currentLives = lives;
            if (OnPlayerDeath != null)
                OnPlayerDeath();
        }
        else
            _checkpoint = checkpointMng.GetActiveCheckpoint();

        enemyMng.ResetEnemies();

        return _checkpoint.transform.position;
    }
    #endregion

}
