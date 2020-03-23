using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	[SerializeField] private GameObject snackBar;
	[SerializeField] private GameObject wholeScreenBlack;
	[SerializeField] private GameObject topCredits;
	
	[SerializeField] private Text text;

	public void Say(string textToSay) {
		gameObject.SetActive(true);
		text.text = textToSay;
	}

	public void FadeToBlack() {
		gameObject.SetActive(true);
		snackBar.SetActive(false);
		wholeScreenBlack.SetActive(true);
		wholeScreenBlack.GetComponent<Image>().color = Color.black;
		wholeScreenBlack.GetComponentInChildren<Text>().text = "Case solved.";
	}
}