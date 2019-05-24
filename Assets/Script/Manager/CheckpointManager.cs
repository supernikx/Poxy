using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoint Manager Settings")]
    [SerializeField]
    private Transform checkpointContainer;
    [SerializeField]
    private CheckpointBase initialCheckpoint = null;

    private List<CheckpointBase> checkpoints = new List<CheckpointBase>();

    [Header("Debug")]
    [SerializeField]
    private CheckpointBase activeCheckpoint;

    private SpeedrunManager speedMng;

    #region API
    public void Init(SpeedrunManager _speedMng)
    {
        speedMng = _speedMng;

        if (initialCheckpoint == null)
        {
            Debug.LogError("An initial checkpoint should always be setted", initialCheckpoint);
            return;
        }

        for (int i = 0; i < checkpointContainer.childCount; i++)
        {
            CheckpointBase _current = checkpointContainer.GetChild(i).GetComponent<CheckpointBase>();
            checkpoints.Add(_current);
            _current.Init();
            _current.ActivateCheckpoint += HandleActivateCheckpoint;
        }

        activeCheckpoint = initialCheckpoint;
        activeCheckpoint.GetCheckpointAnimatorManager().Enable(true);
    }
    #endregion

    #region Handlers
    private void HandleActivateCheckpoint(CheckpointBase _checkpoint)
    {
        if (!speedMng.GetIsActive() && activeCheckpoint != _checkpoint)
        {
            activeCheckpoint.GetCheckpointAnimatorManager().Enable(false);
            activeCheckpoint = _checkpoint;
            activeCheckpoint.GetCheckpointAnimatorManager().Enable(true);
        }
    }

    private void HandlePlayerDeath()
    {
        activeCheckpoint = initialCheckpoint;
    }
    #endregion

    #region Getters
    public CheckpointBase GetActiveCheckpoint()
    {
        return activeCheckpoint;
    }

    public CheckpointBase GetInitialCheckpoint()
    {
        return initialCheckpoint;
    }
    #endregion
}

