using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPortal : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject targetDoor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.foundDoor == this.gameObject.name)
        {
            player.foundDoor = "";
            player.transform.position = targetDoor.transform.position;
        }
    }
}
