using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using Zio;

public class UILoadingPanel : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private MMF_Player _mmfPlayer;
    public void OnEnable()
    {
        EventManager.StartListening(On.ShowScreenTransition, OnShowScreenTransition);
        EventManager.StartListening(On.LoadScene, OnloadScene);
        EventManager.StartListening(On.HideScreenTransition, OnHideScreenTransition);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.ShowScreenTransition, OnShowScreenTransition);
        EventManager.StopListening(On.LoadScene, OnloadScene);
        EventManager.StopListening(On.HideScreenTransition, OnHideScreenTransition);
    }

    private void OnloadScene()
    {
        var scene = (string) On.LoadScene.GetMessage();
        OnShowScreenTransition();
        var loader = new SceneLoader( scene);
        if (scene == "MainMenu")
        {
            EventManager.Instance.Reset();
        }

        loader.onSceneLoaded += OnLoadingSceneComplete ;
    }

    private async void  OnLoadingSceneComplete(string obj)
    {
        await Task.Delay(1000);
        OnHideScreenTransition();
    }

    private void OnHideScreenTransition()
    {
        loadingPanel.SetActive(false);
    }

    private void OnShowScreenTransition()
    {
        loadingPanel.SetActive(true);
    }
}
