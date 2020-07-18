using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPanelScript : MonoBehaviour
{
    public Slider widthValue, heightValue;

    public void ConfirmNewSettings()
    {
        Vector2Int newWorldDimensions = new Vector2Int( (int) widthValue.value, (int) heightValue.value);
        GameManagerScript.Instance.CreateWorld(newWorldDimensions);
	}
}
