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
    private float startAngle;
    private Vector3 rotatingVector;

    private PlatformCollisionController collisionCtrl;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        rotatingVector = new Vector3(0, 0, -rotatingSpeed);
        startAngle = transform.eulerAngles.z;
        angleReached = false;
        sticky = false;
        StartCoroutine(Rotate());
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
    bool sticky;
    bool angleReached;
    private IEnumerator Rotate()
    {
        Vector3 rotatingVelocity;
        while (true)
        {
            if (!sticky)
            {
                if (!angleReached && transform.eulerAngles.z > maxAngle)
                {
                    rotatingVelocity = new Vector3(transform.up.x * 2f, rotatingSpeed / 4f, 0f) * Time.deltaTime;
                    collisionCtrl.MovePassenger(rotatingVelocity);
                    transform.Rotate(rotatingVector * Time.deltaTime);
                }
                else
                {
                    if (!angleReached)
                        angleReached = true;

                    rotatingVelocity = new Vector3(transform.up.x * 2f, rotatingSpeed / 4f, 0f) * Time.deltaTime;
                    collisionCtrl.MovePassenger(rotatingVelocity);
                    transform.Rotate(-rotatingVector * Time.deltaTime);

                    if (transform.eulerAngles.z > startAngle)
                    {
                        angleReached = false;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
