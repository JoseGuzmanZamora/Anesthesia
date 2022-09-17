using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagesController : MonoBehaviour
{
    public PlayerMovement player;
    public Text mainText;
    // Start is called before the first frame update
    void Start()
    {
        HideMe();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.foundObject)
        {
            mainText.text = "You just found an object, very cool.";
            ShowMe();
            player.foundObject = false;
        }

        if (player.woke)
        {
            mainText.text = "Oops, it looks like you woke up...";
            ShowMe();
        }
    }

    public void ShowMe()
    {
        CanvasGroup cg = this.gameObject.GetComponent<CanvasGroup>();
        cg.interactable = true;
        cg.alpha = 1;
    }

    public void HideMe()
    {
        CanvasGroup cg = this.gameObject.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.alpha = 0;
    }
}