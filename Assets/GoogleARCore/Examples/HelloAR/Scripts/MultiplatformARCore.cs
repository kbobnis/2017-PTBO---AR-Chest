using System;
using UnityEditor;
using UnityEngine;

public class MultiplatformARCore : MonoBehaviour {

	[SerializeField] private GameObject AndroidArCore;
	[SerializeField] private GameObject EditorArCore;

	void Awake() {
		AndroidArCore.SetActive(false);
		EditorArCore.SetActive(false);
		
		RuntimePlatform platform = Application.platform;

		if (platform == RuntimePlatform.Android) {
			AndroidArCore.SetActive(true);
		} else if (platform == RuntimePlatform.WindowsEditor ||
		           platform == RuntimePlatform.OSXEditor ||
		           platform == RuntimePlatform.LinuxEditor) {
			EditorArCore.SetActive(true);
		}
		else {
			throw new Exception(string.Format("Platform {0} not implemented.", platform.GetType().Name));
		}
	}

}