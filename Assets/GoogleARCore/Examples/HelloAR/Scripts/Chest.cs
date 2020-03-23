using System;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour {
	[SerializeField] private Material selectedElement;
	[SerializeField] private Material notSelectedElement;

	private DestroyableElement[] destroyableElements;

	private int locksLeft;

	void Awake() {
		destroyableElements = gameObject.GetComponentsInChildren<DestroyableElement>(true);
		locksLeft = destroyableElements.Sum(t => t.gameObject.activeSelf ? 1 : 0);

		foreach (DestroyableElement element in destroyableElements) {
			element.selected = selectedElement;
			element.notSelected = notSelectedElement;
			element.OnDestroyEvent += ElementOnOnDestroyEvent;
			element.OnSelectedEvent += DeselectOtherElements;
		}
	}

	private void DeselectOtherElements(DestroyableElement destroyableElement) {
		foreach (DestroyableElement element in destroyableElements) {
			if (element != null && element != destroyableElement) {
				element.Select(false);
			}
		}
	}

	private void ElementOnOnDestroyEvent() {
		locksLeft--;
		if (locksLeft <= 0) {
			OpenChest();
		}
	}

	private void OpenChest() {
		GetComponent<ActivateChest>().canOpen = true;
	}

	public void DestroySelectedDestroyable() {
		foreach (DestroyableElement element in destroyableElements) {
			if (element != null && element.gameObject != null && element.isSelected) {
				Handheld.Vibrate();
				Destroy(element.gameObject);
				SoundManager.shatter.Play();
			}
		}
	}
}