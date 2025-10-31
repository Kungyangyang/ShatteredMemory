using System;
using UnityEditor;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxFactor = 0.1f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector3 parallaxOffset = new Vector3(worldMousePosition.x * parallaxFactor, worldMousePosition.y * parallaxFactor, 0);
        transform.position = initialPosition + parallaxOffset;
    }
}
