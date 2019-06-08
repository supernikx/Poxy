using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioMixerGroup musicOutput;
    [SerializeField]
    private AudioMixerGroup sfxOutput;

    public void Init()
    {

    }

    /// <summary>
    /// Funzione che imposta il volume della musica con il valore passato come parametro
    /// </summary>
    /// <param name="_volume"></param>
    public void SetMusicVolume(float _volume)
    {
        audioMixer.SetFloat("MusicVolume", _volume);
    }

    /// <summary>
    /// Funzione che imposta il volume degli sfx con il valore passato come parametro
    /// </summary>
    /// <param name="_volume"></param>
    public void SetSFXVolume(float _volume)
    {
        audioMixer.SetFloat("SFXVolume", _volume);
    }

    public AudioMixerGroup GetOutputGroup(SoundOutput _output)
    {
        switch (_output)
        {
            case SoundOutput.Music:
                return musicOutput;
            case SoundOutput.SFX:
                return sfxOutput;
        }

        return null;
    }
}
