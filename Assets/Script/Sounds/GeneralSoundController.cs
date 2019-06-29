using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundController : SoundControllerBase
{
    [Header("General Settings")]
    [SerializeField]
    private AudioClipStruct clip;

    private void OnEnable()
    {
        LevelManager.OnPlayerEndLevel += HandlePlayerEndLevel;
    }

    private void HandlePlayerEndLevel()
    {
        StopClip();
    }

    public void PlayClip()
    {
        PlayAudioClip(clip);
    }

    public void StopClip()
    {
        StopAudioClips();
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerEndLevel -= HandlePlayerEndLevel;
    }
}
