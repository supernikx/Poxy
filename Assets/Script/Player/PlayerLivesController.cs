using UnityEngine;
using System;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    #region Delegates
    public Action<int> OnLivesChange;

    public static Action OnAllLivesLost;
    #endregion

    [Header("Lives Settings")]
    [SerializeField]
    private int lives;

    [Header("DEBUG")]
    [SerializeField]
    private int currentLives;

    private TokenManager tokenMng;

    #region API
    public void Init(TokenManager _tokenMng)
    {
        currentLives = lives;
        tokenMng = _tokenMng;

        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;
        tokenMng.OnGainLife += HandleOnGainLife;
    }
    #endregion

    #region Handlers
    private void HandleOnPlayerDeath()
    {
        currentLives--;

        if (OnLivesChange != null)
            OnLivesChange(currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("All lives lost");
            if (OnAllLivesLost != null)
                OnAllLivesLost();
        }
    }

    private void HandleOnGainLife(int _val)
    {
        currentLives += _val;
        if (currentLives > lives)
            currentLives = lives;
    }
    #endregion

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandleOnPlayerDeath;
        tokenMng.OnGainLife -= HandleOnGainLife;
    }
}
