using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerGraphicController : MonoBehaviour, IGraphic
{
    [Header("Graphics options")]
    [SerializeField]
    private float changeGraphicPercentage;
    [SerializeField]

    [Header("Calm Graphic")]
    private GameObject calmGraphic;
    [SerializeField]
    private Avatar calmAvatar;

    [Header("Angry Graphics")]
    [SerializeField]
    private GameObject angryGraphic;
    [SerializeField]
    private Avatar angryAvatar;

    private GameObject activeGraphicModel;
    private Animator anim;

    #region API
    public void Init()
    {
        PlayerHealthController.OnHealthChange += HandleOnHealthChange;

        anim = GetComponent<Animator>();
        activeGraphicModel = calmGraphic;
        anim.avatar = calmAvatar;
    }

    /// <summary>
    /// Funzione che abilita/disabilita il modello per il tempo passato come parametro
    /// </summary>
    /// <param name="_duration"></param>
    public void Blink(float _duration)
    {
        StartCoroutine(CBlinkCoroutine(_duration));
    }

    /// <summary>
    /// Funzione che disabilita il modello
    /// </summary>
    public void Disable()
    {
        activeGraphicModel.SetActive(false);
    }

    /// <summary>
    /// Funzione che abilita il modello
    /// </summary>
    public void Enable()
    {
        activeGraphicModel.SetActive(true);
    }

    /// <summary>
    /// TEMPORANEA
    /// Funaione che scala la dimensione di entrambi i modelli
    /// </summary>
    public void Scale(float _scaleTo, float _time)
    {
        angryGraphic.transform.DOScale(_scaleTo, _time);
        calmGraphic.transform.DOScale(_scaleTo, _time);
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine che 
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    private IEnumerator CBlinkCoroutine(float _duration)
    {
        float timer = _duration;
        while (timer > 0)
        {
            Enable();
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            Disable();
            yield return new WaitForSeconds(0.2f);
            timer -= 0.2f;
        }
        Enable();
    }
    #endregion

    #region Handlers
    private void HandleOnHealthChange(float _health)
    {
        if (_health > changeGraphicPercentage && !calmGraphic.activeInHierarchy)
        {
            activeGraphicModel = calmGraphic;
            anim.avatar = calmAvatar;
            if (angryGraphic.activeInHierarchy)
            {
                angryGraphic.SetActive(false);
                calmGraphic.SetActive(true);
            }
        }
        else if (_health <= changeGraphicPercentage && !angryGraphic.activeInHierarchy)
        {
            activeGraphicModel = angryGraphic;
            anim.avatar = angryAvatar;
            if (calmGraphic.activeInHierarchy)
            {
                calmGraphic.SetActive(false);
                angryGraphic.SetActive(true);
            }
        }
    }
    #endregion

    #region Getters
    /// <summary>
    /// Funzione che ritorna il modello
    /// </summary>
    /// <returns></returns>
    public GameObject GetModel()
    {
        return activeGraphicModel;
    }
    #endregion
}
