using UnityEngine;

public class MapCoordinator : MonoBehaviour
{
    MapManager mapManager;
    OurNetwork ourNetwork;

    void OnEnable()
    {
        EventManager.StartListening("NextState", NextState);
        mapManager = GameManager.Instance.MapManager;
        ourNetwork = GameManager.Instance.OurNetwork;
    }

    void OnDisable()
    {
        EventManager.StopListening("NextState", NextState);
    }

    void Start()
    {
        mapManager.currentState = MapManager.States.Building;
        EventManager.TriggerEvent("Building");
    }

    void NextState()
    {
        switch (mapManager.currentState)
        {
            case MapManager.States.None:
                mapManager.currentState = MapManager.States.Building;
                EventManager.TriggerEvent("Building");
                break;
            case MapManager.States.Building:
                mapManager.currentState = MapManager.States.Preparing;
                EventManager.TriggerEvent("Preparing");
                break;
            case MapManager.States.Preparing:
                mapManager.currentState = MapManager.States.Rolling;
                EventManager.TriggerEvent("Rolling");
                break;
            case MapManager.States.Rolling:
                mapManager.currentState = MapManager.States.Moving;
                EventManager.TriggerEvent("Moving");
                break;
            case MapManager.States.Moving:
                mapManager.currentState = MapManager.States.Rolling;
                EventManager.TriggerEvent("Rolling");
                break;
        }
    }

}
