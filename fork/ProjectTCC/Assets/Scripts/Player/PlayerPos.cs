using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPos : MonoBehaviour
{

    private Game_Controller gc;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GC").GetComponent<Game_Controller>();
        transform.position = gc.lastCheckPointPos;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
