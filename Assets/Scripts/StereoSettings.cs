using UnityEngine;
using System.Collections;
using System;

public class StereoSettings : MonoBehaviour {
	public float worldScale;
	public float fieldOfView;
	
	
	// Use this for initialization
	void Start () {
		GetComponent<ZSCore>().SetWorldScale(worldScale);
		GetComponent<ZSCore>().SetFieldOfViewScale(fieldOfView);	
	
	}
	
	// Update is called once per frame
	//Calculate eye position difference per frame, log change if significant
	void Update () {
		
	}
}
