using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Can be used to represent board tile ownership, as well
/// as player turn.
/// </summary>
public enum Player
{
	PLAYER_ONE,
	PLAYER_TWO,
	NO_PLAYER
}

public class Board : MonoBehaviour {
	
	//Private variables
	private List<BoardTile> tiles;
	private Player[] tileStates;

	//Public variables
	public GameLogic gameLogic;

	public Board()
	{
		tiles = new List<BoardTile> ();
		tileStates = new Player[64];
	}

	// Use this for initialization
	void Start () {
		BoardInit ();
		return;
	}

	//Updates and returns a simple array of TileStates
	// (backed by ints) representing the board for use
	// in the minimax algorithm. Using this simplified
	// array of ints instead of tile objects should 
	// speed up the minimax algorithm
	public Player[] GetUpdatedBoardModel()
	{
		//Update model
		for(int i = 0; i < 64; i++)
		{
			if(!(tiles[i].occupied))
			{
				tileStates[i] = Player.NO_PLAYER;
			}
			else
			{
				tileStates[i] = tiles[i].GetOwner();
			}
		}

		return tileStates;
	}

	//Adds a tile to the game's tile array - should only be called once per
	//tile at startup
	public void InsertTile(BoardTile tile)
	{
		tiles.Add (tile);

		//If this is the last tile to add
		if(tiles.Count == 64)
		{
			tiles.Sort (
				delegate(BoardTile i1, BoardTile i2) { 
					return i1.gameObject.name.CompareTo (i2.gameObject.name);
				});
		}
	}

	public int IndexOfTile(BoardTile tile)
	{
		return tiles.IndexOf (tile);
	}

	public BoardTile GetTileAt(int idx)
	{
		if(idx >= 0 && idx <= 63)
		{
			return tiles[idx];
		}
		else
		{
			return null;
		}
	}

	void BoardInit()
	{
		//Add starting pieces
		BoardTile tile;

		tile = tiles[27].GetComponent(typeof(BoardTile)) as BoardTile;
		tile.SetOwner (Player.PLAYER_ONE);

		tile = tiles[28].GetComponent(typeof(BoardTile)) as BoardTile;
		tile.SetOwner (Player.PLAYER_TWO);

		tile = tiles[35].GetComponent(typeof(BoardTile)) as BoardTile;
		tile.SetOwner (Player.PLAYER_TWO);
		
		tile = tiles[36].GetComponent(typeof(BoardTile)) as BoardTile;
		tile.SetOwner (Player.PLAYER_ONE);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
