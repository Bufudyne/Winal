using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zio;

public class UIWinPanel : MonoBehaviour
{
    public GameObject winnerPanel;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    private void OnEnable()
    {
        EventManager.StartListening(On.StageComplete, OnStageComplete);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.StageComplete, OnStageComplete);
    }

    private void OnStageComplete()
    {
        var score = (int) On.StageComplete.GetMessage();
        if (score > StageManager.Instance.currentStage.star1Score)
        {
            star1.SetActive(true);
        }
        if (score > StageManager.Instance.currentStage.star2Score)
        {
            star2.SetActive(true);
        }
        if (score > StageManager.Instance.currentStage.star3Score)
        {
            star3.SetActive(true);
        }
        winnerPanel.SetActive(true);
    }
    public void LoadNextStageScene()
    {
        Debug.Log("MainMenu");
        EventManager.TriggerEvent(On.LoadScene, "MainMenu");
        //loader.onSceneLoaded += SetLoadingOff;
    }
    
    
}
