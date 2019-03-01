using UnityEngine;
using System;
using System.Collections;

public class PlayerLivesController : MonoBehaviour
{
    [Header("Lives Settings")]
    [SerializeField]
    private int lives;
    [SerializeField]
    private int currentLives;

    #region API
    public void Init()
    {
        currentLives = lives;
    }
    #endregion

    #region Handlers
    public void LoseLives()
    {
        currentLives--;
        
        if (currentLives <= 0)
        {
            currentLives = lives;
            Debug.Log("Hai finito le vite");
        }
    }
    #endregion

}
