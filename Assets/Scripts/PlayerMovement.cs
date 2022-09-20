using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public bool foundObject = false;
    public bool woke = false;
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
    private int capsulesLeft = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

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
    }

    // Update is called once per frame
    void Update()
    {
        heartRateTimer += Time.deltaTime;
        decreaseTimer += Time.deltaTime;
        if (secondsLeft > 0)
        {
            secondsLeft -= Time.deltaTime;
        }
        else
        {
            woke = true;
        }
        var horizontalPress = Input.GetAxis("Horizontal");
        var verticalPress = Input.GetAxis("Vertical");
        transform.position += 
            new Vector3(horizontalPress, verticalPress, transform.position.z).normalized * (movementSpeed * Time.deltaTime);

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
            movementSpeed = 3;
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
        }
        else
        {
            thresholdTimer = 0;
        }
        secondsLeftText.text = Math.Round(secondsLeft, 2).ToString();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "DiscoveryObject") foundObject = true;
        if (other.tag == "Door") foundDoor = other.name;

        if (other.tag == "Capsule")
        {
            capsulesLeft -= 1;
            capsulesFoundText.text = capsulesLeft.ToString();
            Destroy(other.gameObject);
        }
    }

    private int GetHeartRate(double placement)
    {
        var result = 
        (((maximumHeartRate - minimumHeartRate)/2) * Math.Tanh((placement * horizontalFactor) - 2.5)) + (minimumHeartRate + ((maximumHeartRate - minimumHeartRate)/2));
        return (int) Math.Round(result, 2);
    }
}
