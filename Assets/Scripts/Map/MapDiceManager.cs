using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapDiceManager : MonoBehaviour
{
    public TMP_Text message;
    public BoxCollider floor;
    public List<Dice> dice;
    public List<int> diceResults;

    public Vector3 startPos = Vector3.forward * 8;
    public Vector3 throwPos = Vector3.back * 5 + Vector3.up * 3;
    public Vector3 throwVector = Vector3.forward;

    public float forceMultiplier;
    public bool playerHasStartedGame = false;
    public bool inMotion = false;
    public float failCounter = 0;

    [Header("Motion")]
    public bool useGyro = true;
    public Vector3 accel;
    public float accelThreshold = 3;

    void Start()
    {
        // UpdateMessage("Click to roll.");
        foreach (Dice d in dice)
        {
            d.rb.MovePosition(startPos);
        }
        // Enable gyroscope and accelerometer
        // Input.gyro.enabled = true;
        Throw();
    }

    // Update is called once per frame
    void Update()
    {

        // if (!inMotion && Input.anyKeyDown)
        //     Throw();

        // // check / store sensor value 
        // if (useGyro)
        // {
        //     accel = Input.acceleration;
        //     if (!playerHasStartedGame && accel.x == 0)
        //     {
        //         Input.gyro.enabled = true;
        //         return;
        //     }
        // }

        // if (useGyro && accel.x > accelThreshold || accel.y > accelThreshold || accel.z > accelThreshold)
        //     Throw();

        // check for fail
        if (inMotion && failCounter > 5)
        {
            Debug.Log("Fail! Reroll");
            UpdateMessage("Bummer. Roll again.");
            return;
        }

        // don't check roll until after after first roll
        if (!playerHasStartedGame)
            return;

        // don't check / change text if a roll is in progress
        if (!inMotion) return;

        // if dice has stopped and on ground
        if (AllHaveStopped() && GroundCheck())
        {
            inMotion = false;
            string msg = "Rolled";
            for (int i = 0; i < dice.Count; i++)
            {
                diceResults[i] = dice[i].side;
                if (i > 0)
                    msg += $" and";
                msg += $" {dice[i].side}";
            }
            UpdateMessage(msg);
            GameManager.Instance.MapManager.SendRoll(dice[0].side);
        }
        else
            failCounter += Time.deltaTime;
    }

    public bool AllHaveStopped()
    {
        bool result = true;
        foreach (Dice d in dice)
        {
            if (d.rb.linearVelocity.magnitude > 0.02f)
                result = false;
        }
        return result;
    }

    public bool GroundCheck()
    {
        bool result = true;
        foreach (Dice d in dice)
        {
            if (!d.isGrounded)
                result = false;
        }
        return result;
    }

    public void Throw()
    {
        UpdateMessage("Rolling...");
        // reset positions
        if (dice.Count <= 1)
        {
            dice[0].rb.MovePosition(throwPos);
        }
        else if (dice.Count <= 2)
        {
            dice[0].rb.MovePosition(throwPos + Vector3.left * -.5f);
            dice[1].rb.MovePosition(throwPos + Vector3.right * -.5f);
        }

        for (int i = 0; i < dice.Count; i++)
        {
            // forward motion
            dice[i].rb.AddForce(throwVector * forceMultiplier, ForceMode.Impulse);
            // angular motion (so they roll better)
            dice[i].rb.AddTorque(new Vector3(2, 3, 2), ForceMode.Impulse);
        }
        // wait a bit until reset (this allows the accelerometer to send multiple calls to throw and so faster motion == harder throw)
        Invoke(nameof(Reset), 0.2f);
    }

    void Reset()
    {
        playerHasStartedGame = true;
        inMotion = true;
        failCounter = 0;
    }

    void UpdateMessage(string str, bool append = false)
    {
        if (message == null) return;
        if (append)
            message.text += str;
        else
            message.text = str;
    }
}
