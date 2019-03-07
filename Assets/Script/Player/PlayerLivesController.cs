using UnityEngine;
using System;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    #region Delegates
    public delegate void PlayerLivesDelegates(int lives);
    public static PlayerLivesDelegates OnLivesChange;
    #endregion

    [Header("Lives Settings")]
    [SerializeField]
    private int lives;
    private int _currentLives;
    private int currentLives
    {
        set
        {
            _currentLives = value;
            if (OnLivesChange != null)
                OnLivesChange(_currentLives);
        }
        get
        {
            return _currentLives;
        }
    }

    #region API
    public void Init()
    {
        currentLives = lives;
    }
    #endregion

    #region Handlers
    public void LoseLives()
    {
        currentLives = Mathf.Clamp(currentLives--, 0, lives);

        if (currentLives == 0)
        {
            if (LevelManager.OnGameOver != null)
                LevelManager.OnGameOver();
        }
    }
    #endregion

    public void SetLives(int _lives)
    {
        currentLives = _lives;
    }

    public int GetLives()
    {
        return currentLives;
    }
}
