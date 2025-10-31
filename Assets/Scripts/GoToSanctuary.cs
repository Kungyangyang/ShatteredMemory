using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSanctuary : MonoBehaviour
{
    public string sanctuary;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Sanctuary is still in the works
            SceneManager.LoadScene("MainGame");
        }
    }
}
