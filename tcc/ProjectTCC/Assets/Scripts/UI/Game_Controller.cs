using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour {

    public static Game_Controller instance;

    void Start() {
        instance = this;
    }

    void Update() {

        // Chama o "Reload"
        if(Input.GetKey(KeyCode.R) && Player.pr.blockInput) {
            Reload();
        }

    }

    // Reinicia a cena
    public void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}