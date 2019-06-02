using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

public class PoopController : MonoBehaviour
{
    [SerializeField]
    Poop poop;
    [Header("Movement Settings")]
    [SerializeField]
    Transform poopReachPosition;
    [SerializeField]
    float poopSpeed;

    [Header("Falling Settings")]
    [SerializeField]
    Transform fallingpoint;
    [SerializeField]
    float fallingSpeed;

    IEnumerator movingRoutine;
    Vector3 startPosition;
    bool isMoving;

    public void Init()
    {
        isMoving = false;
        startPosition = poop.transform.position;
        LevelManager.OnPlayerDeath += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        isMoving = false;
        if (movingRoutine != null)
            StopCoroutine(movingRoutine);
        poop.transform.DOKill();
        poop.transform.position = startPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (!isMoving)
            {
                movingRoutine = MovingRoutine();
                StartCoroutine(movingRoutine);
            }
        }
    }

    private IEnumerator MovingRoutine()
    {
        isMoving = true;

        yield return poop.transform.DOMoveX(poopReachPosition.position.x, poopSpeed).SetEase(Ease.Linear).SetSpeedBased().WaitForCompletion();
        yield return poop.transform.DOMoveY(fallingpoint.position.y, fallingSpeed).SetEase(Ease.Linear).SetSpeedBased().WaitForCompletion();

        poop.transform.position = startPosition;
        isMoving = false;
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandlePlayerDeath;
    }
}
