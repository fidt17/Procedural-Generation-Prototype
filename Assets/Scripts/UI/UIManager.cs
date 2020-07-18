using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public List<GameObject> topPanels;

	private void Awake()
	{
		if(Instance != null) {
			Debug.LogError("Only on UIManager script can exist at a time.", gameObject);
			Destroy(Instance.gameObject);
		}

		Instance = this;
	}

	public void DisableAllPanelsExcept(GameObject exceptionPanel)
	{
		foreach(GameObject p in topPanels) {
			if(p != exceptionPanel)
				p.SetActive(false);
		}
	}
}
