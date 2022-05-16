using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Zio
{
    public enum Scenes{
        MainMenu,
        GameScene,
        
    }
    public struct SceneLoader {
        public delegate void SceneLoadedHandler ( string sceneId );

        private readonly string sceneId;
        public SceneLoadedHandler onSceneLoaded;
 
        public SceneLoader ( string sceneId ) : this()
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
        public GameObject loadingPanel;
        private void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        public void LoadGameScene()
        {
            
            var loader = new SceneLoader( "Default" );
            loader.onSceneLoaded += SetLoadingOff;
            //StartCoroutine(AsyncSceneLoader("default"));
        }

        private void SetLoadingOff(string sceneid)
        {
            Debug.Log(sceneid);
            loadingPanel.SetActive(false);
            
        }

        
    }
}