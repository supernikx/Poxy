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

    #region API
    public void Init()
    {
        if (initialCheckpoint == null)
        {
            Debug.LogError("An initial checkpoint should always be setted", initialCheckpoint);
        }
        activeCheckpoint = initialCheckpoint;

        for (int i = 0; i < checkpointContainer.childCount; i++)
        {
            CheckpointBase _current = checkpointContainer.GetChild(i).GetComponent<CheckpointBase>();
            checkpoints.Add(_current);
            _current.Init();
            _current.ActivateCheckpoint += HandleActivateCheckpoint;
        }

        LevelManager.OnPlayerDeath += HandlePlayerDeath;
    }
    #endregion

    #region Handlers
    private void HandleActivateCheckpoint(CheckpointBase _checkpoint)
    {
        if (activeCheckpoint != _checkpoint)
            activeCheckpoint = _checkpoint;
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

