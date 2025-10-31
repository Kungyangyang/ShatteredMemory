using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite hoverSprite;
    private AudioSource audioSource;
    public string targetSceneName;

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
            SceneManager.LoadScene("MainGame");
    }
}
