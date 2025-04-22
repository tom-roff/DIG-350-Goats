using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class MapUI : MonoBehaviour
{
    [SerializeField] public TMP_Text rerollText;
    [SerializeField] public TMP_Text movesText;
    [SerializeField] public TMP_Text displayText;
    [SerializeField] public Button rerollButton;


    public void SetRerollText(int rerolls)
    {
        rerollText.text = "Rerolls: " + rerolls;
    }

    public void SetMovesText(int moves)
    {
        movesText.text = "Moves: " + moves;
    }

    public void DisableRerolling()
    {
        rerollButton.interactable = false;
    }

    public void EnableRerolling()
    {
        rerollButton.interactable = true;
    }

    public void DisplayText(string text)
    {
        displayText.text = text;
        Invoke("CloseText", 1f);
    }

    public void CloseText()
    {
        displayText.text = "";
    }


}