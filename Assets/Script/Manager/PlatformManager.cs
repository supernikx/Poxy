﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Questa Classe potrebbe essere utile per savlvare tutto riguardo a piattaforme, semmai ce ne fosse bisogno
/// </summary>

public class PlatformManager : MonoBehaviour
{

    [Header("Platforms Container")]
    [SerializeField]
    private Transform PlatformContainer;

    private List<IPlatform> platforms;
    private List<LaunchingPlatform> launchingPlatforms;
    private List<LaunchingPlatform> launchingPlatformsInUse;

    #region API
    public void Init()
    {
        platforms = new List<IPlatform>();
        launchingPlatforms = new List<LaunchingPlatform>();
        launchingPlatformsInUse = new List<LaunchingPlatform>();

        for (int i = 0; i < PlatformContainer.childCount; i++)
        {
            IPlatform _current = PlatformContainer.GetChild(i).GetComponent<IPlatform>();
            if (_current != null)
            {
                platforms.Add(_current);
                _current.Init();

                if (_current is LaunchingPlatform)
                    launchingPlatforms.Add(_current as LaunchingPlatform);
            }
        }
    }

    public IControllable GetNearestLaunchingPlatform(Transform _pointTransform, float _range)
    {
        if (launchingPlatforms.Count <= 0 || launchingPlatforms == null)
            return null;

        IControllable target = null;
        float minDistance = -1;
        foreach (IControllable _current in launchingPlatforms)
        {
            float distance = Vector3.Distance(_pointTransform.position, _current.gameObject.transform.position);
            if (_range >= distance && !launchingPlatformsInUse.Contains(_current as LaunchingPlatform))
            {
                if (minDistance == -1 || distance < minDistance)
                {
                    minDistance = distance;
                    target = _current;
                }
            }
        }
        return target;
    }
    #endregion


}

public abstract class Platform : MonoBehaviour, IPlatform
{
    public abstract void Init();
}

public interface IPlatform
{
    void Init();
}

