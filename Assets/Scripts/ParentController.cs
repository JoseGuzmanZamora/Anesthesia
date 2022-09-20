using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour
{
    public MessagesController genericMessage;
    public MessagesController instructionsMessage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var messagesShown = (genericMessage?.shown ?? false) || (instructionsMessage?.shown ?? false);

        if (messagesShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
