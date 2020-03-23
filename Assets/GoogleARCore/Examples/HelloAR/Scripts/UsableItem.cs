using System;
using System.Collections;
using UnityEngine;

public class UsableItem : MonoBehaviour {

	[SerializeField] private Collider collider;
	[SerializeField] private LineRenderer renderer;

	public event Action OnSwingFarthest; 
	
	public void Swing() {
		GetComponent<Animator>().SetTrigger("hit");
		SoundManager.swoosh.Play();
	}

	public void TouchedWith(Collider other) {
		if (other != null ) {
			SoundManager.chestSlam.Play();
			other.GetComponent<Rigidbody>().AddExplosionForce(100000f, transform.position, 1);
		}
	}
	
	void Update() {
		if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)) ) {
			Swing();
		}

		Vector3 start = renderer.transform.TransformPoint(renderer.GetPosition(0));

		RaycastHit[] allHits = Physics.RaycastAll(start, renderer.transform.forward, 1.0f);
		foreach (RaycastHit hit in allHits) {
			DestroyableElement destroyable = hit.collider.GetComponent<DestroyableElement>();
			if (destroyable != null) {
				destroyable.Select(true);
			}
		}
	}

	public void AnimationTopMovement() {
		StartCoroutine(EnableCollider());
		if (OnSwingFarthest != null) {
			OnSwingFarthest();
		}
	}

	IEnumerator  EnableCollider() {
		collider.enabled = true;
		yield return new WaitForSecondsRealtime(0.1f);
		collider.enabled = false;
	}
	
}