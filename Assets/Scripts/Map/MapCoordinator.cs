using UnityEngine;

public class MapCoordinator : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.StartListening("NextState", NextState);
    }

    void OnDisable()
    {
        EventManager.StopListening("NextState", NextState);
    }

    void Start()
    {
        GameManager.Instance.MapManager.currentState = MapManager.States.Building;
        EventManager.TriggerEvent("Building");
    }

    void NextState()
    {
        switch (GameManager.Instance.MapManager.currentState)
        {
            case MapManager.States.None:
                GameManager.Instance.MapManager.currentState = MapManager.States.Building;
                EventManager.TriggerEvent("Building");
                break;
            case MapManager.States.Building:
                GameManager.Instance.MapManager.currentState = MapManager.States.Preparing;
                EventManager.TriggerEvent("Preparing");
                break;
            case MapManager.States.Preparing:
                GameManager.Instance.MapManager.currentState = MapManager.States.Rolling;
                EventManager.TriggerEvent("Rolling");
                break;
            case MapManager.States.Rolling:
                GameManager.Instance.MapManager.currentState = MapManager.States.Moving;
                EventManager.TriggerEvent("Moving");
                break;
            case MapManager.States.Moving:
                GameManager.Instance.MapManager.currentState = MapManager.States.Rolling;
                EventManager.TriggerEvent("Rolling");
                break;
        }
    }

}
