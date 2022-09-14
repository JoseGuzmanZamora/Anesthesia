using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public bool foundObject = false;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float movementCounter = 0;
    [SerializeField] private float increaseFactor = 0.5f;
    [SerializeField] private float decreaseFactor = 1f;
    [SerializeField] private float decreaseTimerLength = 0.1f;
    [SerializeField] private float horizontalFactor = 0.7f;
    public Text heartRateText;
    private int minimumHeartRate = 70;
    private int maximumHeartRate = 200;
    private float heartRateTimer = 0;
    private float decreaseTimer = 0;
    private const int SpeedCap = 7;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        heartRateTimer += Time.deltaTime;
        decreaseTimer += Time.deltaTime;
        var horizontalPress = Input.GetAxis("Horizontal");
        var verticalPress = Input.GetAxis("Vertical");
        transform.position += 
            new Vector3(horizontalPress, verticalPress, transform.position.z) * (movementSpeed * Time.deltaTime);

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

        if (heartRateTimer > 0.5f)
        {
            heartRateText.text = hr.ToString();
            heartRateTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Triggered collision.");
        foundObject = true;
    }

    private int GetHeartRate(double placement)
    {
        var result = 
        (((maximumHeartRate - minimumHeartRate)/2) * Math.Tanh((placement * horizontalFactor) - 2.5)) + (minimumHeartRate + ((maximumHeartRate - minimumHeartRate)/2));
        return (int) Math.Round(result, 2);
    }
}
