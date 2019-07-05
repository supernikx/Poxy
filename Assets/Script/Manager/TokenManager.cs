using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TokenManager : MonoBehaviour
{
    #region Delegates
    public Action FinishToken;
    public Action<int> OnGainLife;
    public static Action<int> OnTokenTaken;
    #endregion

    [Header("Token Mng Options")]
    [SerializeField]
    private Transform tokenContainer;
    [SerializeField]
    private int tokenForGainLife;
    [SerializeField]
    private int livesGain;


    private List<BaseToken> tokens = new List<BaseToken>();

    [Header("Debug")]
    [SerializeField]
    private int tokenCounter;

    #region API
    public void Init(SpeedrunManager _speedRun)
    {
        if (!tokenContainer)
            return;
        for (int i = 0; i < tokenContainer.childCount; i++)
        {
            BaseToken _current = tokenContainer.GetChild(i).GetComponent<BaseToken>();

            if (_speedRun.GetIsActive())
            {
                _current.gameObject.SetActive(false);
            }
            else
            {
                _current.Init();
                tokens.Add(_current);
                _current.GetToken += HandleGetToken;
            }
        }

        tokenCounter = 0;
    }

    public int GetTokensCount()
    {
        return tokenCounter;
    }
    #endregion

    #region Handlers
    private void HandleGetToken(BaseToken _token)
    {
        tokenCounter++;

        if (OnTokenTaken != null)
            OnTokenTaken(tokenCounter);

        if (tokenCounter % tokenForGainLife == 0 && OnGainLife != null)
            OnGainLife(livesGain);

        if (tokenCounter == tokens.Count)
        {
            if (FinishToken != null)
                FinishToken();
        }
    }

    private void HandlePlayerDeath()
    {
        foreach (BaseToken _current in tokens)
        {
            _current.Setup();
        }

        tokenCounter = 0;
    }
    #endregion
}
