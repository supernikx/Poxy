using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StickyObject : MonoBehaviour, IPoolObject
{
    #region IPoolObject
    public GameObject ownerObject
    {
        get
        {
            return _ownerObject;
        }
        set
        {
            _ownerObject = value;
        }
    }
    GameObject _ownerObject;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }

        set
        {
            _currentState = value;
        }
    }
    State _currentState;

    public event PoolManagerEvets.Events OnObjectSpawn;
    public event PoolManagerEvets.Events OnObjectDestroy;
    #endregion

    [Header("Stickyobject General Settings")]
    [SerializeField]
    private float duration;
    [Header("Stickyobject Range Settings")]
    [SerializeField]
    private float minRange;
    [SerializeField]
    private int checkSpaceRayCount = 4;
    private float checkSpaceRaySpacing;
    [SerializeField]
    private LayerMask stickyObjectCollisionLayer;
    [Header("Stickyobject Stuck Settings")]
    [SerializeField]
    private int checkStuckedObjectsRayCount = 4;
    private float checkStuckedObjectsRaySpacing;
    [SerializeField]
    private int rayRequiredStuckObject;
    [SerializeField]
    private LayerMask layersWhichCanBeStucked;

    private BoxCollider boxCollider;
    float maxDistance;
    float maxXScale;
    float yOffeset;
    #region API
    public void Setup()
    {
        boxCollider = GetComponent<BoxCollider>();
        Vector3 bottomLeftCorner = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.min.y, transform.position.z);
        Vector3 bottomRightCorner = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.min.y, transform.position.z);
        yOffeset = boxCollider.bounds.size.y * 0.5f;
        maxDistance = Vector3.Distance(bottomLeftCorner, bottomRightCorner);
        maxXScale = transform.localScale.x;
        checkSpaceRaySpacing = CalculateRaySpacing(checkSpaceRayCount);
    }

    public void Init(Vector3 _spawnPosition, Quaternion _rotation)
    {
        transform.position = _spawnPosition;
        transform.rotation = _rotation;
    }

    public void Spawn(Vector3 _rightPosition, Vector3 _leftPostion)
    {
        float actualDistance = Vector3.Distance(_rightPosition, _leftPostion);

        if (actualDistance < minRange)
        {
            if (OnObjectDestroy != null)
                OnObjectDestroy(this);
        }
        else
        {
            Vector3 newScale = new Vector3(actualDistance * maxXScale / maxDistance, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
            boxCollider.size.Scale(newScale);
            transform.position = Vector3.Lerp(_rightPosition, _leftPostion, 0.5f);
            if (OnObjectSpawn != null)
                OnObjectSpawn(this);

            StartCoroutine(DespawnCoroutine());
            StartCoroutine(CheckCollisionCoroutine(_rightPosition, _leftPostion));
        }
    }

    /// <summary>
    /// Funzione che calcola ritorna l'ultimo punto con cui si collide con il collision layer
    /// nella direzione passata come parametro
    /// </summary>
    /// <param name="_stickyObject"></param>
    /// <param name="_normal"></param>
    /// <param name="_direction"></param>
    /// <returns></returns>
    public Vector3 CheckSpace(Vector3 _normal, int _direction)
    {
        float rayLenght = 0.3f;
        Vector3 maxCenterPoint = boxCollider.bounds.center + transform.forward.normalized * yOffeset;
        Vector3 previewRayOrigin = maxCenterPoint;

        for (int i = 0; i < checkSpaceRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = maxCenterPoint;
            rayOrigin += transform.right * _direction * (checkSpaceRaySpacing * 0.5f * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, -_normal);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, stickyObjectCollisionLayer))
            {
                rayLenght = hit.distance;
                previewRayOrigin = rayOrigin;
            }
            else
            {
                break;
            }
        }

        return previewRayOrigin;
    }

    #region Getter
    public float GetDuration()
    {
        return duration;
    }

    public int GetRayCount()
    {
        return checkSpaceRayCount;
    }

    public LayerMask GetCollisionLayer()
    {
        return stickyObjectCollisionLayer;
    }

    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }
    #endregion
    #endregion

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast orrizontali con il valore di ray passato come parametro
    /// </summary>
    /// <param name="_rayCount"></param>
    /// <returns></returns>
    private float CalculateRaySpacing(float _rayCount)
    {
        if (boxCollider.bounds.size.x > boxCollider.bounds.size.y)
            return boxCollider.bounds.size.x / (_rayCount - 1);
        else
            return boxCollider.bounds.size.y / (_rayCount - 1);
    }

    /// <summary>
    /// Funzione che conta il tempo per far despawnare l'oggetto
    /// </summary>
    /// <returns></returns>
    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(duration);

        for (int i = 0; i < stickyList.Count; i++)
        {
            stickyList[i].GIStickyRef.OnStickyEnd();
        }
        stickyList.Clear();
        if (OnObjectDestroy != null)
            OnObjectDestroy(this);
    }

    /// <summary>
    /// Classe che contiene le informazioni degli oggetti che sono in contatto con l'oggetto sticky
    /// </summary>
    private class StickyCollisionsInfos
    {
        public GameObject Gobject;
        public ISticky GIStickyRef;
        public int StickyRay;
        public bool Sticky;

        public StickyCollisionsInfos(GameObject _Gobject, ISticky _GIStickyRef)
        {
            Gobject = _Gobject;
            GIStickyRef = _GIStickyRef;
            StickyRay = 0;
            Sticky = false;
        }
    }
    /// <summary>
    /// Funzione che controlla le collisioni con oggetti ISticky
    /// </summary>
    /// <param name="_rightPosition"></param>
    /// <param name="_leftPostion"></param>
    /// <returns></returns>
    private List<StickyCollisionsInfos> stickyList = new List<StickyCollisionsInfos>();
    private IEnumerator CheckCollisionCoroutine(Vector3 _rightPosition, Vector3 _leftPostion)
    {
        //Calcolo lo spazio tra un ray e l'altro
        checkStuckedObjectsRaySpacing = CalculateRaySpacing(checkStuckedObjectsRayCount);
        Vector3 rightPositionFixY = _rightPosition - transform.forward.normalized * yOffeset;

        while (CurrentState == State.InUse)
        {
            //Determina la lunghezza del raycast
            float rayLenght = 0.1f;
            //Ciclo tutti i ray
            for (int i = 0; i < checkStuckedObjectsRayCount; i++)
            {
                //Determina il punto da cui deve partire il ray
                Vector3 rayOrigin = rightPositionFixY;
                rayOrigin += transform.right * (checkStuckedObjectsRaySpacing * i);

                //Crea il ray
                Ray ray = new Ray(rayOrigin, transform.forward);
                RaycastHit hit;

                //Eseguo il raycast
                if (Physics.Raycast(ray, out hit, rayLenght, layersWhichCanBeStucked))
                {
                    //Se colpisco qualcosa che rientra nei layer stuck
                    GameObject objectHit = hit.transform.gameObject;
                    StickyCollisionsInfos stickyInfo = stickyList.FirstOrDefault(g => g.Gobject == objectHit);
                    //Controllo se è presente nella lista
                    if (stickyInfo == null)
                    {
                        //Se non è presente controllo che sia un oggetto ISticky
                        ISticky stickyRef = objectHit.GetComponent<ISticky>();
                        if (stickyRef != null)
                        {
                            //Se lo è creo un nuovo StickyCollisionsInfos con i dati del GameObject e lo aggiunto alla lista
                            stickyInfo = new StickyCollisionsInfos(objectHit, stickyRef);
                            stickyInfo.StickyRay++;
                            stickyList.Add(stickyInfo);
                        }
                    }
                    else
                    {
                        //Se è resente aumento i ray che lo colpiscono di uno
                        stickyInfo.StickyRay++;
                    }
                }
                Debug.DrawRay(rayOrigin, transform.forward * rayLenght, Color.black);
            }

            //Ciclo tutti gli oggetti con cui sono in collisione
            for (int i = stickyList.Count - 1; i >= 0; i--)
            {
                //Se i ray che colpiscono l'oggetto sono maggiori di quelli necessari e non è ancora incolllato
                if (stickyList[i].StickyRay >= rayRequiredStuckObject && !stickyList[i].Sticky)
                {
                    //Lo incollo chiamo la callback comunicando la direzione in cui si è bloccati
                    stickyList[i].Sticky = true;
                    stickyList[i].GIStickyRef.OnSticky(GetHitDirection());
                }
                //Se i ray che colpiscono l'oggetto sono minori di quelli necessari e sono incollato
                else if (stickyList[i].StickyRay < rayRequiredStuckObject && stickyList[i].Sticky)
                {
                    //Lo scollo chiamo la callback di fine sticky
                    stickyList[i].Sticky = false;
                    stickyList[i].GIStickyRef.OnStickyEnd();
                }
                //Se i ray che colpiscono l'oggetto sono 0
                else if (stickyList[i].StickyRay == 0)
                {
                    //Rimuovo l'oggetto dalla lista
                    stickyList.RemoveAt(i);
                    continue;
                }

                //Azzero i ray che colpiscono l'oggetto
                stickyList[i].StickyRay = 0;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Funzione che ritorna la direzione opposta alla posizione attuale dell'oggetto
    /// </summary>
    /// <returns></returns>
    private Direction GetHitDirection()
    {
        if (transform.right.y > 0.9f)
            return Direction.Left;
        if (transform.right.y < -0.9f)
            return Direction.Right; ;
        if (transform.right.x > 0.9f)
            return Direction.Above;
        if (transform.right.x < -0.9f)
            return Direction.Below;

        return Direction.None;
    }
}
