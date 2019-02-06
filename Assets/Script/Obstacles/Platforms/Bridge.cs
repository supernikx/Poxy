using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Bridge : Platform
{
    [Header("Movement Options")]
    [SerializeField]
    private float movingSpd;

    private BridgeBody bridgeBody;

    #region API
    public override void Init()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Pause();
        sequence.Append(transform.DORotate(new Vector3(0, 0, -90), movingSpd).SetSpeedBased().SetEase(Ease.Linear));
        sequence.Append(transform.DORotate(new Vector3(0, 0, 0), movingSpd).SetSpeedBased().SetEase(Ease.Linear));

        bridgeBody = GetComponentInChildren<BridgeBody>();
        if (bridgeBody != null)
            bridgeBody.Init(sequence);
                
        sequence.Play().SetLoops(-1);
    }
    #endregion
    
}
