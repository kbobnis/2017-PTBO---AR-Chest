using UnityEngine;
using System.Collections;

public class ActivateChest : MonoBehaviour {
	public Transform lid, lidOpen, lidClose; // Lid, Lid open rotation, Lid close rotation
	public float openSpeed = 5F; // Opening speed

	[HideInInspector] public bool isOpen; // Is the chest opened

	public bool canOpen;
	private bool isMoving;

	void Update() {

		if (isMoving) {
			if (isOpen) {
				ChestClicked(lidOpen.rotation);
			}
			else if (canOpen) {
				ChestClicked(lidClose.rotation);
			}
		}
	}

	// Rotate the lid to the requested rotation
	void ChestClicked(Quaternion toRot) {
		if (lid.rotation != toRot) {
			lid.rotation = Quaternion.Lerp(lid.rotation, toRot, Time.deltaTime * openSpeed);
		}
		else {
			isMoving = false;
			isOpen = !isOpen;
		}
	}

	public void Swung() {
		isMoving = true;
	}
}