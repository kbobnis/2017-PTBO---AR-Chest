	using System;
	using UnityEngine;
public class PlayingController : MonoBehaviour {

	[SerializeField] private UsableItem hammer;
	public Chest chest;
	public event Action OnPlayFinished;
	
	private void OnDisable() {
		hammer.gameObject.SetActive(false);
	}

	private void OnEnable() {
		hammer.gameObject.SetActive(true);
		hammer.OnSwingFarthest += HammerOnOnSwingFarthest;
	}

	private void HammerOnOnSwingFarthest() {
		chest.DestroySelectedDestroyable();
		chest.GetComponent<ActivateChest>().Swung();
	}

}