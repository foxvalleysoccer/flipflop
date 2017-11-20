using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;

public class GameLogic : MonoBehaviour
{
	public Board board;

	private Player turn;

	//True if the most recent turn has been skipped
	private bool turnSkipped;

	public TextManager TextManager;

	public Player Turn { get{ return turn; } }

	public int difficulty;

	public void SetTurn(Player toSet)
	{
		turn = toSet;

		//Check if there's a valid move to play
		bool moveAvailable = false;
		List<int> tileList;
		Player[] boardState = board.GetUpdatedBoardModel ();

		for(int i = 0; i < 64; i++)
		{
			if(boardState[i] == Player.PLAYER_ONE || boardState[i] == Player.PLAYER_TWO)
			{
				continue;
			}
			tileList = getTilesAffected(boardState, turn, i);
			
			if(tileList.Count > 0)
			{
				moveAvailable = true;
				break;
			}
		}

		if(moveAvailable)
		{
			turnSkipped = false;
		}
		else
		{
			if(turnSkipped)
			{
				print ("Stalemate Detected");

				//Determine winner
				int playerPieces = 0;
				int aiPieces     = 0;

				foreach(Player p in board.GetUpdatedBoardModel())
				{
					if(p == Player.PLAYER_ONE) playerPieces++;
					if(p == Player.PLAYER_TWO) aiPieces    ++;
				}

				if(playerPieces > aiPieces)
				{
					//Human wins
					PlayerPrefs.SetString("Winner", "Human Player");
				}
				else if(playerPieces < aiPieces)
				{
					//AI wins
					PlayerPrefs.SetString("Winner", "AI");
				}
				else
				{
					//Tie game
					PlayerPrefs.SetString("Winner", "Nobody");
				}
				FindObjectOfType<PostgameGUI>().ShowGUI = true;
			}
			else
			{
				turnSkipped = true;
				SetTurn(turn == Player.PLAYER_ONE ? Player.PLAYER_TWO : Player.PLAYER_ONE);
			}
		}
	}

	// Use this for initialization
	IEnumerator Start ()
	{
        PlayerPrefs.SetInt("Difficulty",5);
        difficulty = PlayerPrefs.GetInt ("Difficulty");

		SetTurn (Player.PLAYER_ONE);
		
		while (true)
		{
			if(Turn == Player.PLAYER_ONE)
			{
				yield return StartCoroutine(PlayerChoice());
			}
			if(Turn == Player.PLAYER_TWO)
			{
				yield return StartCoroutine(AIChoice());
			}
		}
	}

	public void MoveAttempt(BoardTile tile)
	{
		//Ensure all existing pieces are stable
		for(int i = 0; i < 64; i++)
		{
			if((board.GetTileAt(i).piece != null) && (!board.GetTileAt(i).piece.Stable))
			{
				return;
			}
		}

		List<int> tilesAffected = getTilesAffected (board.GetUpdatedBoardModel(), Turn, board.IndexOfTile (tile));
		if(tilesAffected.Count > 0)
		{
			tile.SetOwner (Turn);
			foreach(int tileIndex in tilesAffected)
			{
				board.GetTileAt(tileIndex).SetOwner(Turn);
			}

			//Swap turns
			SetTurn(Turn==Player.PLAYER_ONE ? Player.PLAYER_TWO : Player.PLAYER_ONE);

			TextManager.RefreshText();
		}
	}

	//Given a tile index, returns an array of tiles representing
	//the tiles that would be flipped if the player were to place
	//a tile at the given index.
	//
	// Assumes a 1-dimensional board representation such that
	// tile 0 lies in the top-left,
	// tile 7 lies in the top-right,
	// tile 56 lies in the bottom-left, and
	// tile 63 lies in the bottom-right,
	private List<int> getTilesAffected(Player[] boardState, Player moveMaker, int tileIndex)
	{
		List<int> affectedTiles = new List<int>();

		// We want to test the following offsets:
		//    +/-1, +/-7, +/-8, +/-9
		//
		// +----+----+----+
		// | -9 | -8 | -7 |
		// +----+----+----+
		// | -1 |This| +1 |
		// +----+----+----+
		// | +7 | +8 | +9 |
		// +----+----+----+
		//

		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex,  1));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex, -1));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex,  7));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex, -7));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex,  8));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex, -8));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex,  9));
		affectedTiles.AddRange(getTilesAffectedHelper(boardState, moveMaker, tileIndex, -9));

		return affectedTiles;
	}

	/// <summary>
	/// Checks if one or more tiles in the direction specified by an offset
	/// will result in an enemy tile getting flanked. If so, returns all
	/// indices that will be flanked.
	/// </summary>
	/// <returns>The tiles affected.</returns>
	/// <param name="tileIndex">"Parent" tile index</param>
	/// <param name="offset">Offset of direction</param>
	private List<int> getTilesAffectedHelper(Player[] boardState, Player moveMaker, int tileIndex, int offset)
	{
		int i;
		int xOffset = 0, yOffset = 0;
		bool enemyFound = false;
		Player tile;
		List<int> affectedTiles = new List<int>();

		i = tileIndex;
		xOffset = (i + offset)%8 - (i%8);
		yOffset = (i + offset)/8 - (i/8);
		i += offset;
		while(i >= 0 && i <= 63 &&
		      xOffset >= -1 && xOffset <=1 &&
		      yOffset >= -1 && yOffset <=1)
		{
			tile = boardState[i];
			if(tile != Player.NO_PLAYER && tile != moveMaker)
			{
				enemyFound = true;
			}
			else if(tile == moveMaker && enemyFound) //Valid move
			{
				i -= offset;
				//Add tiles between here and tileIndex
				while(i != tileIndex)
				{
					affectedTiles.Add(i);
					i -= offset;
				}
				break;
			}
			else
			{
				break;
			}

			xOffset = (i + offset)%8 - (i%8);
			yOffset = (i + offset)/8 - (i/8);
			i += offset;
		}
		
		return affectedTiles;
	}

	IEnumerator PlayerChoice()
	{
		print ("Player's turn!");
		while (Turn == Player.PLAYER_ONE)
		{
			yield return null;
		}
//		
//		bool minimaxFinished = false;
//		bool allTilesStable = false;
//		
//		Player[] currentBoardState = board.GetUpdatedBoardModel ();
//		Player[] bestBoardState = null;
//		
//		//Run minimax
//		BackgroundWorker bw = new BackgroundWorker ();
//		
//		bw.DoWork += new DoWorkEventHandler(
//			delegate(object o, DoWorkEventArgs args)
//			{
//			Tuple<Player[], double> result = Minimax (currentBoardState, 3, Turn, true);
//			print(result.Second);
//			bestBoardState = result.First;
//			
//			minimaxFinished = true;
//		});
//		
//		bw.RunWorkerAsync();
//		
//		yield return new WaitForSeconds(.3f);
//		
//		while(!minimaxFinished)
//		{
//			yield return null;
//		}
//		
//		for(int i = 0; i < 64; i++)
//		{
//			if(bestBoardState[i] == Turn && currentBoardState[i] == Player.NO_PLAYER)
//			{
//				//Ensure all existing pieces are stable
//				while(allTilesStable == false)
//				{
//					allTilesStable = true;
//					for(int j = 0; j < 64; j++)
//					{
//						if((board.GetTileAt(j).piece != null) && (!board.GetTileAt(j).piece.Stable))
//						{
//							allTilesStable = false;
//							yield return null;
//						}
//					}
//				}
//				//Make dat move
//				MoveAttempt(board.GetTileAt(i));
//				
//				SetTurn(Player.PLAYER_TWO);
//			}
//		}
	}
	
	IEnumerator AIChoice()
	{
		print ("AI's turn!");

		bool minimaxFinished = false;
		bool allTilesStable = false;

		Player[] currentBoardState = board.GetUpdatedBoardModel ();
		Player[] bestBoardState = null;

		//Run minimax
		BackgroundWorker bw = new BackgroundWorker ();

		bw.DoWork += new DoWorkEventHandler(
			delegate(object o, DoWorkEventArgs args)
			{
				Tuple<Player[], double> result = Minimax (currentBoardState, (difficulty+2)/2, Turn, true);
				print(result.Second);
				bestBoardState = result.First;
				
				minimaxFinished = true;
			});

		bw.RunWorkerAsync();

		yield return new WaitForSeconds(.3f);

		while(!minimaxFinished)
		{
			yield return null;
		}

		for(int i = 0; i < 64; i++)
		{
			if(bestBoardState[i] == Turn && currentBoardState[i] == Player.NO_PLAYER)
			{
				//Ensure all existing pieces are stable
				while(allTilesStable == false)
				{
					allTilesStable = true;
					for(int j = 0; j < 64; j++)
					{
						if((board.GetTileAt(j).piece != null) && (!board.GetTileAt(j).piece.Stable))
						{
							allTilesStable = false;
							yield return null;
						}
					}
				}
				//Make dat move
				MoveAttempt(board.GetTileAt(i));

				SetTurn(Player.PLAYER_ONE);
			}
		}
	}

	Tuple<Player[], double> Minimax(Player[] boardState, int depth, Player minimaxTurn, bool maximizingPlayer)
	{
		//Generate a map s.t. each key is a child board state, and each
		// value is the child's minimax result.
		Dictionary<Player[], double> childMap = new Dictionary<Player[], double>();
		Player nextDepthPlayer = (minimaxTurn == Player.PLAYER_ONE) ? Player.PLAYER_TWO : Player.PLAYER_ONE;
		//Player nextDepthPlayer = minimaxTurn;

		List<int> tileList;
		Player[] childState;
		Tuple<Player[], double> bestChild = new Tuple<Player[], double>(null, 0);

		if(depth == 0 || boardIsTerminal(boardState, minimaxTurn))
		{
			bestChild.First = boardState; bestChild.Second = staticEval(boardState);
			return bestChild;
		}

		for(int i = 0; i < 64; i++)
		{
			if(boardState[i] == Player.PLAYER_ONE || boardState[i] == Player.PLAYER_TWO)
			{
				continue;
			}
			tileList = getTilesAffected(boardState, minimaxTurn, i);

			if(tileList.Count > 0)
			{
				//Make a new child state and add it to the map
				childState = boardState.Clone() as Player[];
				childState[i] = minimaxTurn;
				foreach(int tileIdx in tileList)
				{
					childState[tileIdx] = minimaxTurn;
				}
				childMap.Add(childState, 0);
			}
		}

		if(maximizingPlayer)
		{
			double val;
			bestChild.Second = double.NegativeInfinity;
			foreach(Player[] child in childMap.Keys)
			{
				val = Minimax(child, depth-1, nextDepthPlayer, false).Second;
				if(val > bestChild.Second)
				{
					bestChild.First = child; bestChild.Second = val;
				}
			}
			return bestChild;
		}
		else // if(!maximizingPlayer)
		{
			double val;
			bestChild.Second = double.PositiveInfinity;
			foreach(Player[] child in childMap.Keys)
			{
				val = Minimax(child, depth-1, nextDepthPlayer, true).Second;
				if(val < bestChild.Second)
				{
					bestChild.First = child; bestChild.Second = val;
				}
			}
			return bestChild;
		}

		print ("ERROR: minimax reached invalid state.");
		return bestChild;
	}

	//Always evaluate from perspective of AI
	float staticEval(Player[] boardState)
	{
		const float cornerPrecedence = 200;
		float countTemp;
		int myTiles;
		int opponentTiles;

		double score = 0f;
		Player thisPlayer = Player.PLAYER_TWO;
		Player opponent = Player.PLAYER_ONE;

		//Boost score if we have a corner
		countTemp = 0;
		if (boardState [0]  == thisPlayer) countTemp++;
		if (boardState [7]  == thisPlayer) countTemp++;
		if (boardState [56] == thisPlayer) countTemp++;
		if (boardState [63] == thisPlayer) countTemp++;
		score += cornerPrecedence * countTemp * (difficulty / 10);

		//Lower score if opponent has a corner
		countTemp = 0;
		if (boardState [0]  == opponent) countTemp++;
		if (boardState [7]  == opponent) countTemp++;
		if (boardState [56] == opponent) countTemp++;
		if (boardState [63] == opponent) countTemp++;
		score -= cornerPrecedence * countTemp * (difficulty / 10);

		//Lower score if we have a tile adjacent to the corner
//		countTemp = 0;
//		if (boardState [1]  == thisPlayer) countTemp++;
//		if (boardState [8]  == thisPlayer) countTemp++;
//		if (boardState [9]  == thisPlayer) countTemp++;
//		if (boardState [6]  == thisPlayer) countTemp++;
//		if (boardState [14]  == thisPlayer) countTemp++;
//		if (boardState [15]  == thisPlayer) countTemp++;
//		if (boardState [48]  == thisPlayer) countTemp++;
//		if (boardState [49]  == thisPlayer) countTemp++;
//		if (boardState [57]  == thisPlayer) countTemp++;
//		if (boardState [55]  == thisPlayer) countTemp++;
//		if (boardState [54]  == thisPlayer) countTemp++;
//		if (boardState [62]  == thisPlayer) countTemp++;
		//score -= cornerPrecedence * countTemp / 4 * (difficulty / 10);

		//Raise score if opponent has a tile adjacent to the corner
//		countTemp = 0;
//		if (boardState [1]  == opponent) countTemp++;
//		if (boardState [8]  == opponent) countTemp++;
//		if (boardState [9]  == opponent) countTemp++;
//		if (boardState [6]  == opponent) countTemp++;
//		if (boardState [14]  == opponent) countTemp++;
//		if (boardState [15]  == opponent) countTemp++;
//		if (boardState [48]  == opponent) countTemp++;
//		if (boardState [49]  == opponent) countTemp++;
//		if (boardState [57]  == opponent) countTemp++;
//		if (boardState [55]  == opponent) countTemp++;
//		if (boardState [54]  == opponent) countTemp++;
//		if (boardState [62]  == opponent) countTemp++;
		//score += cornerPrecedence * countTemp / 4 * (difficulty / 10);

		//Factor in edge tiles
		myTiles = 0;
		opponentTiles = 0;
		for(int i = 0; i < 64; i++)
		{
			if(i / 8 == 0 ||
			   i / 8 == 7 ||
			   i % 8 == 0 ||
			   i % 8 == 7)
			{
				if(boardState[i] == thisPlayer) myTiles++;
				if(boardState[i] == opponent) opponentTiles++;
			}
		}
		score += myTiles * 20 * (difficulty / 10);
		score -= opponentTiles - 20 * (difficulty / 10);


		//Adjust score based on a "softened" tile ratio
		opponentTiles = 0;
		myTiles = 0;
		for(int i = 0; i < 64; i++)
		{
			if(boardState[i]== thisPlayer)
			{
				myTiles++;
			}
			else if(boardState[i]== opponent)
			{
				opponentTiles++;
			}
		}

		myTiles += 2;
		opponentTiles += 2;
		float tileRatio = (float)myTiles / (float)opponentTiles;
		//score += (tileRatio>=0)? Math.Sqrt (1 - tileRatio) * 5 : -Math.Sqrt(Math.Abs(1 - tileRatio)) * 5;
		score += (tileRatio - 1) * 10 * (float)20/difficulty;

		return (float)score;
	}

	bool boardIsTerminal(Player[] boardState, Player playerTurn)
	{
		for(int i = 0; i < 64; i++)
		{
			if(boardState[i] == Player.NO_PLAYER)
			{
				if(getTilesAffected(boardState, playerTurn, i).Count > 0)
				{
					return false;
				}
			}
		}
		return true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel("main_menu");
	}

}

public class Tuple<T1, T2>
{
	public T1 First { get; set; }
	public T2 Second { get; set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}
