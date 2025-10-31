using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.2f);
    }
}