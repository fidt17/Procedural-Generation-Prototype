using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerlinTerrainPanel : MonoBehaviour
{
    public Slider seaLevelSlider;
    public TMPro.TMP_InputField seedInput;

    private bool shouldUpdate = false;

    private void OnEnable()
    {
        CalculateMaxOctavesCount();
	}

    private void OnDisable()
    {
        shouldUpdate = false;
	}

    public void OnSettingsChange()
    {
        if(shouldUpdate)
            OnConfirm();
	}

    public void OnSeedEndEdit()
    {
        int seedValue = int.Parse(seedInput.text);
        seedValue = Mathf.Clamp(seedValue, 0, int.MaxValue);
	    
        seedInput.text = seedValue.ToString();
    }

    public void RandomiseSeed()
    {
        int seedValue = (int) Random.Range(0, int.MaxValue);
        seedInput.text = seedValue.ToString();

        OnSeedEndEdit();
	}

    public void OnConfirm()
    {
        if(GameManagerScript.Instance.world == null)
            return;

        int nOctaves = CalculateMaxOctavesCount();
        float fBias = 2;
        int seed = int.Parse(seedInput.text);

        float seaLevel = seaLevelSlider.value;

        MapCreationScript.Instance.Generate2DPerlinTerrain(nOctaves, seed, fBias, seaLevel);
        shouldUpdate = true;
	}

    private int CalculateMaxOctavesCount()
    {
        int maxOctavesCount = 0;
        int worldWidth = GameManagerScript.Instance.world.map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        return maxOctavesCount;
	}
}
