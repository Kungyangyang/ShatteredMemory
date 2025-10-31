using System;
using UnityEditor;
using UnityEngine;

public class ButtonCredits : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    private AudioSource audioSource;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseEnter()
    {
        if (hoverSprite != null)
        {
            spriteRenderer.sprite = hoverSprite;
        }

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnMouseExit()
    {
        if (defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void OnMouseDown()
    {
        Application.Quit();
    }
}
