using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerGraphicController : MonoBehaviour, IGraphic
{
    #region Delegates
    public Action OnModelChanged;
    #endregion

    [Header("Graphics options")]
    [SerializeField]
    private float changeGraphicPercentage;
    [SerializeField]

    [Header("Calm Graphic")]
    private GameObject calmGraphic;
    [SerializeField]
    private Avatar calmAvatar;
    [SerializeField]
    private GameObject calmAimObject;

    [Header("Angry Graphics")]
    [SerializeField]
    private GameObject angryGraphic;
    [SerializeField]
    private Avatar angryAvatar;
    [SerializeField]
    private GameObject angryAimObject;

    private GameObject _activeGraphicModel;
    private GameObject activeGraphicModel
    {
        set
        {
            _activeGraphicModel = value;
            if (OnModelChanged != null)
                OnModelChanged();
        }
        get
        {
            return _activeGraphicModel;
        }
    }
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
    /// <summary>
    /// Funzione che gestisce l'evento PlayerHealthController.OnHealthChange
    /// </summary>
    /// <param name="_health"></param>
    private void HandleOnHealthChange(float _health)
    {
        if (_health > changeGraphicPercentage && activeGraphicModel != calmGraphic)
        {
            activeGraphicModel = calmGraphic;
            if (angryGraphic.activeSelf)
            {
                angryGraphic.SetActive(false);
                calmGraphic.SetActive(true);
            }

            calmGraphic.transform.localScale = angryGraphic.transform.localScale;
            calmGraphic.transform.SetAsFirstSibling();
            anim.avatar = calmAvatar;
        }
        else if (_health <= changeGraphicPercentage && activeGraphicModel != angryGraphic)
        {
            activeGraphicModel = angryGraphic;
            if (calmGraphic.activeSelf)
            {
                calmGraphic.SetActive(false);
                angryGraphic.SetActive(true);
            }

            angryGraphic.transform.localScale = calmGraphic.transform.localScale;
            angryGraphic.transform.SetAsFirstSibling();
            anim.avatar = angryAvatar;
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

    /// <summary>
    /// Funzione che ritorna l'oggetto con cui si mira
    /// </summary>
    /// <returns></returns>
    public GameObject GetAimObject()
    {
        if (activeGraphicModel == calmGraphic)
            return calmAimObject;

        if (activeGraphicModel == angryGraphic)
            return angryAimObject;

        return null;
    }
    #endregion

    private void OnDisable()
    {
        PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
    }

    public void ChangeTexture(TextureType _type)
    {
        return;
    }
}
