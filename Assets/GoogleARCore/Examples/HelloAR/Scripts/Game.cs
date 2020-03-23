using System;
using GoogleARCore.HelloAR;
using UnityEngine;

public class Game : MonoBehaviour {

	[SerializeField] private UI ui;
	[SerializeField] private ScanningARController scanning;
	[SerializeField] private PlayingController playing;

	void Awake() {
		scanning.ui = ui;
		scanning.OnFinishedScanning += ScanningOnOnFinishedScanning;
		scanning.gameObject.SetActive(true);
		playing.gameObject.SetActive(false);
	}

	private void ScanningOnOnFinishedScanning(Chest chest) {
		scanning.gameObject.SetActive(false);
		ui.gameObject.SetActive(false);
		playing.chest = chest;
		playing.OnPlayFinished += PlayingOnOnPlayFinished;
		playing.gameObject.SetActive(true);
	}

	private void PlayingOnOnPlayFinished() {
		ui.FadeToBlack();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (playing.gameObject.activeSelf) {
				playing.gameObject.SetActive(false);
				scanning.gameObject.SetActive(true);
			} else if (scanning.gameObject.activeSelf) {
				Application.Quit();
			}
		}
	}
}