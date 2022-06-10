using System;
using System.Collections;
using System.Collections.Generic;
using DTT.Singletons;
using UnityEngine;

public class StageManager : SingletonBehaviour<StageManager>
{
    public List<StageData> stagesData = new List<StageData>();

    public StageData currentStage;
    public float timer;
    private bool _startCount;

    public void OnEnable()
    {
        EventManager.StartListening(On.LoadStageData, OnLoadStageData,true);
        EventManager.StartListening(On.StartGame, OnBeginStage, true);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.LoadStageData, OnLoadStageData);
        EventManager.StopListening(On.StartGame, OnBeginStage);
    }

    private void OnBeginStage()
    {
        timer = 0f;
        _startCount = true;
    }

    private void Update()
    {
        if (!_startCount) return;
        timer += Time.deltaTime;
        if (!(timer > currentStage.duration)) return;
        EventManager.TriggerEvent(On.TimerRanOut, null);
        ResetTimer();
    }
    
    private void ResetTimer(){
        timer = 0f;
        _startCount = false;

    }

    private void OnLoadStageData()
    {
        var stage = (int)On.LoadStageData.GetMessage();
        timer = 0f;
        currentStage = stagesData[stage];
    }
}
