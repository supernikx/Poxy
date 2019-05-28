using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorsButtonsManager : MonoBehaviour
{

    [Header("Element Containers")]
    [SerializeField]
    private Transform doorsContainer;
    [SerializeField]
    private Transform buttonsContainer;
    private TokenManager tokenMng;
    private List<IActivable> doors = new List<IActivable>();
    private List<IButton> buttons = new List<IButton>();

    #region API
    public void Init()
    {
        if (!doorsContainer || !buttonsContainer)
            return;

        //tokenMng = _tokenMng;

        for (int i = 0; i < doorsContainer.childCount; i++)
        {
            IActivable _current = doorsContainer.GetChild(i).GetComponent<IActivable>();
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

        //tokenMng.FinishToken += HandleFinishToken;
        LevelManager.OnPlayerDeath += HandlePlayerDeath;
    }
    #endregion

    #region Handlers
    private void HandleFinishToken()
    {
        foreach (IActivable _current in doors)
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

        foreach (IActivable _current in doors)
        {
            _current.Setup();
        }
    }
    #endregion

    private void OnDestroy()
    {
        //tokenMng.FinishToken -= HandleFinishToken;
        LevelManager.OnPlayerDeath -= HandlePlayerDeath;
    }
}
