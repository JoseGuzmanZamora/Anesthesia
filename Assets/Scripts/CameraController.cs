using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public GameObject mapObject;
    public float smoothing;
    private Camera cam;
    private float cameraHeight;
    private float cameraWidth;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cameraHeight = 2f * cam.orthographicSize;
        cameraWidth = cameraHeight * cam.aspect;

        // Calculate the min and max position based on the camera and map
        var mapSize = mapObject.GetComponent<Renderer>().bounds.size;
        var minPositionX = (mapObject.transform.position.x - (mapSize.x / 2)) + (cameraWidth / 2);
        var maxPositionX = (mapObject.transform.position.x + (mapSize.x / 2)) - (cameraWidth / 2);
        var minPositionY = (mapObject.transform.position.y - (mapSize.y / 2)) + (cameraHeight / 2);
        var maxPositionY = (mapObject.transform.position.y + (mapSize.y / 2)) - (cameraHeight / 2);

        minPosition = new Vector2(minPositionX, minPositionY);
        maxPosition = new Vector2(maxPositionX, maxPositionY);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.transform.position != transform.position)
        {
            var playerTransform = player.transform.position;

            Vector3 targetPosition = new Vector3(playerTransform.x, playerTransform.y, transform.position.z);
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            //transform.position = new Vector3(playerTransform.x, playerTransform.y, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
