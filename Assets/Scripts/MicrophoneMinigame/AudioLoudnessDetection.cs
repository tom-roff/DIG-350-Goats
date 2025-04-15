using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

// https://www.youtube.com/watch?v=dzD0qP8viLw

public class AudioLoudnessDetection : MonoBehaviour
{

    public int sampleWindow = 64;
    private AudioClip microphoneClip;
    
    void Start()
    {
        MicrophoneToAudioClip();
    }

    public void MicrophoneToAudioClip()
    {
        microphoneClip = Microphone.Start(Microphone.devices[0], true, 20, AudioSettings.outputSampleRate);
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;

        if(startPosition < 0) return 0;

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        for(int i = 0; i < sampleWindow; i++) 
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
    }


}