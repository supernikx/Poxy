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
    float defaultSpeed;
    [SerializeField]
    DistanceSettings distanceSetting;

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
        poop.Setup();

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
        poop.ResetEffect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (!isMoving)
            {
                movingRoutine = MovingRoutine(other.gameObject);
                StartCoroutine(movingRoutine);
            }
        }
    }

    private IEnumerator MovingRoutine(GameObject _player)
    {
        poop.StartEffect();
        isMoving = true;

        Tweener movementTween = poop.transform.DOMoveX(poopReachPosition.position.x, defaultSpeed).SetEase(Ease.Linear).SetSpeedBased();
        yield return movementTween
            .OnUpdate(() =>
            {
                float distance = Mathf.Abs(poop.transform.position.x - _player.transform.position.x);

                if (!distanceSetting.GetIsActive() && distance > distanceSetting.distance)
                {
                    movementTween.ChangeStartValue(poop.transform.position, distanceSetting.speed).SetSpeedBased();
                    distanceSetting.SetActive(true);
                }
                else if (distanceSetting.GetIsActive() && distance < distanceSetting.distance)
                {
                    movementTween.ChangeStartValue(poop.transform.position, defaultSpeed).SetSpeedBased();
                    distanceSetting.SetActive(false);
                }
            })
            .WaitForCompletion();

        yield return poop.transform.DOMoveY(fallingpoint.position.y, fallingSpeed)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .WaitForCompletion();

        poop.transform.position = startPosition;
        poop.ResetEffect();
        isMoving = false;
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandlePlayerDeath;
    }

    [Serializable]
    private class DistanceSettings
    {
        public float distance;
        public float speed;

        private bool active = false;
        public void SetActive(bool _active)
        {
            active = _active;
        }

        public bool GetIsActive()
        {
            return active;
        }
    }
}
