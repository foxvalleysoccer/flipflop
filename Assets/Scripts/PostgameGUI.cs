using UnityEngine;
using System.Collections;

public class PostgameGUI : MonoBehaviour {

	private bool showGui = false;

	public bool ShowGUI
	{
		get { return showGui; }
		set { showGui = value; }
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
		if(showGui)
		{

			GUI.backgroundColor = Color.red;

			//Title
			int titleWidth = 500;
			int titleHeight = 80;
			GUIStyle style = new GUIStyle("label");
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 48;
			GUI.Box(new Rect (Screen.width / 2 - titleWidth/2, 100, titleWidth, titleHeight), "");
			GUI.Box(new Rect (Screen.width / 2 - titleWidth/2, 100, titleWidth, titleHeight), "");
			GUI.Label (new Rect (Screen.width / 2 - titleWidth/2, 100, titleWidth, titleHeight),  PlayerPrefs.GetString("Winner") + " Wins!", style);

			//Start button
			int btnWidth = 150;
			int btnHeight = 50;
			if (GUI.Button(new Rect(Screen.width/2 - btnWidth/2, Screen.height/2 - btnHeight/2 + 30, btnWidth, btnHeight), "Play Again"))
			{
				Application.LoadLevel("game");
			}

			//Difficulty label string
			style.fontSize = 24;
			int diffWidth = 120;
			int diffHeight = 30;
			GUI.Box(new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 80, diffWidth, diffHeight*2), "");
			GUI.Box(new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 80, diffWidth, diffHeight*2), "");
			GUI.Label (new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 80, diffWidth, diffHeight), "Difficulty", style);

			//Difficulty label string
			style.fontSize = 24;
			GUI.Label (new Rect (Screen.width / 2 - diffWidth/2, Screen.height/2 + 110, diffWidth, diffHeight),
			           PlayerPrefs.GetInt("Difficulty").ToString(), style);

			//Difficulty button (left)
			if (GUI.Button(new Rect (Screen.width / 2 - diffWidth/2 - diffHeight, Screen.height/2 + 80, diffHeight, diffHeight), "-"))
			{
				PlayerPrefs.SetInt("Difficulty", Mathf.Max(PlayerPrefs.GetInt("Difficulty") - 1, 1));
			}

			//Difficulty button (right)
			if (GUI.Button(new Rect (Screen.width / 2 + diffWidth/2, Screen.height/2 + 80, diffHeight, diffHeight), "+"))
			{
				PlayerPrefs.SetInt("Difficulty", Mathf.Min(PlayerPrefs.GetInt("Difficulty") + 1, 10));
			}
		}
	}
}
