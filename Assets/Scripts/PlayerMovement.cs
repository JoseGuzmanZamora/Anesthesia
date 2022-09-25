using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public bool foundObject = false;
    public bool woke = false;
    public bool instructions = false;
    public string foundDoor = "";
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float movementCounter = 0;
    [SerializeField] private float increaseFactor = 0.5f;
    [SerializeField] private float decreaseFactor = 1f;
    [SerializeField] private float decreaseTimerLength = 0.1f;
    [SerializeField] private float horizontalFactor = 0.7f;
    [SerializeField] private int hrThreshold = 110;
    [SerializeField] private float deathThreshold = 5;
    [SerializeField] private float thresholdTimer = 0;
    public Text heartRateText;
    public Text capsulesFoundText;
    public Text secondsLeftText;
    public GameObject capsulePrefab;
    public int capsuleAmount;
    public float secondsLeft;
    public GameObject mapObject;
    private int minimumHeartRate = 70;
    private int maximumHeartRate = 200;
    private float heartRateTimer = 0;
    private float decreaseTimer = 0;
    private const int SpeedCap = 7;
    private Animator anim;
    public Animator heartRateAnim;
    public Text noteObject;
    public GameObject doorParent;
    public LevelChanger sceneManager;
    private bool showNote = false;
    private float noteCounter = 0;
    private int capsulesLeft = 0;
    private List<Transform> availableDoors = new List<Transform>();
    private Transform selectedDoor = null;
    private Rigidbody2D rb;
    private AudioSource audioS;
    public AudioClip heartBeat;
    public AudioClip normalClip;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        audioS = gameObject.GetComponent<AudioSource>();
        audioS.clip = normalClip;

        // limits calculation
        var mapSize = mapObject.GetComponent<Renderer>().bounds.size;
        var smallGap = 3;
        var maxY = (mapObject.transform.position.y + (mapSize.y / 2)) - smallGap;
        var minY = (mapObject.transform.position.y - (mapSize.y / 2)) + smallGap;
        var maxX = (mapObject.transform.position.x + (mapSize.x / 2)) - smallGap;
        var minX = (mapObject.transform.position.x - (mapSize.x / 2)) + smallGap;

        for (int i = 0; i < capsuleAmount; i++)
        {
            var tmpX = UnityEngine.Random.Range(minX, maxX);
            var tempY = UnityEngine.Random.Range(minY, maxY);
            Instantiate(capsulePrefab, new Vector3(tmpX, tempY, 0), Quaternion.identity);
        }

        capsulesFoundText.text = capsuleAmount.ToString();
        capsulesLeft = capsuleAmount;

        // Hide initial notes
        if (noteObject != null) noteObject.gameObject.SetActive(showNote);

        // If this is the boss fight, prepare door logic
        if (doorParent != null)
        {
            foreach (Transform child in doorParent.transform)
            {
                availableDoors.Add(child);
            }

            // This is the selected door
            selectedDoor = availableDoors[UnityEngine.Random.Range(0, availableDoors.Count - 1)];
        }

        rb = GetComponent<Rigidbody2D> ();
    }

    private void FixedUpdate() {
        var horizontalPress = Input.GetAxis("Horizontal");
        var verticalPress = Input.GetAxis("Vertical");
        //transform.position += 
        //    new Vector3(horizontalPress, verticalPress, transform.position.z).normalized * (movementSpeed * Time.deltaTime);
        var newPosition = new Vector3(horizontalPress, verticalPress, transform.position.z).normalized * (movementSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + newPosition);

        // Flip and animation logic
        if (horizontalPress > 0)
        {
            // we are walking to the right side
            gameObject.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            anim.SetBool("isWalking", true);
        }
        else if (horizontalPress < 0)
        {
            // Normal
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            anim.SetBool("isWalking", true);
        }
        else if (verticalPress != 0)
        {
            // Movement but in the vertical direction
            anim.SetBool("isWalking", true);
        }
        else
        {
            // Idle animation
            anim.SetBool("isWalking", false);
        }

        var pressingDown = horizontalPress != 0 || verticalPress != 0;

        if (pressingDown)
        {
            movementCounter += (Time.deltaTime * increaseFactor);
            if (movementSpeed < SpeedCap) movementSpeed += (Time.deltaTime * increaseFactor);
        }
        else
        {
            if (movementCounter > 0 && decreaseTimer > decreaseTimerLength) 
            {
                movementCounter -= (Time.deltaTime * decreaseFactor);
                decreaseTimer = 0;
            }
            movementSpeed = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        heartRateTimer += Time.deltaTime;
        decreaseTimer += Time.deltaTime;
        if (showNote) noteCounter += Time.deltaTime;
        if (secondsLeft > 0)
        {
            secondsLeft -= Time.deltaTime;
        }
        else
        {
            woke = true;
        }

        var hr = GetHeartRate(movementCounter);
        heartRateAnim.SetInteger("heartRate", hr);

        if (heartRateTimer > 0.5f)
        {
            heartRateText.text = hr.ToString();
            heartRateTimer = 0;
        }

        if (hr > hrThreshold)
        {
            thresholdTimer += Time.deltaTime;
            if (thresholdTimer > deathThreshold)
            {
                woke = true;
            }

            audioS.clip = heartBeat;
            audioS.volume = 1;
            if (!audioS.isPlaying)
            {
                audioS.Play();
            }
        }
        else
        {
            thresholdTimer = 0;
            if (audioS.clip == heartBeat) audioS.Stop();
            audioS.clip = normalClip;
        }
        secondsLeftText.text = Math.Round(secondsLeft, 2).ToString();

        // Check if we have to hide the note
        if (noteCounter > 3)
        {
            showNote = false;
            noteObject.gameObject.SetActive(showNote);
            noteCounter = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "DiscoveryObject") foundObject = true;
        if (other.tag == "Door") foundDoor = other.name;

        if (other.tag == "Capsule")
        {
            capsulesLeft -= 1;
            capsulesFoundText.text = capsulesLeft.ToString();
            audioS.clip = normalClip;
            audioS.volume = 0.09f;
            audioS.Play();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "ERDoor")
        {
            if (capsulesLeft <= 0)
            {
                sceneManager.FadeToLevel(2);
                //SceneManager.LoadScene("Level2", LoadSceneMode.Single);
            }
            else
            {
                noteObject.text = "You need to collect all capsules before entering the next room...";
                showNote = true;
                noteObject.gameObject.SetActive(showNote);
            }
        }
        else if (other.gameObject.tag == "LockedDoor")
        {
            noteObject.text = "This is locked...";
            showNote = true;
            noteObject.gameObject.SetActive(showNote);
        }
        else if (other.gameObject.tag == "DeathMessage")
        {
            //death for not following instructions
            instructions = true;
        }
        else if (other.gameObject.tag == "HallwayDoor")
        {
            if (capsulesLeft <= 0)
            {
                sceneManager.FadeToLevel(3);
                //SceneManager.LoadScene("Level3", LoadSceneMode.Single);
            }
            else
            {
                noteObject.text = "You need to collect all capsules before entering the next room...";
                showNote = true;
                noteObject.gameObject.SetActive(showNote);
            }
        }
        else if (other.gameObject.tag == "ChillDoor")
        {
            if (capsulesLeft <= 0)
            {
                sceneManager.FadeToLevel(4);
                //SceneManager.LoadScene("Level4", LoadSceneMode.Single);
            }
            else
            {
                noteObject.text = "You need to collect all capsules before entering the next room...";
                showNote = true;
                noteObject.gameObject.SetActive(showNote);
            }
        }
        else if (other.gameObject.tag == "FinalDoor")
        {
            sceneManager.FadeToLevel(5);
            SceneManager.LoadScene("Level5Boss", LoadSceneMode.Single);
        }
        // Finally the simple boss door logic
        else if (other.gameObject.tag == "RandomDoor")
        {
            // Check if it is the selected door or another random door
            if (other.transform.name == selectedDoor.name)
            {
                if (capsulesLeft <= 0)
                {
                    noteObject.text = "You won the game";
                    showNote = true;
                    noteObject.gameObject.SetActive(showNote);
                    sceneManager.FadeToLevel(6);
                }
                else
                {
                    noteObject.text = "You found it! Get the capsules and come back!";
                    showNote = true;
                    noteObject.gameObject.SetActive(showNote);
                }
            }
            else
            {
                // Grab another random door :) and move the player there
                Transform newDoor = null;
                var isSameDoor = true;
                var isWinnerDoor = true;
                // Just make sure the player is not being transported to the same door or the winner door
                while (isSameDoor || isWinnerDoor)
                {
                    var randomDoor = availableDoors[UnityEngine.Random.Range(0, availableDoors.Count - 1)];
                    isSameDoor = randomDoor.name == other.transform.name;
                    isWinnerDoor = randomDoor.name == selectedDoor.name;
                    newDoor = randomDoor;
                }

                // Transform the character to the new door
                // just offset a little bit because of the collider
                if (newDoor.position.x > 0)
                {
                    // Offset to the left
                    transform.position = new Vector3(newDoor.position.x - 2.5f, newDoor.position.y, 0);
                }
                else
                {
                    // Offset to the right
                    transform.position = new Vector3(newDoor.position.x + 2.5f, newDoor.position.y, 0);
                }
            }
        }
    }

    private int GetHeartRate(double placement)
    {
        var result = 
        (((maximumHeartRate - minimumHeartRate)/2) * Math.Tanh((placement * horizontalFactor) - 2.5)) + (minimumHeartRate + ((maximumHeartRate - minimumHeartRate)/2));
        return (int) Math.Round(result, 2);
    }
}
