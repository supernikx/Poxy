using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class FallingPlatform : PlatformBase
{

    [Header("Platform Options")]
    [SerializeField]
    private bool canRespawn;
    [SerializeField]
    private float respawnTime;
    [SerializeField]
    private float fallTime;
    [SerializeField]
    private float shakeTime;
    [SerializeField]
    private TriggerOption triggerOption;
    [SerializeField]
    private GameObject graphic;

    [Header("Collisions")]
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int verticalRayCount = 4;
    [SerializeField]
    private LayerMask playerLayer;
    /// <summary>
    /// Spazio tra un raycast verticale e l'altro
    /// </summary>
    private float verticalRaySpacing;
    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;
    /// <summary>
    /// Referenza agli start point
    /// </summary>
    private RaycastStartPoints raycastStartPoints;

    private Collider collider;

    private FallingTrigger fallingTrigger;

    private Vector3 startingPosition;

    private Coroutine respawnCoroutine = null;

    private bool isActive;
    /// <summary>
    /// Indica se nel precedente frame il player era a contatto con la piattaforma
    /// </summary>
    private bool previousContactState;
    /// <summary>
    /// Indica se nel frame corrente il player è a contatto con la piattaforma
    /// </summary>
    private bool currentContactState;

    private void Respawn()
    {
        graphic.SetActive(true);
        collider.enabled = true;
        currentContactState = false;
        previousContactState = false;
        transform.position = startingPosition;
        isActive = true;
    }

    #region API
    public override void Init()
    {
        collider = GetComponent<Collider>();
        previousContactState = false;
        currentContactState = true;
        startingPosition = transform.position;
        fallingTrigger = GetComponentInChildren<FallingTrigger>();

        if (triggerOption == TriggerOption.BeforeTouch)
        {
            fallingTrigger.FallTriggerEvent += HandleFallTriggerEvent;
        }
        else if (triggerOption == TriggerOption.AfterTouch)
        {
            fallingTrigger.gameObject.SetActive(false);

            CalculateRaySpacing();
            StartCoroutine(CCollisionCheck());
        }

        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;

        isActive = true;
    }
    #endregion

    #region Handlers
    private void HandleFallTriggerEvent()
    {
        if (isActive)
        {
            isActive = false;

            StartCoroutine(FallCoroutine());
        }
    }

    private void HandleOnPlayerDeath()
    {
        if (respawnCoroutine != null)
            StopCoroutine(respawnCoroutine);
        Respawn();
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

    private IEnumerator CCollisionCheck()
    {
        while (true)
        {
            if (isActive)
            {
                UpdateRaycastOrigins();

                currentContactState = false;
                VerticalCollisions();

                if (!currentContactState && currentContactState != previousContactState)
                {
                    HandleFallTriggerEvent();
                }
                else
                {
                    previousContactState = currentContactState;
                }
            }

            yield return null;
        }
    }

    private IEnumerator FallCoroutine()
    {
        Vector3 shakeStrenght = Vector3.zero;
        shakeStrenght.x = 0.6f;
        yield return new WaitForSeconds(fallTime - shakeTime);
        yield return graphic.transform.DOShakePosition(shakeTime, shakeStrenght, 150).WaitForCompletion();
        collider.enabled = false;
        yield return transform.DOMoveY(transform.position.y - 5f, 0.5f).SetEase(Ease.Linear).WaitForCompletion();
        graphic.SetActive(false);
        respawnCoroutine = StartCoroutine(CRespawn());
    }
    #endregion

    #region Collision
    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(collisionOffset * -2);

        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }

    /// <summary>
    /// Funzione che calcola i 2 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastStartPoints.upperLeft = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        raycastStartPoints.upperRight = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);

    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi verticali
    /// </summary>
    private void VerticalCollisions()
    {
        //Rileva la direzione in cui si sta andando
        float directionY = 1;
        //Determina la lunghezza del raycast
        float rayLenght = 0.3f;

        //Cicla tutti i punti da cui deve partire un raycast sull'asse verticale
        for (int i = 0; i < verticalRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = raycastStartPoints.upperLeft;
            rayOrigin += transform.right * (verticalRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, transform.up * directionY);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLenght, playerLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                rayLenght = hit.distance;

                Player _player;
                if (hit.transform.gameObject.tag == "Player")
                    _player = hit.transform.gameObject.GetComponent<Player>();
                else
                    _player = hit.transform.gameObject.GetComponentInParent<Player>();

                currentContactState = currentContactState || (_player != null);
            }
            Debug.DrawRay(rayOrigin, transform.up * directionY * rayLenght, Color.red);
        }
    }
    #endregion

    #region Classes
    public enum TriggerOption
    {
        BeforeTouch,
        AfterTouch,
    }

    private struct RaycastStartPoints
    {
        public Vector3 upperLeft;
        public Vector3 upperRight;
    }
    #endregion

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandleOnPlayerDeath;

        if (triggerOption == TriggerOption.BeforeTouch)
        {
            fallingTrigger.FallTriggerEvent -= HandleFallTriggerEvent;
        }
    }
}

