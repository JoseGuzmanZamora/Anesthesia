using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MessagesController : MonoBehaviour
{
    public PlayerMovement player;
    public Text mainText;
    public bool hideOnStart = true;
    public bool shown = true;
    private RectTransform canvasTransform;
    private Vector3 originalTransform;
    // Start is called before the first frame update
    void Start()
    {
        canvasTransform = gameObject.GetComponent<RectTransform>();
        originalTransform = canvasTransform.position;
        if (hideOnStart) HideMe();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.woke && gameObject.tag == "GenericMessage")
        {
            mainText.text = @$"... You are no longer under the anesthesia effects. Good luck dealing with the pain. Bye.";
            ShowMe();
        }
    }

    public void ShowMe()
    {
        CanvasGroup cg = this.gameObject.GetComponent<CanvasGroup>();
        cg.interactable = true;
        cg.alpha = 1;
        shown = true;
        canvasTransform.position = new Vector3(originalTransform.x, originalTransform.y, originalTransform.z);
    }

    public void HideMe()
    {
        CanvasGroup cg = this.gameObject.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.alpha = 0;
        shown = false;
        canvasTransform.position = new Vector3(originalTransform.x - 5000, originalTransform.y, originalTransform.z);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
