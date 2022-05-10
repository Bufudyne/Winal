
using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zio;

public class TilePointerManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private GameObject pointerPrefab;
    private List<TilePointer> questPointerList;

    private void Awake()
    {
        questPointerList = new List<TilePointer>();
    }

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
    private void OnSpawnedPoint()
    {
        CreatePointer((Vector3) On.SpawnedPoint.GetMessage());
    }
    private void OnDeSpawnedPoint()
    {
        for (var i = 0; i <  questPointerList.Count; i++)
        {
            if (questPointerList[i].TargetPosition == (Vector3) On.DeSpawnedPoint.GetMessage())
            {
                DestroyPointer(questPointerList[i]);
            }
        }
    }

    private TilePointer CreatePointer(Vector3 targetPosition)
    {
        var pointerGameObject = Instantiate(pointerPrefab, transform, true);
        var questPointer = pointerGameObject.GetComponent<TilePointer>();
        questPointer.Init(targetPosition,uiCamera,mainCamera);
        questPointerList.Add(questPointer);
        return questPointer;
    }

    private void DestroyPointer(TilePointer tilePointer)
    {
        questPointerList.Remove(tilePointer);
        tilePointer.DestroySelf();
    }

    
}