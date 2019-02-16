using UnityEngine;
using System.Collections;

public class SimpleToken : BaseToken
{
    #region API
    public override void Init()
    {

    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GetToken(this);
            gameObject.SetActive(false);
        }
    }
}
