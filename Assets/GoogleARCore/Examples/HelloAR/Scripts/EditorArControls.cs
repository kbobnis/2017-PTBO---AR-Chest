using System;
using UnityEngine;

/// <summary>
/// Attach this behaviour to a camera
/// </summary>
public class EditorArControls : MonoBehaviour {
	public event Action OnUseItem;

	void LateUpdate() {
		if (Application.platform != RuntimePlatform.WindowsEditor) {
			return;
		}
		Transform t = GetComponent<Camera>().transform.parent;
		Transform p = t.parent;

		float speed = 2f;
		float step = Time.deltaTime * speed;

		if (Input.GetKey(KeyCode.A)) {
			p.localPosition += -t.right * step;
		}
		else if (Input.GetKey(KeyCode.D)) {
			p.localPosition += t.right * step;
		}
		else if (Input.GetKey(KeyCode.W)) {
			p.localPosition += t.forward * step;
		}
		else if (Input.GetKey(KeyCode.S)) {
			p.localPosition += -t.forward * step;
		}
		else if (Input.GetKey(KeyCode.Q)) {
			p.localPosition += -t.up * step;
		}
		else if (Input.GetKey(KeyCode.E)) {
			p.localPosition += t.up * step;
		}

		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		float axisX = Input.GetAxis("Mouse X");
		float axisY = Input.GetAxis("Mouse Y");

		if (Math.Abs(scrollWheel) > 0.01f) {
			//move closer/back with scroll
			p.localPosition += t.forward * step * scrollWheel * 50f;
		}

		if (Input.GetMouseButton(2)) {
			//while holding middle mouse scroll button
			if (Math.Abs(axisX) > 0.01f) {
				//move up/down
				p.localPosition -= t.right * step * axisX;
			}
			if (Math.Abs(axisY) > 0.01f) {
				//move left right
				p.localPosition -= t.up * step * axisY;
			}
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			//strike with hammer
			if (OnUseItem != null) {
				OnUseItem();
			}
		}

		if (Input.GetMouseButton(1)) {
			//while holding right button
			Vector2 rotation = new Vector2(-axisY, axisX) * 2;

			//transform and transform.parent with rotatin x and y separately is to avoid camera tilting.
			//as described in topic: https://forum.unity.com/threads/rotating-on-x-y-axis-only.146375/
			p.Rotate(0, rotation.y, 0);
			t.Rotate(rotation.x, 0, 0);
		}
	}
}