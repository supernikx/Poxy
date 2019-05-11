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

    private void Update()
    {
        MovePlatforms();
    }

    #region API
    /// <summary>
    /// Funzione che inizializza il platform Manager
    /// </summary>
    /// <param name="_uiGameplay"></param>
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
                _current.Init();

                if (_current is LaunchingPlatform)
                    launchingPlatforms.Add(_current as LaunchingPlatform);
                else
                    platforms.Add(_current);
            }
        }

        OnParasite += HandleOnParasite;
        OnParasiteEnd += HandleOnParasiteEnd;

        SetCanMove(true);
    }

    /// <summary>
    /// Funzione che ritorna la prima launching platform in range (se ce ne sono)
    /// altrimenti ritorna null
    /// </summary>
    /// <param name="_pointTransform"></param>
    /// <param name="_range"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Funzione che imposta la variabile canMove con il parametro passato
    /// </summary>
    /// <param name="_canMove"></param>
    public void SetCanMove(bool _canMove)
    {
        canMove = _canMove;
    }
    #endregion

    #region Handlers
    /// <summary>
    /// Funzione che gestisce l'evento OnParasite
    /// </summary>
    /// <param name="_platform"></param>
    private void HandleOnParasite(LaunchingPlatform _platform)
    {
        launchingPlatformsInUse.Add(_platform);
        uiGameplay.GetGamePanel().SetMaxToleranceValue(_platform.GetToleranceCtrl().GetMaxTolerance());
        uiGameplay.GetGamePanel().EnableToleranceBar(true);
    }

    /// <summary>
    /// Funzione che gestisce l'evento OnParasiteEnd
    /// </summary>
    /// <param name="_platform"></param>
    private void HandleOnParasiteEnd(LaunchingPlatform _platform)
    {
        launchingPlatformsInUse.Remove(_platform);
        _platform.gameObject.transform.parent = PlatformContainer;
        uiGameplay.GetGamePanel().EnableToleranceBar(false);
    }
    #endregion

    /// <summary>
    /// Funzione che muove le piattaforme che hanno un behaviour
    /// </summary>
    private bool canMove;
    private void MovePlatforms()
    {
        if (!canMove)
            return;

        foreach (IPlatform platform in platforms)
        {
            platform.MoveBehaviour();
        }
    }

    private void OnDisable()
    {
        OnParasite -= HandleOnParasite;
        OnParasiteEnd -= HandleOnParasiteEnd;
    }
}

