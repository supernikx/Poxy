using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGraphic
{
    /// <summary>
    /// Referenza al gameObject
    /// </summary>
    GameObject gameObject { get; }

    /// <summary>
    /// Funzione che abilita il modello
    /// </summary>
    void Enable();

    /// <summary>
    /// Funzione che disabilita il modello
    /// </summary>
    void Disable();

    /// <summary>
    /// Funzione che abilita/disabilita il modello per il tempo passato come parametro
    /// </summary>
    /// <param name="_duration"></param>
    void Blink(float _duration);

    /// <summary>
    /// Funzione che ritorna l'oggetto con cui si mira
    /// </summary>
    /// <returns></returns>
    GameObject GetAimObject();

    /// <summary>
    /// Funzione che ritorna il modello
    /// </summary>
    /// <returns></returns>
    GameObject GetModel();

    /// <summary>
    /// Funzione che cambia la texture del modello
    /// </summary>
    /// <param name="_type"></param>
    void ChangeTexture(TextureType _type);
}

public enum TextureType
{
    Default,
    Parasite,
}
