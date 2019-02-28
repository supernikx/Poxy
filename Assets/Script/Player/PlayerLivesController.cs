using UnityEngine;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    #region Delegates
    public delegate Vector3 LivesEvent();
    public LivesEvent LoseLife;
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
        }
        else
            _checkpoint = checkpointMng.GetActiveCheckpoint();

        enemyMng.ResetEnemies();

        // TODO: Se si finiscono le vite resettare tutto

        return _checkpoint.transform.position;
    }
    #endregion

}
