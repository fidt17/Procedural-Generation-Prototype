using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButtonScript : MonoBehaviour
{
	public GameObject panel;

	private void Awake() {
		if(panel == null)
			Debug.Log("Panel is not linked to PanelButtonScript", gameObject);
	}

    public void OnButtonClick() {
		
		panel.SetActive(!panel.active);
		UIManager.Instance.DisableAllPanelsExcept(panel);
	}
}
