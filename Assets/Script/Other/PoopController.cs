using UnityEngine;
using DG.Tweening;
using System;

public class PoopController : MonoBehaviour
{
    [SerializeField]
    Poop poop;
    [SerializeField]
    Transform poopReachPosition;
    [SerializeField]
    float poopSpeed;

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
        poop.transform.DOKill();
        poop.transform.position = startPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving)
        {
            poop.transform.DOMoveX(poop.transform.position.x + poopReachPosition.position.x, poopSpeed).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
            {
                poop.transform.position = startPosition;
                isMoving = false;
            });

            isMoving = true;
        }
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandlePlayerDeath;
    }
}
