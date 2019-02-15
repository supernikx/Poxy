using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatingPlatform : Platform
{

    [Header("Rotate Options")]
    [SerializeField]
    private float angle;
    private float startingAngle;
    [SerializeField]
    private float rotatingSpeed;
    [SerializeField]
    private float timeBetweenRotate;

    private PlatformCollisionController collisionCtrl;
    private bool isActive = false;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        isActive = true;
        startingAngle = transform.eulerAngles.z;
        StartCoroutine(CRotate());
    }
    #endregion

    #region Coroutines 
    private IEnumerator CRotate()
    {
        while (isActive)
        {
            //yield return transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z + angle), rotationTime).SetEase(Ease.Linear).WaitForCompletion();
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + angle));
            Vector3 rotatingVelocity;
            Vector3 rotatingVector = new Vector3(0, 0, rotatingSpeed);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0f)
            {
                rotatingVelocity = new Vector3(transform.up.x * 2f, rotatingSpeed / 4f, 0f) * Time.deltaTime;
                collisionCtrl.MovePassenger(rotatingVelocity);
                transform.Rotate(rotatingVector * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(timeBetweenRotate);
        }
    }
    #endregion

}
