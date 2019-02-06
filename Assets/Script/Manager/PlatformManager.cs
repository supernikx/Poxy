using UnityEngine;
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

    private List<IPlatform> platforms = new List<IPlatform>();
    private List<LaunchingPlatform> launchingPlatforms = new List<LaunchingPlatform>();

    #region API
    public void Init()
    {
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

    public LaunchingPlatform GetNearestLaunchingPlatform(Transform _pointTransform, float _range)
    {
        return null;
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

