using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicTimer : MonoBehaviour
{
    [Header("Audio Reference")]
    public AudioSource audioSource;
    
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public Slider progressSlider;
    
    private float totalDuration;
    private bool wasPlaying = false;

    void Start()
    {
        // Get total duration from the audio clip
        if (audioSource != null && audioSource.clip != null)
        {
            totalDuration = audioSource.clip.length;
            
            // Setup progress slider
            if (progressSlider != null)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = totalDuration;
                progressSlider.value = totalDuration; // Start at full
            }
            
            UpdateTimerDisplay();
        }
    }

    void Update()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            UpdateTimerDisplay();
            UpdateProgressSlider();
        }
    }

    void UpdateTimerDisplay()
    {
        // Calculate remaining time (countdown)
        float remainingTime = Mathf.Max(0, totalDuration - audioSource.time);
        timerText.text = FormatTime(remainingTime);
    }

    void UpdateProgressSlider()
    {
        if (progressSlider != null && audioSource.isPlaying)
        {
            // Progress slider shows remaining time (starts full, goes to empty)
            progressSlider.value = Mathf.Max(0, totalDuration - audioSource.time);
        }
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    
    public void PlayMusic()
    {
        audioSource.Play();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void StopMusic()
    {
        audioSource.Stop();
        // Reset timer display to full duration
        UpdateTimerDisplay();
        if (progressSlider != null)
            progressSlider.value = totalDuration;
    }

    public void TogglePlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

    //parry freeze frame mechanic
    public void PauseMusicWithFreeze()
    {
        wasPlaying = audioSource.isPlaying;
        if (wasPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusicAfterFreeze()
    {
        if (wasPlaying)
        {
            audioSource.Play();
        }
    }

    // Get remaining time for enemy spawning logic
    public float GetRemainingTime()
    {
        return Mathf.Max(0, totalDuration - audioSource.time);
    }

    // For enemies that remove time (they ADD time in countdown)
    public void RemoveTime(float secondsToRemove)
    {
        // In countdown mode, removing time means moving the playback forward
        float newTime = Mathf.Min(totalDuration, audioSource.time + secondsToRemove);
        audioSource.time = newTime;
        
        StartCoroutine(TimeRemovalFlash());
    }

    private System.Collections.IEnumerator TimeRemovalFlash()
    {
        Color originalColor = timerText.color;
        timerText.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        timerText.color = originalColor;
    }

    // eclipse warden
    public bool IsTimeUp()
    {
        return audioSource.time >= totalDuration;
    }
}