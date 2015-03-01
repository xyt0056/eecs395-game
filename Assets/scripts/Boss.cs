﻿using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	private float smooth = 5;
	Color angry = Color.red;
	Color calm = Color.yellow;
//	gameObject.Collider.Material = null;

	IBossBehavior behavior; 

	void Angry(){
		Color oldColor = gameObject.renderer.material.color;
		gameObject.renderer.material.color = Color.Lerp(oldColor, angry, Time.deltaTime * smooth);
	}
	 
	void Calm(){
		gameObject.renderer.material.color = calm;
	}

	void Start() {
		behavior = new LineOfSightBossBehavior (GameObject.Find("Runner"));
	}

	void Update() {
		behavior.Update(this);
	}

	void OnCollisionStay(Collision col){
		if (col.gameObject.name == "Runner") {
			Angry();
			behavior.collideWithPlayer();
			GUIManager.showBribeScreen();

			Light mylight = col.gameObject.GetComponentsInChildren<Light>()[0];
			mylight.color = Color.Lerp(mylight.color, Color.red, Time.deltaTime);
//			GameObject.Find("")/
		}
	}

	void OnCollisionExit(Collision col){
//		Debug.Log ("calm");
		if (col.gameObject.name == "Runner") {	
			Calm();
			Light mylight = col.gameObject.GetComponentsInChildren<Light>()[0];
			mylight.color = Color.Lerp(mylight.color, Color.white, Time.deltaTime);
		}
	}

}
