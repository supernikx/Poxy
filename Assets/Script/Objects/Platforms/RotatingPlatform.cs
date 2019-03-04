using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatingPlatform : Platform
{

    [Header("Rotate Settings")]
    [SerializeField]
    private float angleEachRotation;
    [SerializeField]
    private float rotatingSpeed;
    [SerializeField]
    private float timeBetweenRotate;

    private PlatformCollisionController collisionCtrl;
    private bool isActive;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        rotatingSpeed = (angleEachRotation < 0) ? -rotatingSpeed : rotatingSpeed;

        isActive = true;
        StartCoroutine(CRotate());
    }
    #endregion

    #region Coroutines 
    private IEnumerator CRotate()
    {
        while (isActive)
        {
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + angleEachRotation));
            Vector3 rotatingVelocity;
            Vector3 rotatingVector = new Vector3(0, 0, rotatingSpeed);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
            {
                rotatingVelocity = new Vector3(transform.up.x * 2f, Mathf.Abs(rotatingSpeed) / 4, 0f) * Time.deltaTime;
                collisionCtrl.MovePassengerRotating(rotatingVelocity);
                transform.Rotate(rotatingVector * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            transform.rotation = targetRotation;
            yield return new WaitForSeconds(timeBetweenRotate);
        }
    }
    #endregion

}
