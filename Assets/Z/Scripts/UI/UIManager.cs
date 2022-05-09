using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region "DOTween Animations"
    [SerializeField] private DOTweenAnimation getReadyAnimation;
    

    #endregion
    
    private void Start()
    {
        EventManager.TriggerEvent(On.StartGame, null);
    }

    private void OnEnable()
    {
        EventManager.StartListening(On.LoadStage, OnLoadStage);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.LoadStage, OnLoadStage);
    }

    private void OnLoadStage()
    {
        getReadyAnimation.gameObject.SetActive(true);
        getReadyAnimation.DOPlay();
    }
}
