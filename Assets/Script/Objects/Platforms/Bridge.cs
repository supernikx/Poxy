using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Bridge : Platform, ISticky
{
    [Header("Movement Options")]
    [SerializeField]
    private float rotatingSpeed;
    [SerializeField]
    private float maxAngle;

    private PlatformCollisionController collisionCtrl;
    private bool isActive;
    private bool sticky;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        rotatingSpeed = (maxAngle < 0)? -rotatingSpeed : rotatingSpeed;

        isActive = true;
        sticky = false;

        StartCoroutine(CRotate());
    }

    public void OnSticky(Direction _direction)
    {
        sticky = true;
    }

    public void OnStickyEnd()
    {
        sticky = false;
    }
    #endregion

    private IEnumerator CRotate()
    {
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + maxAngle));
        Vector3 rotatingVelocity;
        Vector3 rotatingVector = new Vector3(0, 0, rotatingSpeed);
        bool opening = true;
        while (isActive)
        {
            if (!sticky)
            {
                if (opening)
                {
                    rotatingVelocity = new Vector3(transform.up.x * 2f, Mathf.Abs(rotatingSpeed) / 4, 0f) * Time.deltaTime;
                    collisionCtrl.MovePassengerRotating(rotatingVelocity);
                    transform.Rotate(rotatingVector * Time.deltaTime);
                    if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                    {
                        transform.rotation = targetRotation;
                        rotatingVector = new Vector3(0, 0, -rotatingSpeed);
                        targetRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z - maxAngle));
                        opening = false;
                    }
                }
                else
                {
                    rotatingVelocity = new Vector3(transform.up.x * 2f, Mathf.Abs(rotatingSpeed) / 4, 0f) * Time.deltaTime;
                    collisionCtrl.MovePassengerRotating(rotatingVelocity);
                    transform.Rotate(rotatingVector * Time.deltaTime);
                    if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                    {
                        transform.rotation = targetRotation;
                        rotatingVector = new Vector3(0, 0, rotatingSpeed);
                        targetRotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z + maxAngle));
                        opening = true;
                    }
                }                
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
