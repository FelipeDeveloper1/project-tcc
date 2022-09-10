using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    private Game_Controller gc;

    void Start() {
        gc = GameObject.FindGameObjectWithTag("GC").GetComponent<Game_Controller>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            gc.lastCheckPointPos = transform.position;
        }
    }
}
