using UnityEngine;

public class SeamlessParallax : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.5f;
    private Transform cam;
    private Vector3 lastCamPos;

    private float textureUnitSizeX;
    private Transform leftBG, centerBG, rightBG;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        textureUnitSizeX = sprite.bounds.size.x;

        // Make parent container
        GameObject container = new GameObject("ParallaxContainer");
        container.transform.position = transform.position;

        // Create center (this one), left, and right
        centerBG = transform;
        leftBG = Instantiate(centerBG, container.transform);
        rightBG = Instantiate(centerBG, container.transform);

        leftBG.position = centerBG.position - new Vector3(textureUnitSizeX, 0, 0);
        rightBG.position = centerBG.position + new Vector3(textureUnitSizeX, 0, 0);

        // Remove scripts from clones
        Destroy(leftBG.GetComponent<SeamlessParallax>());
        Destroy(rightBG.GetComponent<SeamlessParallax>());

        // Parent them
        centerBG.parent = container.transform;
    }

    void FixedUpdate()
    {
        // Move with camera
        Vector3 deltaMovement = cam.position - lastCamPos;
        transform.parent.position += new Vector3(deltaMovement.x * parallaxSpeed, deltaMovement.y * parallaxSpeed, 0);
        lastCamPos = cam.position;

        // Calculate camera distance from the center background
        float camX = cam.position.x;
        float centerX = centerBG.position.x;

        // Check if camera is past center -> shift backgrounds BEFORE visible gaps
        if (camX - centerX >= textureUnitSizeX / 2f)
            ShiftRight();
        else if (centerX - camX >= textureUnitSizeX / 2f)
            ShiftLeft();
    }

    private void ShiftRight()
    {
        // Move left to the new right
        Transform oldLeft = leftBG;
        leftBG = centerBG;
        centerBG = rightBG;
        rightBG = oldLeft;

        rightBG.position = centerBG.position + new Vector3(textureUnitSizeX, 0, 0);
    }

    private void ShiftLeft()
    {
        // Move right to the new left
        Transform oldRight = rightBG;
        rightBG = centerBG;
        centerBG = leftBG;
        leftBG = oldRight;

        leftBG.position = centerBG.position - new Vector3(textureUnitSizeX, 0, 0);
    }
}
