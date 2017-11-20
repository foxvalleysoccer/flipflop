using UnityEngine;
using System.Collections;

public class TextManager : MonoBehaviour {

	public TextMesh TurnIndicator;
	public TextMesh ScoreStringP1;
	public TextMesh ScoreStringP2;
	public TextMesh DifficultyString;
	public Transform ExamplePieceLocation;

	public Board Board;
	public GameLogic GameLogic;

	// Use this for initialization
	void Start () {
		DifficultyString.text = "AI Difficulty: " + PlayerPrefs.GetInt ("Difficulty");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RefreshText()
	{
		Player[] tileArray = Board.GetUpdatedBoardModel ();
		int p1_count = 0;
		int p2_count = 0;

		for(int i = 0; i < 64; i++)
		{
			if(tileArray[i] == Player.PLAYER_ONE)
			{
				p1_count++;
			}
			else if(tileArray[i] == Player.PLAYER_TWO)
			{
				p2_count++;
			}
		}

		ScoreStringP1.text = "Tiles: " + p1_count;
		ScoreStringP2.text = "Tiles: " + p2_count;

		TurnIndicator.transform.position = new Vector3(
			((GameLogic.Turn == Player.PLAYER_ONE) ? ScoreStringP1 : ScoreStringP2).transform.position.x,
			TurnIndicator.transform.position.y,
			TurnIndicator.transform.position.z);

		ExamplePieceLocation.position = new Vector3 (
			(GameLogic.Turn == Player.PLAYER_ONE) ? -5.969621f : 6.219641f,
			ExamplePieceLocation.position.y,
			ExamplePieceLocation.position.z);
		ExamplePieceLocation.Rotate(new Vector3(180,0,0));
	}
}
