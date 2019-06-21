using UnityEngine;
using System;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    #region Delegates
    public static Action<int> OnLivesChange;
    #endregion

    [Header("Lives Settings")]
    [SerializeField]
    private int lives;

    private int currentLives
    {
        set
        {
            _currentLives = value;
            if (OnLivesChange != null)
                OnLivesChange(currentLives);
        }
        get
        {
            return _currentLives;
        }
    }
    private int _currentLives;

    private Player player;
    private TokenManager tokenMng;

    #region API
    public void Init(Player _player, TokenManager _tokenMng)
    {
        currentLives = lives;
        tokenMng = _tokenMng;
        player = _player;

        player.OnPlayerDeath += HandleOnPlayerDeath;
        tokenMng.OnGainLife += HandleOnGainLife;
    }

    public int GetLives()
    {
        return currentLives;
    }
    #endregion

    #region Handlers
    private void HandleOnPlayerDeath()
    {
        currentLives = Mathf.Clamp(currentLives - 1, 0, 99);
    }

    private void HandleOnGainLife(int _val)
    {
        currentLives = Mathf.Clamp(currentLives + _val, 0, 99);
    }
    #endregion

    private void OnDisable()
    {
        player.OnPlayerDeath -= HandleOnPlayerDeath;
        tokenMng.OnGainLife -= HandleOnGainLife;
    }
}
