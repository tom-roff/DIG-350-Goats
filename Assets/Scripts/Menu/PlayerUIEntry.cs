using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIEntry : NetworkBehaviour
{
    public string name;
    public PlayerColor entryColor;
    public UnityEngine.UI.Image colorImage;
    public TMP_Text nameTextObj;

    public void SetNameAndColor(string nameToSet, PlayerColor colorToSet){
        name = nameToSet;
        nameTextObj.text = name;
        entryColor = colorToSet;
        colorImage.color = colorToSet.colorRGB;
    }
}
