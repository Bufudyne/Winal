using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Zio
{
    public enum Scenes{
        MainMenu,
        GameScene,
        
    }
    public class SceneLoader {
        private readonly string sceneId;
        public Action<string> onSceneLoaded;
 
        public SceneLoader ( string sceneId ) 
        {
            this.sceneId = sceneId;
            var asyncOp = SceneManager.LoadSceneAsync( sceneId ); 
            asyncOp.completed += OnSceneLoaded;
        }
        
        private void OnSceneLoaded ( AsyncOperation op )
        {
            onSceneLoaded?.Invoke( sceneId );
        }
    }
     

    
    public class MainUIManager : MonoBehaviour
    {

        public void LoadGameScene(string stage)
        {
            int.TryParse(stage, out var stagedata);
            EventManager.TriggerEvent(On.LoadStageData, stagedata);
            
            EventManager.TriggerEvent(On.LoadScene, "Default");
            
 
        }
        public void LoadScene(string value)
        {
            var loader = new SceneLoader( value );
        }

        private void SetLoadingOff(string sceneId)
        {
            EventManager.TriggerEvent(On.HideScreenTransition, null);

        }

        public void ApplicationQuit()
        {
            Application.Quit();
        }
        


        
    }
}