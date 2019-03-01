﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorsButtonsManager : MonoBehaviour
{

    [Header("Element Containers")]
    [SerializeField]
    private Transform doorsContainer;
    [SerializeField]
    private Transform buttonsContainer;

    private List<IDoor> doors = new List<IDoor>();
    private List<IButton> buttons = new List<IButton>();

    #region API
    public void Init(TokenManager _tokenMng)
    {
        if (!doorsContainer || !buttonsContainer)
            return;
        for (int i = 0; i < doorsContainer.childCount; i++)
        {
            IDoor _current = doorsContainer.GetChild(i).GetComponent<IDoor>();
            if (_current != null)
            {
                _current.Init();
                doors.Add(_current);
            }
        }

        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            IButton _current = buttonsContainer.GetChild(i).GetComponent<IButton>();
            if (_current != null)
            {
                _current.Init();
                buttons.Add(_current);
            }
        }

        _tokenMng.FinishToken += HandleFinishToken;
        LevelManager.OnPlayerDeath += HandlePlayerDeath;
    }
    #endregion

    #region Handlers
    private void HandleFinishToken()
    {
        foreach (IDoor _current in doors)
        {
            if (_current is TokenDoor)
            {
                _current.Activate();
            }
        }
    }

    private void HandlePlayerDeath()
    {
        foreach (IButton _current in buttons)
        {
            _current.Setup();
        }

        foreach (IDoor _current in doors)
        {
            _current.Setup();
        }
    }
    #endregion
}
