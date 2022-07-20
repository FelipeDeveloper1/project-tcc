using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : MonoBehaviour {

	[SerializeField] float   velY; 
	public Rigidbody2D 	     rb; 
	private bool             gliding; 
	
	void Update () {
		
		// Velociadade que o personagem cai
		if (this.gliding) { 
			Vector2 vel = this.rb.velocity; 
			if (vel.y < velY) {
				vel.y = velY; 
				this.rb.velocity = vel; 
			}
		}

	}

	// Paraquedas aberto 
	public void OpenParachute () {
		this.gameObject.SetActive (true); 
		this.gliding = true; 
	}

	// Paraquedas fechado 
	public void CloseParachute () {
		this.gameObject.SetActive (false); 
		this.gliding = false; 
	}
}