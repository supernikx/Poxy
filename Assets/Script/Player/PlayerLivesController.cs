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

    #region API
    public void Init(CheckpointManager _checkpointMng)
    {
        checkpointMng = _checkpointMng;
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

        return _checkpoint.transform.position;
    }
    #endregion

}
