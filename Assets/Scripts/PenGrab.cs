using UnityEngine;
using System.Collections;
using System;

public class PenGrab : MonoBehaviour {
	public float springConstant = 100.0f;
	public float dampingConstant = 10.0f;
	public float rayLength = 10.0f;
	public float maxForce = 10.0f;
	public GameObject[] puzzlePieces;
	public GameObject zspace;
	public GameObject levelManager;
	
	private bool firstPieceLiberated = false;
	private bool buttonPrev = false;
	private GameObject selected;
	private GameObject collided;
	private float selectedObjectDistance;
	private Vector3 selectedObjectHitPos;
	
	public float eyePosX;
	public float eyePosY;
	public float eyePosZ;
	
	void Start () {
		//initialize zspace glasses position
		Vector3 eyePosition = zspace.GetComponent<ZSCore>().GetEyePosition(ZSCore.Eye.Center);
		eyePosX = eyePosition.x;
		eyePosY = eyePosition.y;
		eyePosZ = eyePosition.z;
		
	}
	
	void Update () {
		ClearBlockHighlighting();
		Ray pointerRay = new Ray(transform.position, transform.rotation*Vector3.forward);
		

		RaycastHit collidedRaycast = GetCollidedRaycast(pointerRay);
		DrawRay(pointerRay, collidedRaycast);
		if (collidedRaycast.collider && !selected) {
			GameObject hoverObject = collidedRaycast.collider.gameObject.transform.root.gameObject;
			if (collidedRaycast.collider.GetComponent<BlockSelection>() != null)
				hoverObject.GetComponent<BlockSelection>().AddHoverHighlight();
		}
		
		bool buttonCurrent = zspace.GetComponent<ZSCore>().IsTrackerTargetButtonPressed(ZSCore.TrackerTargetType.Primary, 0);
		
		if (buttonCurrent && !buttonPrev) {
			ButtonJustPressed(collidedRaycast);
		} else if (buttonCurrent) {
			ButtonPressed(pointerRay);
		} else if (!buttonCurrent && buttonPrev) {
			ButtonJustReleased(collidedRaycast);
		}
		buttonPrev = buttonCurrent;
		
		//eye tracking log code
		//increment count if significant movement
		Vector3 newEyePosition = zspace.GetComponent<ZSCore>().GetEyePosition(ZSCore.Eye.Center);
		Debug.Log ("Previous eyePosX: " + eyePosX);
		Debug.Log ("Current eyePosX: " + newEyePosition.x);
		Debug.Log ("DiffX: " + (newEyePosition.x - eyePosX));
		Debug.Log ("AbsDiffX: " + Math.Abs (newEyePosition.x - eyePosX));
		Debug.Log ("Rounded AbsDiffx: " + Math.Round(Math.Abs(newEyePosition.x - eyePosX), 3));
		Debug.Log ("Greater than 0.01: " + (Math.Round(Math.Abs(newEyePosition.x - eyePosX), 3) >= 0.005));
		if (Math.Round(Math.Abs(newEyePosition.x - eyePosX), 3) >= 0.005)
			levelManager.GetComponent<LevelManager>().IncrNumParallaxXEvents();
		if (Math.Round(Math.Abs(newEyePosition.y - eyePosY), 3) >= 0.005)
			levelManager.GetComponent<LevelManager>().IncrNumParallaxYEvents();
		if (Math.Round(Math.Abs(newEyePosition.z - eyePosZ), 3) >= 0.005)
			levelManager.GetComponent<LevelManager>().IncrNumParallaxZEvents();
		
		//update coordinates for next time
		eyePosX = newEyePosition.x;
		eyePosY = newEyePosition.y;
		eyePosZ = newEyePosition.z;
	}
	
	void DrawRay(Ray ray, RaycastHit raycast) {
		try {
		LineRenderer line = GetComponent<LineRenderer>();
		
		line.SetPosition (0, ray.origin);
		
		
		if(raycast.collider) {
			float newLength = (ray.origin-raycast.point).magnitude;
			line.SetPosition (1, ray.GetPoint(newLength));
		} else {
			line.SetPosition (1, ray.GetPoint(rayLength));
		}
		}
		catch(NullReferenceException e){
		//do nothing	
		}
		
		
	}
	
	RaycastHit GetCollidedRaycast(Ray pointerRay) {
		RaycastHit[] allCollidedRaycasts = Physics.RaycastAll (pointerRay);
		
		RaycastHit collidedRaycast = new RaycastHit();
		float shortestDistance = -1.0f;
		
		// Find the collided object that's closest to the origin of the ray
		for (int i=0; i < allCollidedRaycasts.Length; i++) {
			if (!allCollidedRaycasts[i].collider)
				continue;
			//Vector3 objectHitPos = allCollidedRaycasts[i].collider.gameObject.transform.root.transform.position - pointerRay.origin;
			Vector3 objectHitPos = allCollidedRaycasts[i].point - pointerRay.origin;
			if (objectHitPos.magnitude < shortestDistance || shortestDistance == -1.0) {
				collidedRaycast = allCollidedRaycasts[i];
				shortestDistance = objectHitPos.magnitude;
			}
		}
		
		return collidedRaycast;
	}
	
	void ButtonJustPressed(RaycastHit raycastHit) {
		levelManager.GetComponent<LevelManager>().toggleObserveStateStopwatch(); //toggle observe_state stopwatch to stop
		if (!raycastHit.collider) {
			return;
		}
		selected = raycastHit.collider.gameObject.transform.root.gameObject;
		if (!selected.GetComponent<BlockSelection>()) {
			selected = null;
			return;
		}

		if (selected && selected.rigidbody != null) {
			selected.rigidbody.drag = 0;
			selected.rigidbody.isKinematic = false;	
//		selected.GetComponent<BlockPositions>().lastPos = selected.transform.position;
		}
		
		// Increase logger's count for number of selections
		int pieceIndex = -1;
		Debug.Log ("PuzzlePieces.Length: " + puzzlePieces.Length);
		for (int i=0; i<puzzlePieces.Length; i++) {
			if (selected == puzzlePieces[i]) {
				pieceIndex = i;
				break;
			}
		}
		Debug.Log("Puzzlepiece index: " + pieceIndex);
		if (pieceIndex < puzzlePieces.Length) {
			Debug.Log("Went into if statement to IncrNumSelectEvents");
			levelManager.GetComponent<LevelManager>().IncrNumSelectEvents(pieceIndex);
		}
		
		selectedObjectDistance =  raycastHit.distance;
		selectedObjectHitPos = raycastHit.collider.gameObject.transform.root.transform.position - raycastHit.point;
		
	}
		
	void ButtonJustReleased(RaycastHit raycastHit) {
		levelManager.GetComponent<LevelManager>().toggleObserveStateStopwatch(); //toggle observe_state stopwatch to resume
		
		if (selected) {
//			BlockPositions blockPos = selected.GetComponent<BlockPositions>();
//			if (blockPos.IsNearOriginalPos() && !blockPos.WasNearOriginalPos()) {
//			 	blockPos.snapbackForce = true;
//				selected.GetComponent<BlockSelection>().AddSelectedHighlight();
//			}
			
			// Check to see if user has won
			int numPiecesInOrigPos = 0;
			foreach (GameObject puzzlePiece in puzzlePieces) {
				if (puzzlePiece.GetComponent<BlockPositions>().IsNearOriginalPos())
					numPiecesInOrigPos++;
			}
			if (numPiecesInOrigPos == puzzlePieces.Length - 1 && !firstPieceLiberated) {
				levelManager.GetComponent<LevelManager>().SetFirstPieceLiberationTime();
//				Debug.Log ("Liberated first piece!");
				firstPieceLiberated = true;
			}
			//checks if current level is tutorial (1), if so, don't go to next one automatically
			else if ((numPiecesInOrigPos <= 1) && (Application.loadedLevel != 1)) {
				levelManager.GetComponent<LevelManager>().LevelFinished();
			}
				
			selected.rigidbody.isKinematic = true;
			selected.rigidbody.drag = 100;
			selected = null;
		}
		
		
	}
	
	void ButtonPressed(Ray pointerRay) {
		levelManager.GetComponent<LevelManager>().toggleObserveStateStopwatch(); //toggle observe_state stopwatch to stop
		if (selected && selected.GetComponent<BlockSelection>()) {
			selected.GetComponent<BlockSelection>().AddSelectedHighlight();
			Vector3 diff = pointerRay.GetPoint(selectedObjectDistance)+selectedObjectHitPos - selected.transform.position;
			Vector3 force = diff * springConstant - dampingConstant*selected.rigidbody.velocity;
			if (force.magnitude > maxForce) {
				force = maxForce*force.normalized;	
			}
			selected.rigidbody.AddForce(force);
		}
	}
	
	void ClearBlockHighlighting() {
		UnityEngine.Object[] objects = FindObjectsOfType(typeof(BlockSelection));
		foreach (UnityEngine.Object block in objects) {
			BlockSelection blockSelection = block as BlockSelection;
			blockSelection.RemoveHighlight();
		}	
	}
}

