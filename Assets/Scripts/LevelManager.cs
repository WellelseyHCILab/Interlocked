using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;

public class LevelManager : MonoBehaviour {
	public int nextLevel;
	public GameObject pen;
	private string filename = "userdata.csv";
	private string userIdLocation = "userId.txt";
	static public string userId;

	private Stopwatch sw;
	private Stopwatch observe_state_sw; //for measuring time elapsed when user is only observing pieces via parallax and camera rotation
	private int numSelectEvents, numMouseEvents, numParaX, numParaY, numParaZ;
	private TimeSpan firstPieceLiberationTime;
	private int[] numSelectByPiece;
	private string reset_or_skip = ""; //string to indicate in userdata.csv if a level has been reset or skipped, blank = solved, "r" = reset, "s" skipped
	
	// Use this for initialization
	void Start () {
		firstPieceLiberationTime = new TimeSpan(0);
		if (pen.GetComponent<PenGrab>()) {
			numSelectByPiece = new int[pen.GetComponent<PenGrab>().puzzlePieces.Length];
		} else if (pen.GetComponent<MouseGrab>()) {
			//FIX ME
			numSelectByPiece = new int[pen.GetComponent<MouseGrab>().puzzlePieces.Length];
		}

		sw = new Stopwatch();
		observe_state_sw = new Stopwatch();
		sw.Start();
		observe_state_sw.Start (); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void IncrNumSelectEvents(int pieceIndex) {
		numSelectByPiece[pieceIndex]++;
		numSelectEvents++;
	}
	
	public void IncrNumMouseEvents() {
		numMouseEvents++;
	}
	
	public void IncrNumParallaxXEvents() {
		numParaX++;	
	}
	
	public void IncrNumParallaxYEvents() {
		numParaY++;	
	}
	
	public void IncrNumParallaxZEvents() {
		numParaZ++;	
	}
	
	public void SetFirstPieceLiberationTime() {
		if (firstPieceLiberationTime.Ticks == 0) { // Has it not been set yet?
			firstPieceLiberationTime = sw.Elapsed;
			UnityEngine.Debug.Log ("Time to liberate first piece: " + FormattedTimeString (firstPieceLiberationTime));
		}
	}
	
	//If observe_State_sw is running, stop it, vice versa
	public void toggleObserveStateStopwatch() {
		if (observe_state_sw.IsRunning)
			observe_state_sw.Stop(); //pause time logging
		else
			observe_state_sw.Start(); //resume time logging
	}
	
	public void LevelFinished () {	
		// Get today's date
		string dateString = DateTime.Now.ToString ("yyyy-MM-dd");
		
		// Get total ellapsed time
		sw.Stop();
		observe_state_sw.Stop();
		TimeSpan totalElapsedTime = sw.Elapsed;
		UnityEngine.Debug.Log ("Total time for level: " + FormattedTimeString(totalElapsedTime));
		TimeSpan observeStateElapsedTime = observe_state_sw.Elapsed;
				
		bool logExists = File.Exists (filename);
		using (StreamWriter writer = new StreamWriter (filename, true)) {
			
			string numSelectByPieceStr = "";
			for (int i=0; i<numSelectByPiece.Length; i++) {
				numSelectByPieceStr += numSelectByPiece[i];
				if (i != numSelectByPiece.Length) {
					numSelectByPieceStr += ", ";
				}
			}
			for (int i=numSelectByPiece.Length; i<8; i++) {
				numSelectByPieceStr += ", --";
			}
			
			if (!logExists) {
				writer.Write (	"UserID, " + 
							  	"Date, " +
								"LevelId, " +
								"TotalElapsedTime, " + 
								"ObserveStateElapsedTime, " +
								"NumSelectionEvents, " +
								"NumMouseEvents, " + 
								"DeltaX, " +
								"DeltaY, " +
								"DeltaZ, " +
								"FirstPieceLiberationTime, " + 
								"Piece1Selections, " +
								"Piece2Selections, " +
								"Piece3Selections, " +
								"Piece4Selections, " +
								"Piece5Selections, " +
								"Piece6Selections, " +
								"Piece7Selections, " +
								"Piece8Selections, " + 
								"Reset/Skip" + "\n");
								
			}
			writer.Write (	userId + ", " + 
							dateString + ", " + 
							Application.loadedLevelName + ", " + 
							FormattedTimeString(totalElapsedTime) + ", " +
							FormattedTimeString(observeStateElapsedTime) + ", " +
							numSelectEvents + ", " +
							numMouseEvents + ", " + 
							numParaX + ", " +
							numParaY + ", " +
							numParaZ + ", " +
							FormattedTimeString(firstPieceLiberationTime) + ", " +
							numSelectByPieceStr + ", " +
							reset_or_skip + "\n");
		}
		
		Application.LoadLevel (nextLevel);
		
	}
	
	/// <summary>
	/// Reload the current level after logging data down
	/// </summary>
	public void LevelReload () {	
		// Get today's date
		string dateString = DateTime.Now.ToString ("yyyy-MM-dd");
		
		// Get total ellapsed time
		sw.Stop();
		observe_state_sw.Stop();
		TimeSpan totalElapsedTime = sw.Elapsed;
		UnityEngine.Debug.Log ("Total time for level: " + FormattedTimeString(totalElapsedTime));
		TimeSpan observeStateElapsedTime = observe_state_sw.Elapsed;
				
		bool logExists = File.Exists (filename);
		using (StreamWriter writer = new StreamWriter (filename, true)) {
			
			string numSelectByPieceStr = "";
			for (int i=0; i<numSelectByPiece.Length; i++) {
				numSelectByPieceStr += numSelectByPiece[i];
				if (i != numSelectByPiece.Length) {
					numSelectByPieceStr += ", ";
				}
			}
			for (int i=numSelectByPiece.Length; i<8; i++) {
				numSelectByPieceStr += ", --";
			}
			
			if (!logExists) {
				writer.Write (	"UserID, " + 
							  	"Date, " +
								"LevelId, " +
								"TotalElapsedTime, " + 
								"ObserveStateElapsedTime, " +
								"NumSelectionEvents, " +
								"NumMouseEvents, " + 
								"DeltaX, " +
								"DeltaY, " +
								"DeltaZ, " +
								"FirstPieceLiberationTime, " + 
								"Piece1Selections, " +
								"Piece2Selections, " +
								"Piece3Selections, " +
								"Piece4Selections, " +
								"Piece5Selections, " +
								"Piece6Selections, " +
								"Piece7Selections, " +
								"Piece8Selections, " + 
								"Reset/Skip" + "\n");
								
			}
			writer.Write (	userId + ", " + 
							dateString + ", " + 
							Application.loadedLevelName + ", " + 
							FormattedTimeString(totalElapsedTime) + ", " +
							FormattedTimeString(observeStateElapsedTime) + ", " +
							numSelectEvents + ", " +
							numMouseEvents + ", " + 
							numParaX + ", " +
							numParaY + ", " +
							numParaZ + ", " +
							FormattedTimeString(firstPieceLiberationTime) + ", " +
							numSelectByPieceStr + ", " +
							reset_or_skip + "\n");
		}
		
		Application.LoadLevel (Application.loadedLevel); //reloads current level
		
	}
	
	private String FormattedTimeString (TimeSpan ts) {
		return String.Format ("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
	}
	
	
	public void OnGUI () {
		
		//NOTE: I'm so sorry to whoever is damaging their eyes by looking at the two conditionals below. I can't for the life of me figure out WHY both calls to 
		//either advance a level or reloading a level creates many duplicate entries in the logging file. If you can help me fix it, I'll let you have access
		//to my deliciously insane booze cabinet -tfeng
		
		//manual advancement of level via right arrow key
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			reset_or_skip = "s";
			LevelFinished();
			//*****BUG: log outputs FOUR times of same entry. No issue with this conditional, already tested using flags. Could be four simultaneous LevelManager objects? 
			//Calls to LevelFinished() in other .cs files work fine, except in here with this bug. 
			//*****MAKE SURE: To calculate total number of skips in a session (multiple levels), divide total count of "s" by 4!!! I.e. Sum("s")/4 = skipCount;
		
		}
		
		//reset level via R key
		if(Input.GetKeyDown(KeyCode.R)) {
			reset_or_skip = "r";
			LevelReload();
			//*****BUG: log outputs SIX times of same entry. No issue with this conditional, already tested using flags. Could be four simultaneous LevelManager objects? 
			//Calls to LevelLoaded() in other .cs files work fine, except in here with this bug. 
			//*****MAKE SURE: To calculate total number of resets in a level or session, divide total count of "r" by 6!!! I.e. Sum("r")/6 = resetCount;
		}
			
		/* Code for a next button (Not used)---------------------
		// Center components on screen
		GUILayout.BeginArea(new Rect(1720, 0, 200, Screen.height)); //right edge of screen - feels right
	    GUILayout.FlexibleSpace();
	    GUILayout.BeginHorizontal();
	    GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();		
				
		// Level options
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Next", GUILayout.Width(180), GUILayout.Height(40))) {
			
				Application.LoadLevel(nextLevel);			
		}
		GUILayout.Space(20);
		
		GUILayout.EndHorizontal();
		
		// Align components on screen
		GUILayout.EndVertical();
	    GUILayout.FlexibleSpace();
	    GUILayout.EndHorizontal();
	    GUILayout.FlexibleSpace();
	    GUILayout.EndArea();
	    */
	}
}