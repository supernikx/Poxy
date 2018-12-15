using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.GameSM;
using UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager singleton;
    /// <summary>
    /// Riferimento alla GameSM
    /// </summary>
    GameSMController gameSM;
    /// <summary>
    /// Riferimento all'ui manager
    /// </summary>
    UIManager uiManager;

    private void Awake()
    {
        //Get Components
        gameSM = GetComponent<GameSMController>();
        uiManager = GetComponent<UIManager>();

        // Singleton
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    void Start()
    {
        uiManager.Init();
        gameSM.Init(this);
    }

    #region API
    /// <summary>
    /// Funzione che ritorna l'ui manager
    /// </summary>
    public UIManager GetUIManager()
    {
        return uiManager;
    }

    /// <summary>
    /// Funzione che inizia la partita
    /// </summary>
    public void StartGame()
    {
        if (gameSM.GoToLevelSetup != null)
            gameSM.GoToLevelSetup();
    }
    #endregion
}
