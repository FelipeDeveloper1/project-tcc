using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour {

    public static Game_Controller instance;
    public Vector2 lastCheckPointPos;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        else {
            Destroy(gameObject);
        }
    }
}