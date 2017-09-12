using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButtons : MonoBehaviour {

	[SerializeField]
	private Transform puzzleField;

	[SerializeField]
	private GameObject btn;

	void Awake () {

		for(int i = 0; i < 8; i++) {

			GameObject button = Instantiate(btn); //creates a button and assigns it to this game object
			button.name = "" + i;
			button.transform.SetParent(puzzleField, false); //false because we don't want to keep world position
		}

	}

}
