using UnityEngine;
using System.Collections;


public class MusicLogic : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // MOBILE
        if (EditorRunMode.Current == EditorRunMode.RunMode.Mobile)
        {
            Debug.Log("iOS or Android");
            SetMute(true);
        }
        // EDITOR (MOBILE SIMULATOR)
        else if (EditorRunMode.Current == EditorRunMode.RunMode.Simulator)
        {
            Debug.Log("Unity Simulator");
            SetMute(true);
        }
        // EDITOR
        else if (Application.installMode == ApplicationInstallMode.Editor)
        {
            Debug.Log("Unity Editor");
            SetMute(false);
        }
        else
        {
            Debug.Log("Unity running on something else! (Mac/Win/Linux)");
            SetMute(false);
        }
    }

    void SetMute(bool state = false)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        audioSource.mute = state;
    }
}

// https://discussions.unity.com/t/how-to-know-if-a-script-is-running-inside-unity-editor-when-using-device-simulator/797971/20
public static class EditorRunMode
{
    public enum RunMode
    {
        Device,
        Editor,
        Mobile,
        Simulator
    }
    public static RunMode Current
    {
        get
        {
#if UNITY_EDITOR
            if (UnityEngine.Device.Application.isEditor && !UnityEngine.Device.Application.isMobilePlatform)
                return RunMode.Editor;
            else
                return RunMode.Simulator;
#else
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                return RunMode.Mobile;
            else
                return RunMode.Device;
#endif
        }
    }
}