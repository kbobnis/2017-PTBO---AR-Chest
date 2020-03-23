using UnityEngine;

public class PassTriggerTo : MonoBehaviour {

	[SerializeField] private UsableItem passTo;

	private void OnTriggerEnter(Collider other) {
		passTo.TouchedWith(other);
	}
}