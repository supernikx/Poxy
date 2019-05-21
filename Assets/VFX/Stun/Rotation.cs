using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField]
    float speedMultiplier;
    [SerializeField]
    AnimationCurve xVelocity, yVelocity, zVelocity;
    [SerializeField]
    private List<GameObject> fixedPositionChildren = new List<GameObject>();

    float time;
    IEnumerator rotatingCoroutineRef;

    private void OnEnable()
    {
        rotatingCoroutineRef = RotatingCoroutine();
        StartCoroutine(rotatingCoroutineRef);
    }

    IEnumerator RotatingCoroutine()
    {
        while (true)
        {
            time += Time.deltaTime;
            transform.Rotate(xVelocity.Evaluate(time) * speedMultiplier, yVelocity.Evaluate(time) * speedMultiplier, zVelocity.Evaluate(time) * speedMultiplier);
            if (fixedPositionChildren.Count != 0)
            {
                foreach (GameObject _current in fixedPositionChildren)
                {
                    _current.transform.Rotate(-xVelocity.Evaluate(time) * speedMultiplier, -yVelocity.Evaluate(time) * speedMultiplier, -zVelocity.Evaluate(time) * speedMultiplier);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        StopCoroutine(rotatingCoroutineRef);
    }
}
