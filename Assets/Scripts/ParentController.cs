using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour
{
    public MessagesController genericMessage;
    public MessagesController instructionsMessage;
    public MessagesController fastMenuMessage;
    public MessagesController deathMessage;
    public bool pausedGame = false;
    public GameObject pauseButton;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var messagesShown = (genericMessage?.shown ?? false) || (instructionsMessage?.shown ?? false) || (fastMenuMessage?.shown ?? false) || (deathMessage?.shown ?? false);

        if (messagesShown)
        {
            Time.timeScale = 0;
            pauseButton.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            if (!pauseButton.activeSelf) pauseButton.SetActive(true);
        }
    }

    public void PauseGame()
    {
        pausedGame = !pausedGame;

        if (pausedGame)
        {
            fastMenuMessage.ShowMe();
            pauseButton.SetActive(false);
        }
        else
        {
            fastMenuMessage.HideMe();
            pauseButton.SetActive(true);
        }
    }
}
