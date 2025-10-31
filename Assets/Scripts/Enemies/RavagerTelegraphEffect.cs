using System;
using UnityEngine;

public class RavagerTelegraphEffect : MonoBehaviour
{
    public float flickerSpeed = 10f;
    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        // Use sine wave to flicker between two opacity values
        float opacity = Mathf.Sin(timer * flickerSpeed) > 0 ? 150f : 50f;
        
        float alpha = opacity / 255f;
        
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}