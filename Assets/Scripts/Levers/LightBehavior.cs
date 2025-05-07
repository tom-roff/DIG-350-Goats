using UnityEngine;

public class LightBehavior: MonoBehaviour
{
    public Light indicatorLight; // Assign in Inspector
    public Color defaultColor = Color.red;
    public Color successColor = Color.green;

    void Start()
    {
        if (indicatorLight != null)
        {
            indicatorLight.color = defaultColor;
        }
        else
        {
            Debug.LogError("Indicator light not assigned on LeverLightController!");
        }
    }

    public void SetLightToSuccess()
    {
        if (indicatorLight != null)
        {
            indicatorLight.color = successColor;
        }
    }
}
