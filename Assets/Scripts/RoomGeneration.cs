using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int numberOfRooms = 50;
    public float roomLength = 25f;

    // Start spawning after the entry room (x = 25)
    private Vector2 nextRoomPosition = new Vector2(25f, 0f);

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            // Randomly choose between the two room prefabs
            GameObject chosenRoom = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

            // Spawn at the next room position
            Instantiate(chosenRoom, nextRoomPosition, Quaternion.identity);

            // Move to the next room position
            nextRoomPosition.x += roomLength;
        }
    }
}
