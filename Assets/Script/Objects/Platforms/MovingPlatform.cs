using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof (PlatformCollisionController))]
public class MovingPlatform : Platform
{
    [Header("Movement Options")]
    [SerializeField]
    private float movingSpd;
    [SerializeField]
    private Transform reachPoint;
    [SerializeField]
    private float waitTime = 0;
    Vector3 reachPosition;
    private Vector3 startPosition;
    private Vector3 direction;
    bool setupped = false;
    bool canMove = false;
    private PlatformCollisionController collisionCtrl;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        startPosition = transform.position;
        reachPosition = reachPoint.position;
        direction = (reachPosition - startPosition).normalized;
        pointReached = false;
        setupped = true;
        canMove = true;
    }

    private void Update()
    {
        if (!setupped || !canMove)
            return;

        //Calcolo il movimento della piattaforma e dei passeggeri
        CalculatePlatformMovement();
        collisionCtrl.CalculatePassengerMovement(moveVelocity);
        collisionCtrl.CalculateAlwaysMovement(moveVelocity);

        //Muovo i passeggeri che devono essere mossi prima della piattaforma
        collisionCtrl.MovePassengers(true);

        //Muovo la piattaforma
        MovePlatform();

        //Muovo i passeggeri che devono essere mossi dopo la piattaforma
        collisionCtrl.MovePassengers(false);
    }

    #endregion

    /// <summary>
    /// Funzione che calcola la direzione in cui deve andare la piattaforma
    /// </summary>
    Vector3 moveVelocity;
    bool pointReached;
    private void CalculatePlatformMovement()
    {
        if (!pointReached && Vector3.Distance(transform.position, reachPosition) > 0.1f)
        {
            moveVelocity = direction * movingSpd * Time.deltaTime;
        }
        else
        {
            if (!pointReached)
            {
                pointReached = true;
                StartCoroutine(WaitTime(waitTime));
            }

            moveVelocity = -direction * movingSpd * Time.deltaTime;

            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                pointReached = false;
                StartCoroutine(WaitTime(waitTime));
            }
        }
    }

    /// <summary>
    /// Funzione che muove la piattaforma
    /// </summary>
    private void MovePlatform()
    {
        transform.Translate(moveVelocity);
    }

    /// <summary>
    /// Funzione che disabilità il movimento per il tempo passato come parametro
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator WaitTime(float _time)
    {
        canMove = false;
        yield return new WaitForSeconds(_time);
        canMove = true;
    }
}