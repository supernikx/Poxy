using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SimpleToken : BaseToken
{
    private bool isActive;

    #region API
    public override void Init()
    {
        Setup();
    }

    public override void Setup()
    {
        isActive = true;
        gameObject.SetActive(isActive);
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            GetToken(this);
            gameObject.SetActive(false);
        }
    }
}
