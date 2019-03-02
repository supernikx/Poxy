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
        tweening = false;
    }

    bool tweening;
    private void Update()
    {
        if (!tweening)
            transform.DOShakePosition(1.5f, 0.15f, 3, 90f, false, false).OnPlay(() => tweening = true).OnComplete(() => tweening = false);
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
