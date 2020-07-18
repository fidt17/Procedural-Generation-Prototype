using UnityEngine;
using System.Collections;
using UnityEngine.UI;
     
[ExecuteInEditMode]
public class SliderWithValue : MonoBehaviour {
     
    public Slider slider;
    public TMPro.TMP_InputField text;
    private float minValue, maxValue;

    private void Start()
    {
        minValue = slider.minValue;
        maxValue = slider.maxValue;
	}

    public void OnEndEdit()
    {
        int editedValue = int.Parse(text.text);

        editedValue = (int) Mathf.Clamp(editedValue, minValue, maxValue);

        text.text = editedValue.ToString();
        slider.value = editedValue;
	}

    private void OnEnable() 
    {
        slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(slider.value);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
     
    private void ChangeValue(float value)
    {
        text.text = value.ToString();
    }
}

