using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGraphicController : MonoBehaviour, IGraphic
{
    [SerializeField]
    private GameObject graphicModel;
    [SerializeField]
    private GameObject aimObject;

    /// <summary>
    /// Funzione che abilita/disabilita il modello per il tempo passato come parametro
    /// </summary>
    /// <param name="_duration"></param>
    public void Blink(float _duration)
    {
        StartCoroutine(BlinkCoroutine(_duration));
    }
    /// <summary>
    /// Coroutine che 
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    private IEnumerator BlinkCoroutine(float _duration)
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

    /// <summary>
    /// Funzione che disabilita il modello
    /// </summary>
    public void Disable()
    {
        graphicModel.SetActive(false);
    }

    /// <summary>
    /// Funzione che abilita il modello
    /// </summary>
    public void Enable()
    {
        graphicModel.SetActive(true);
    }

    /// <summary>
    /// Funzione che ritorna il modello
    /// </summary>
    /// <returns></returns>
    public GameObject GetModel()
    {
        return graphicModel;
    }

    /// <summary>
    /// Funzione che ritorna l'aim object
    /// </summary>
    /// <returns></returns>
    public GameObject GetAimObject()
    {
        return aimObject;
    }
}
