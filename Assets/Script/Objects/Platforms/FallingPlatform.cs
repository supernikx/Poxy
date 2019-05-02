using UnityEngine;
using System;
using System.Collections;

public class FallingPlatform : PlatformBase
{

    [Header("Platform Options")]
    [SerializeField]
    private bool canRespawn;
    [SerializeField]
    private float respawnTime;
    [SerializeField]
    private TriggerOption triggerOption;
    [SerializeField]
    private GameObject graphic;
    private Collider collider;

    private FallingTrigger fallingTrigger;
    
    private Vector3 startingPosition;

    private bool isActive;

    private void Respawn()
    {
        graphic.SetActive(true);
        collider.enabled = true;
        isActive = true;
    }

    #region API
    public override void Init()
    {
        collider = GetComponent<Collider>();

        if (triggerOption == TriggerOption.BeforeTouch)
        {
            fallingTrigger = GetComponentInChildren<FallingTrigger>();
            fallingTrigger.FallTriggerEvent += HandleFallTriggerEvent;
        }

        isActive = true;
    }
    #endregion

    #region Handlers
    private void HandleFallTriggerEvent()
    {
        if (isActive)
        {
            isActive = false;
            graphic.SetActive(false);
            collider.enabled = false;
            StartCoroutine(CRespawn());
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CRespawn()
    {
        if (canRespawn)
        {
            float timer = 0;
            while (timer <= respawnTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Respawn();
        }
    }
    #endregion

    private void OnCollisionExit(Collision collision)
    {
        if (isActive && collision.gameObject.tag == "Player" && triggerOption == TriggerOption.AfterTouch)
        {
            isActive = false;
            graphic.SetActive(false);
            StartCoroutine(CRespawn());
        }
    }

}

public enum TriggerOption
{
    BeforeTouch,
    AfterTouch,
}
