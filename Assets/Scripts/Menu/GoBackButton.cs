using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GoBackButton : MonoBehaviour
{

    public Button buttonRef;

    
    void Start()
    {
        buttonRef.onClick.AddListener(GoBackToMap);
    }


    void GoBackToMap(){
        GameManager.Instance.MapManager.TimedReturnToMap();
    }


}
