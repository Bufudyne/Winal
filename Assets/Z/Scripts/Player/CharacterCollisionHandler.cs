using System;
using System.Collections;
using System.Collections.Generic;
using Micosmo.SensorToolkit;
using UnityEngine;
using Zio;

public class CharacterCollisionHandler : MonoBehaviour
{
    [SerializeField] private RangeSensor _rangeSensor;
    private List<Tile> _tiles= new List<Tile>();

    private void OnEnable()
    {
        _rangeSensor.OnDetected.AddListener(OnDetected);
        _rangeSensor.OnLostDetection.AddListener(OnLostDetection); 
    }

    private void OnDisable()
    {
        _rangeSensor.OnDetected.RemoveListener(OnDetected);
        _rangeSensor.OnLostDetection.RemoveListener(OnLostDetection); 
    }

    private void OnLostDetection(GameObject arg0, Sensor arg1)
    {
        _tiles.Remove(arg0.GetComponent<Tile>());
    }

    private void OnDetected(GameObject arg0, Sensor arg1)
    {
        _tiles.Add(arg0.GetComponent<Tile>());
    }

    private void Update()
    {
        for (var i = 0; i < _tiles.Count; i++)
        {
            if (_tiles[i].IsPoint())
            {
                _tiles[i].ChangeType(TileType.IsPointCollected);
            }
        }
    }
}
