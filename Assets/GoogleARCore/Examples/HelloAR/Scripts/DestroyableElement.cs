using System;
using UnityEngine;

public class DestroyableElement : MonoBehaviour {
	public Material selected;
	public Material notSelected;

	public event Action OnDestroyEvent;
	public event Action<DestroyableElement> OnSelectedEvent;
	public bool isSelected { get; private set; } 

	private void OnDestroy() {
		if (OnDestroyEvent != null) {
			OnDestroyEvent();
		}
	}

	public void Select(bool select) {
		if (select != isSelected) {
			isSelected = select;
			GetComponent<MeshRenderer>().materials = new Material[] {select ? selected : notSelected};

			if (select) {
				SoundManager.rechambering.Play();
				if (OnSelectedEvent != null) {
					OnSelectedEvent(this);
				}
			}
		}
	}

}