using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zio;
using Random = UnityEngine.Random;

public class TileSystem : MonoBehaviour
{
    [SerializeField] private Animator TileAnimator;
    public List<Tile> tileList = new();
    private  Tile[,] _tiles = new Tile[10, 10];

    private WaitForSeconds _randomWait = new WaitForSeconds(1f);
    private float _seconds;
    private static readonly int StartGame = Animator.StringToHash("StartGame");
    private bool shouldLoop = true;

    private IEnumerator Start()
    {
        yield return _randomWait;
        int x = 0, y = 0;
        foreach (var tile in tileList)
        {
            _tiles[x, y] = tile;
            y++;
            if (y != 10) continue;
            y = 0;
            x++;
        }
        
    }

    private void OnEnable()
    {
        EventManager.StartListening(On.StartGame, OnStartGame);
        EventManager.StartListening(On.StageComplete, OnStageComplete);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.StartGame, OnStartGame);
        EventManager.StartListening(On.StageComplete, OnStageComplete);
    }

    private void OnStageComplete()
    {
        shouldLoop = false;
    }

    private void OnStartGame()
    {
        _tiles[4, 4].ChangeType(TileType.IsSafe);
        _tiles[5, 4].ChangeType(TileType.IsSafe);
        _tiles[4, 5].ChangeType(TileType.IsSafe);
        _tiles[5, 5].ChangeType(TileType.IsSafe);
        StartCoroutine( CreateRandomPoints());
        var stage = (StageData)On.StartGame.GetMessage();
        TileAnimator.runtimeAnimatorController = stage.stagePattern;
        TileAnimator.SetBool(StartGame, true);
    }

    private IEnumerator CreateRandomPoints()
    {
        _randomWait = new WaitForSeconds(Random.Range(2f, 5f));
        var h = Random.Range(0, 9);
        var v = Random.Range(0, 9);
        _tiles[h, v].ChangeType(TileType.IsPoint);
        EventManager.TriggerEvent(On.SpawnedPoint,_tiles[h, v].gameObject);
        yield return _randomWait;
        if(shouldLoop)
            StartCoroutine(CreateRandomPoints());
    }

    private void UpdateColorAnimationEvent()
    {
        EventManager.TriggerEvent(On.UpdateTileAnimation, null);
    }

}