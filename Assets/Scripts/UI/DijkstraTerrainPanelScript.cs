using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DijkstraTerrainPanelScript : MonoBehaviour
{
    public Toggle rtRender;
    public Slider eSlider, sSlider, wSlider;
    private Coroutine lastCoroutine = null;

    public void ConfirmSettings()
    {
        if(lastCoroutine != null)
            StopCoroutine(lastCoroutine);

        MapCreationScript.Instance.render = rtRender.isOn;

        lastCoroutine = StartCoroutine(MapCreationScript.Instance.StartTerrainGenerationDijkstra((int) eSlider.value, (int) sSlider.value, (int) wSlider.value));
	}
}
