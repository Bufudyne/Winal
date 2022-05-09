using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEnums : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        EventManager.StartListening(On.StartGame, OnStartGame);
    }

    private void OnStartGame(On obj)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.StartGame, OnStartGame);
    }

    private void OnStartGame()
    {
        var test = On.StartGame.GetMessage();
        
        if (On.StartGame.GetMessage() is string value)
        {
            Debug.Log(value);
        }   
    }

    // Update is called once per frame
    public void DispatchEvent(){
        EventManager.TriggerEvent(On.StartGame, "lol");
    }

    public void ShowLol()
    {
        Debug.Log("lololol");
    }
}
