using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour {

//Wow, this was fun. No way I could have built this without the help of this fantastic tutorial which I followed along with,
//commenting in detail and making sure I understood each part of the process: https://www.youtube.com/watch?v=J2mja7s4SFg

	//bg image for buttons
	[SerializeField]
	private Sprite bgImage;

	public Sprite[] puzzles;

	//creating a new list of sprites called gamePuzzles
	public List<Sprite> gamePuzzles = new List<Sprite>();

	//creating a new list of buttons called btns
	public List<Button> btns = new List<Button>();

	private bool firstGuess, secondGuess;

	private int countGuesses;
	private int countCorrectGuesses;
	private int gameGuesses;

	private int firstGuessIndex, secondGuessIndex;

	private string firstGuessPuzzle, secondGuessPuzzle;

	public AudioSource source;
	public AudioClip incorrectGuessClip;
	public AudioClip correctGuessClip;
	public AudioClip endGameClip;

	void Awake () {
		puzzles = Resources.LoadAll<Sprite>("Colors");
	}

//First, need to get a reference to each button

	void Start () {
		source = GetComponent<AudioSource>();
		GetButtons();
		AddListeners();
		AddGamePuzzles();
		gameGuesses = gamePuzzles.Count / 2;
		Shuffle(gamePuzzles);
	}
	
	void GetButtons() {
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("PuzzleButton"); //will get all game objects with this tag. 
		//next, want to store them in the list

		//as long as i is less than objects.length, add buttons
		for (int i = 0; i < objects.Length; i++) {
			btns.Add(objects[i].GetComponent<Button>()); //buttons are actually components in this case, not game objects
			//using same index to access same button and change background image (i.e. same button we added right above, we're setting the bg image)
			btns[i].image.sprite = bgImage;
		}
	}

	void AddGamePuzzles() {
		int looper = btns.Count; //looper = the amount of buttons we have (in this case, 8)
		int index = 0;

		//looper will be = to how many button elements we have in our list of buttons (8)
		// i.e. we will loop the same amount of times as buttons we have in our list
		for(int i = 0; i < looper; i++) {
			//when we reach the length of the list / 2, we reset the index to 0, so we'll get a repeat of each element
			if(index == looper / 2) { //looper / 2 = 4 here
				index = 0;
			}
			//passing our index to get the element at that index
			gamePuzzles.Add(puzzles[index]);
			//when we reach 4, index is equal to looper/2 and is set back to zero, so the loop adds the 0 element into slot 4 in array
			index++;
		}

	}

	void AddListeners() {
		foreach (Button btn in btns) {
			//syntax for adding a listener to button
			btn.onClick.AddListener(() => PickAPuzzle());

		}
	}

	//to be executed when button is pressed
	public void PickAPuzzle() {
		string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name; //returns name of current selected game object
		Debug.Log("You are clicking a button named " + name);

		//if this isn't the first guess
		if (!firstGuess) {
			firstGuess = true; //then it is true that this is the first guess,
			//so next time if you try to guess, it will know it's not the first guess
			firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name); //int.Parse() converts a string to an integer. Passing name of game object and converting to int
			//and storing it in our first guess
			//i.e. firstGuessIndex equals the number(name) of the current game object (btn)

			firstGuessPuzzle = gamePuzzles[firstGuessIndex].name; //gets us the name of our image

			btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];

		} else if (!secondGuess) { //else if this isn't the second guess
			secondGuess = true; 

			secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name); 

			secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

			btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

			countGuesses++; //each time we guess, we'll count the guesses

			StartCoroutine(CheckIfThePuzzlesMatch());

		}
	}

	//Coroutine = delayed behavior
	IEnumerator CheckIfThePuzzlesMatch() {

		yield return new WaitForSeconds (1f);
		//if the player has correctly guessed the two colors
		if (firstGuessPuzzle == secondGuessPuzzle) {

			yield return new WaitForSeconds(.5f);
			//disabling the buttons if we've guessed correctly
			btns[firstGuessIndex].interactable = false;
			btns[secondGuessIndex].interactable = false;

			//If player has guessed correctly, buttons turn invisible
			//important that we're setting the Alpha channel to zero here
			btns[firstGuessIndex].image.color = new Color(0,0,0,0);
			btns[secondGuessIndex].image.color = new Color(0,0,0,0);

			source.clip = correctGuessClip;
			source.Play();

			CheckIfTheGameIsFinished();

		} else { //if the player hasn't guessed correctly, reset the buttons' background images
			btns[firstGuessIndex].image.sprite = bgImage;
			btns[secondGuessIndex].image.sprite = bgImage;
			source.clip = incorrectGuessClip;
			source.Play();

		}
		yield return new WaitForSeconds(.5f);

		//
		//equivalent to saying each one is equal to false seperately
		firstGuess = secondGuess = false;
	}

	void CheckIfTheGameIsFinished() {
		countCorrectGuesses++;

		if(countCorrectGuesses == gameGuesses) {
			source.clip = endGameClip;
			source.Play();
			Debug.Log("Game Finished");
			Debug.Log("It took you " + countGuesses + "many guess(es) to finish the game.");

		}
	}

	//this function takes in a list and shuffles its elements
	void Shuffle(List<Sprite> list) {
		for (int i = 0; i < list.Count; i++) { 
			Sprite temp = list[i];
			int randomIndex = Random.Range(i, list.Count); //returns a random number between i and list.Count, not including list.Count
			list[i] = list[randomIndex]; //assigning the element of our randomIndex slot back to i
			list[randomIndex] = temp; //assigning a random sprite

		}
	}
}
