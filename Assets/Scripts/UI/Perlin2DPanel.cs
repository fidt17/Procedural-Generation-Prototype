using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perlin2DPanel : MonoBehaviour
{
    public Slider nOctavesSlider, bias, scaleSlider;
    public TMPro.TMP_InputField seedInput;
    public Toggle animate;

    private Coroutine lastCoroutine = null;
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
        if(shouldUpdate && !animate.isOn)
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

        if(lastCoroutine != null)
                StopCoroutine(lastCoroutine);

        int nOctaves = (int) nOctavesSlider.value;
        float fBias = bias.value;
        int seed = int.Parse(seedInput.text);

        if(animate.isOn) {

            lastCoroutine = StartCoroutine(MapCreationScript.Instance.PerlinNoise2DTerrainAnimation(nOctaves, seed, fBias));
            shouldUpdate = false;
		} else {
            MapCreationScript.Instance.PerlinNoise2DTerrainGeneration(nOctaves, seed, fBias, scaleSlider.value);
            shouldUpdate = true;
		}
	}

    private void CalculateMaxOctavesCount()
    {
        if(GameManagerScript.Instance.world == null)
            return;

        int maxOctavesCount = 0;
        int worldWidth = GameManagerScript.Instance.world.map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        nOctavesSlider.maxValue = maxOctavesCount;
        nOctavesSlider.value = maxOctavesCount;
	}
}
