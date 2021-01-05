using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.game.control;

public class SomeCameraScriptThing : MonoBehaviour
{
    //public GameControl gameControl;

    private void OnDisable()
    {
        //GameObject.Find("GameControl").GetComponent<old_GameControl>().StartGame();
        //GameObject.Find("GameControl").GetComponent<GameControl>().StartGame();
        FindObjectOfType<GameControl>().StartGame();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
