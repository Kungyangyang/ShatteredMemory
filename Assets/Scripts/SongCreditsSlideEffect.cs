using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using TMPro;

public class TextSlideEffect : MonoBehaviour
{
    public RectTransform textTransform;
    public float speed = 500f;
    public float delay = 2f;
    public Vector3 startPosition;
    public Vector3 centerPosition;

    void Start()
    {
        StartCoroutine(SlideText());
    }

    IEnumerator SlideText()
    {
        while (Vector3.Distance(textTransform.localPosition, centerPosition) > 0.1f)
        {
            textTransform.localPosition = Vector3.MoveTowards(textTransform.localPosition, centerPosition, speed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        // Move text
        while (Vector3.Distance(textTransform.localPosition, startPosition) > 0.1f)
        {
            textTransform.localPosition = Vector3.MoveTowards(textTransform.localPosition, startPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
