using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SimpleToken : BaseToken
{
    [Header("Graphics Settings")]
    [SerializeField]
    private GameObject graphic;
    [SerializeField]
    private float maxbrightness;
    [SerializeField]
    private float minbrightness;

    private GeneralSoundController sfxCtrl;
    new private Renderer renderer;
    IEnumerator blinkRoutine;
    private bool isActive;

    #region API
    public override void Init()
    {
        sfxCtrl = GetComponentInChildren<GeneralSoundController>();
        renderer = GetComponentInChildren<Renderer>();
        Setup();
    }

    public override void Setup()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = BlinkCoroutine();
        StartCoroutine(blinkRoutine);

        isActive = true;
        gameObject.SetActive(isActive);
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity")))
        {
            isActive = false;
            GetToken(this);
            StartCoroutine(PickupRoutine());
        }
    }

    private IEnumerator PickupRoutine()
    {
        graphic.SetActive(false);
        sfxCtrl.PlayClip();
        yield return new WaitForSeconds(1f);
        graphic.SetActive(true);
        gameObject.SetActive(false);
    }

    private IEnumerator BlinkCoroutine()
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

    private void OnDisable()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
    }

    private void OnDestroy()
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
    }
}
