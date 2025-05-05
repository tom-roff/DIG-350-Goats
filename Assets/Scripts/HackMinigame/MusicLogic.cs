using UnityEngine;
using System.Collections;


public class MusicLogic : MonoBehaviour
{
    public AudioSource audio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Structs.IsComputer == true){
            audio.mute = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
