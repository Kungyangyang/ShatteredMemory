using UnityEngine;

public class GroundCheckFollow : MonoBehaviour
{
    public Transform player;   // Drag your Player here in Inspector
    public Vector3 offset;     // Offset so it stays under the feet

    void Update()
    {
        transform.position = player.position + offset;
    }
}
