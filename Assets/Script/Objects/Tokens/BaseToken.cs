using UnityEngine;
using System.Collections;

public abstract class BaseToken : MonoBehaviour
{
    #region Delegates
    public delegate void TokenEvent(BaseToken _token);
    public TokenEvent GetToken;

    #endregion

    #region API
    public abstract void Init();
    public abstract void Setup();
    #endregion

}
