using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerationPanel : MonoBehaviour
{
    public TMPro.TMP_InputField seedInput;

    private void OnEnable()
    {
	}

    private void OnDisable()
    {
	}

    public void OnSettingsChange()
    {
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

        MapCreationScript.Instance.ClearWorld();

        RogueGenerationScript.Instance.SetSeed(int.Parse(seedInput.text));

        Vector2Int minMaxWidth = new Vector2Int(5, 20);
        Vector2Int minMaxHeight = new Vector2Int(5, 20);

        RogueGenerationScript.Instance.CreateRooms(5, minMaxWidth, minMaxHeight);
	}
}
