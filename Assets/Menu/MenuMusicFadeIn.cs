using System;
using UnityEditor;
using UnityEngine;

public class MenuMusicFadeIn : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasFadedIn = false;
    public float fadeDuration = 3.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        audioSource.Play();
        StartCoroutine(FadeInAudio());
    }

    private System.Collections.IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 0.67f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0.67f; 
        hasFadedIn = true;
    }

    private void Update()
    {
        if (audioSource != null && !audioSource.isPlaying && hasFadedIn)
        {
            audioSource.volume = 0.67f;
            audioSource.Play();
        }
    }
}
