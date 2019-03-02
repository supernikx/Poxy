using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Questa Classe potrebbe essere utile per savlvare tutto riguardo a piattaforme, semmai ce ne fosse bisogno
/// </summary>

public class PlatformManager : MonoBehaviour
{
    #region Delegates
    public delegate void ParasiteEvent(LaunchingPlatform _platform);
    public static ParasiteEvent OnParasite;
    public static ParasiteEvent OnParasiteEnd;
    #endregion

    [Header("Platforms Container")]
    [SerializeField]
    private Transform PlatformContainer;

    private List<IPlatform> platforms;
    private List<LaunchingPlatform> launchingPlatforms;
    private List<LaunchingPlatform> launchingPlatformsInUse;

    private UI_GameplayManager uiGameplay;

    #region API
    public void Init(UI_GameplayManager _uiGameplay)
    {
        if (!PlatformContainer)
            return;
        platforms = new List<IPlatform>();
        launchingPlatforms = new List<LaunchingPlatform>();
        launchingPlatformsInUse = new List<LaunchingPlatform>();
        uiGameplay = _uiGameplay;
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

        OnParasite += HandleOnParasite;
        OnParasiteEnd += HandleOnParasiteEnd;
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
            LaunchingPlatform platform = _current as LaunchingPlatform;
            if (_range >= distance && !launchingPlatformsInUse.Contains(platform) && platform.IsActive())
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

    #region Handlers
    private void HandleOnParasite(LaunchingPlatform _platform)
    {
        launchingPlatformsInUse.Add(_platform);
        uiGameplay.GetGamePanel().EnableToleranceBar(true);
    }

    private void HandleOnParasiteEnd(LaunchingPlatform _platform)
    {
        launchingPlatformsInUse.Remove(_platform);
        _platform.gameObject.transform.parent = PlatformContainer;
        uiGameplay.GetGamePanel().EnableToleranceBar(false);
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

