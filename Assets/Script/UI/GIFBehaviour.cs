
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GIFBehaviour : MonoBehaviour
{
    [SerializeField]
    Image gifImage;
    [SerializeField]
    Sprite[] frames;
    [SerializeField]
    int framesPerSecond = 10;
    float index;
    IEnumerator GifCoroutineRef;

    private void OnEnable()
    {
        GifCoroutineRef = GifCoroutine();
        //StartCoroutine(GifCoroutineRef);
    }

    IEnumerator GifCoroutine()
    {
        if (gifImage == null)
            yield break;

        while (true)
        {
            index = Time.time * framesPerSecond;
            index = index % frames.Length;
            gifImage.sprite = frames[(int)index];
            yield return null;
        }
    }

    private void Update()
    {
        index = Time.time * framesPerSecond;
        index = index % frames.Length;
        gifImage.sprite = frames[(int)index];
    }

    private void OnDisable()
    {
        StopCoroutine(GifCoroutineRef);
    }
}
