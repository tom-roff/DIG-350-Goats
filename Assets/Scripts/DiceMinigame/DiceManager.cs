using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviour
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
    public Vector3 accel;
    public float accelThreshold = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        message.text = $"Click to roll.";
        dice[0].rb.MovePosition(startPos);
        dice[1].rb.MovePosition(startPos);
        // Enable gyroscope and accelerometer
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Throw();
        }

        accel = Input.acceleration;
        if (!playerHasStartedGame && accel.x == 0)
        {
            Input.gyro.enabled = true;
            return;
        }

        if (inMotion && failCounter > 5)
        {
            Debug.Log("Fail! Reroll");
            message.text = $"Bummer. Roll again.";
            return;
        }
        if (!inMotion)
        {
            if (accel.x > accelThreshold || accel.y > accelThreshold || accel.z > accelThreshold)
                Throw();
        }
        // don't check roll until after after first roll
        if (!playerHasStartedGame)
            return;

        if (HasStopped() && GroundCheck())
        {
            inMotion = false;
            for (int i = 0; i < dice.Count; i++)
            {
                diceResults[i] = dice[i].side;
            }
            message.text = $"Rolled {dice[0].side} and {dice[1].side}";
        }
        else if (HasStopped())
        {
            failCounter += Time.deltaTime;
        }
        else if (GroundCheck())
        {
            failCounter += Time.deltaTime;
        }
    }

    public bool HasStopped()
    {
        return (
            dice[0].rb.linearVelocity.magnitude < 0.02f
            && dice[1].rb.linearVelocity.magnitude < 0.02f
        );
    }

    public bool GroundCheck()
    {
        return dice[0].isGrounded && dice[1].isGrounded;
    }

    public bool FloorCheck()
    {
        return (
            dice[0].boxCollider.bounds.Intersects(floor.bounds)
            && dice[1].boxCollider.bounds.Intersects(floor.bounds)
        );
    }

    public void Throw()
    {
        message.text = $"Rolling...";
        // reset positions
        dice[0].rb.MovePosition(throwPos + Vector3.left * -.5f);
        dice[1].rb.MovePosition(throwPos + Vector3.right * -.5f);
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
}
