using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatingPlatform : Platform
{

    [Header("Rotate Options")]
    [SerializeField]
    private float angle;
    [SerializeField]
    private float rotateTime;
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
        int _direction = 1;
        float _targetAngle = angle + transform.rotation.z;

        while(isActive)
        {
            yield return transform.DORotateQuaternion(Quaternion.Euler(0.0f, 0.0f, _direction * _targetAngle), rotateTime).SetEase(Ease.Linear).WaitForCompletion();
            _direction = _direction * -1;
            yield return new WaitForSeconds(timeBetweenRotate);
        }
    }
    #endregion

}
