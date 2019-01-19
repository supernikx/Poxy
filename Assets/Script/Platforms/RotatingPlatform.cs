using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatingPlatform : Platform
{

    [Header("Rotate Options")]
    [SerializeField]
    private float angle;
    [SerializeField]
    private float rotationTime;
    [SerializeField]
    private float timeBetweenRotate;

    private bool isActive = false;

    #region API
    public override void Init()
    {
        isActive = true;
        StartCoroutine(CRotate());
    }
    #endregion

    #region Coroutines 
    private IEnumerator CRotate()
    {
        while(isActive)
        {
            yield return transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z + angle), rotationTime).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(timeBetweenRotate);
        }
    }
    #endregion

}
