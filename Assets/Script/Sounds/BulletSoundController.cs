using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSoundController : SoundControllerBase
{
    [Header("Bullet sound settings")]
    [SerializeField]
    private AudioClipStruct shot;
    [SerializeField]
    private AudioClipStruct hit;

    public void Shot()
    {
        PlayAudioClip(shot);
    }

    public void Hit()
    {
        PlayAudioClip(hit);
    }
}
