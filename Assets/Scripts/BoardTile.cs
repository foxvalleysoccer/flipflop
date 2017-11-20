using UnityEngine;
using System.Collections;

public class BoardTile : MonoBehaviour {

	public bool occupied = false;
    public TextMesh feedback;
	private Player Owner;
	public GamePiece piece;
	//private Transform piece;

	public Board board;

	public GameLogic gameLogic;

	//private string name;

	void Awake()
	{
		//Add tile to the board
		board.InsertTile (this);
	}

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Player GetOwner()
	{
		return Owner;
	}

	public void SetOwner(Player owner)
	{
		if (!occupied)
		{
			Owner = owner;
			
			Vector3 placementCoords = this.transform.position + Vector3.up * 2;
		
			Quaternion rotation;
			if(owner == Player.PLAYER_ONE)
			{
				rotation = Quaternion.identity;
			}
			else
			{
				rotation = Quaternion.AngleAxis(180, Vector3.left);
			}

			GameObject gobj = Instantiate (Resources.Load("Prefabs/GamePiece"), placementCoords, rotation) as GameObject;
			piece = gobj.GetComponent(typeof(GamePiece)) as GamePiece;

			piece.whiteUp = (owner == Player.PLAYER_ONE);

			occupied = true;
		}
		else if(occupied && owner != Owner) //Tile is already occupied, switch owner
		{
			piece.flipTile();
			Owner = owner;
		}
		else //Tile is occupied by player we're switching to. Invalid operation
		{
			print ("Error: tile attempted to perform owner change to the same owner.");
		}
	}
    void OnSelect()
    {
        feedback.text += "onselect in board tile";
        //SetOwner (gameLogic.PlayerOneTurn);
        gameLogic.MoveAttempt(this);
    }
    void OnMouseUp()
	{
		//SetOwner (gameLogic.PlayerOneTurn);
		gameLogic.MoveAttempt (this);
		//piece.flipTile ();
	}
}
