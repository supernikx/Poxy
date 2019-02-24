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
    UI_ManagerBase uiManager;

    private void Awake()
    {
        //Get Components
        gameSM = GetComponent<GameSMController>();

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
        gameSM.Init(this);
    }

    #region API
    /// <summary>
    /// Funzione che cerca un ui manager in scena e se è diverso da quello precedente lo sostituisce
    /// </summary>
    public UI_ManagerBase FindUIManager()
    {
        UI_ManagerBase newUi = FindObjectOfType<UI_ManagerBase>();
        if (newUi != uiManager)
        {
            uiManager = newUi;
            uiManager.Setup(this);
        }
        
        return uiManager;
    }

    /// <summary>
    /// Funzione che ritorna l'ui manager
    /// </summary>
    public UI_ManagerBase GetUIManager()
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
