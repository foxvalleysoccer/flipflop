using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

	public void Start()
	{
		PlayerPrefs.SetInt ("Difficulty", 3);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void OnGUI()
	{
		//Title
		int titleWidth = 600;
		int titleHeight = 200;
		GUIStyle style = new GUIStyle("label");
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = 72;
		GUI.Label (new Rect (Screen.width / 2 - titleWidth/2, 20, titleWidth, titleHeight), "Reversi", style);

		//Start button
		int btnWidth = 150;
		int btnHeight = 50;

		if (GUI.Button(new Rect(Screen.width/2 - btnWidth/2, Screen.height/2 - btnHeight/2, btnWidth, btnHeight), "Start Game"))
		{
			Application.LoadLevel("game");
		}

		//Difficulty label string
		style.fontSize = 24;
		int diffWidth = 120;
		int diffHeight = 30;
		GUI.Label (new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 50, diffWidth, diffHeight), "Difficulty", style);

		//Difficulty label string
		style.fontSize = 24;
		GUI.Label (new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 80, diffWidth, diffHeight),
		           PlayerPrefs.GetInt("Difficulty").ToString(), style);

		//Difficulty button (left)
		if (GUI.Button(new Rect (Screen.width / 2 - diffWidth/2 - diffHeight, Screen.height/2 + 50, diffHeight, diffHeight), "-"))
		{
			PlayerPrefs.SetInt("Difficulty", Mathf.Max(PlayerPrefs.GetInt("Difficulty") - 1, 1));
		}

		//Difficulty button (right)
		if (GUI.Button(new Rect (Screen.width / 2 + diffWidth/2, Screen.height/2 + 50, diffHeight, diffHeight), "+"))
		{
			PlayerPrefs.SetInt("Difficulty", Mathf.Min(PlayerPrefs.GetInt("Difficulty") + 1, 10));
		}
	}
}
