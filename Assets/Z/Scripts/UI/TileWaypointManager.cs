
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zio;

public class TileWaypointManager : MonoBehaviour
{

    [SerializeField] private Waypoint_Indicator pointerPrefab;



    private void OnEnable()
    {
        EventManager.StartListening(On.SpawnedPoint, OnSpawnedPoint);
        EventManager.StartListening(On.DeSpawnedPoint, OnDeSpawnedPoint);
    }
    private void OnDisable()
    {
        EventManager.StopListening(On.SpawnedPoint, OnSpawnedPoint);
        EventManager.StopListening(On.DeSpawnedPoint, OnDeSpawnedPoint);
    }

    private void Start()
    {
        pointerPrefab.offScreenSpriteHide = true;
        
    }

    private void OnSpawnedPoint()
    {
        if((GameObject)On.SpawnedPoint.GetMessage() == gameObject)
            pointerPrefab.offScreenSpriteHide = false;
    }
    private void OnDeSpawnedPoint()
    {
        if((GameObject)On.DeSpawnedPoint.GetMessage() == gameObject)
            pointerPrefab.offScreenSpriteHide = true;
    }

}