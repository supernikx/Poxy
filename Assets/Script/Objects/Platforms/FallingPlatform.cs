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
    private float fallSpeed;
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

    [Header("Graphic Settings")]
    [SerializeField]
    private float minbrightness;
    private float maxbrightness;

    private new Renderer renderer;
    private new Collider collider;
    private GeneralSoundController soundCtrl;
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
        renderer = GetComponentInChildren<Renderer>();
        collider = GetComponent<Collider>();
        soundCtrl = GetComponentInChildren<GeneralSoundController>();

        maxbrightness = renderer.material.GetColor("_EmissiveColor").r;
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
        soundCtrl.PlayClip();
        Vector3 shakeStrenght = Vector3.zero;
        shakeStrenght.x = 0.6f;
        yield return new WaitForSeconds(fallTime - shakeTime);
        IEnumerator emissionRoutine = ChangeEmission();
        StartCoroutine(emissionRoutine);
        yield return graphic.transform.DOShakePosition(shakeTime, shakeStrenght, 150).WaitForCompletion();
        StopCoroutine(emissionRoutine);
        yield return transform.DOMoveY(transform.position.y - 5f, fallSpeed).SetSpeedBased().SetEase(Ease.Linear).WaitForCompletion();
        renderer.material.SetColor("_EmissiveColor", new Color(maxbrightness, maxbrightness, maxbrightness, 1f));
        collider.enabled = false;
        graphic.SetActive(false);
        respawnCoroutine = StartCoroutine(CRespawn());
    }

    /// <summary>
    /// Coroutine infinita che aumenta/diminuisce l'emissiva del materiale
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeEmission()
    {
        bool lampincreasbrightness = true;
        float actualbrightness = minbrightness;
        while (true)
        {
            if (lampincreasbrightness)
            {
                while (actualbrightness < maxbrightness)
                {
                    renderer.material.SetColor("_EmissiveColor", new Color(actualbrightness, actualbrightness, actualbrightness, 1f));
                    actualbrightness += 0.1f;
                    yield return new WaitForSecondsRealtime(0.01f);
                }
                lampincreasbrightness = false;
            }
            else
            {
                while (actualbrightness > minbrightness)
                {
                    renderer.material.SetColor("_EmissiveColor", new Color(actualbrightness, actualbrightness, actualbrightness, 1f));
                    actualbrightness -= 0.1f;
                    yield return new WaitForSecondsRealtime(0.01f);
                }
                lampincreasbrightness = true;
            }
        }
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

