using System;
using System.Collections.Generic;
using Ardalis.SmartEnum;
using DTT.Singletons;

public class EventManager : SingletonBehaviour<EventManager>
{
    private static EventManager _eventManager;
    private Dictionary<int, Action> _eventDictionary;

    protected override void Awake()
    {
        _eventDictionary ??= new Dictionary<int, Action>();
    }

    public static void StartListening(On onEvent, Action listener)
    {
        if (Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent))
        {
            thisEvent += listener;
            Instance._eventDictionary[onEvent.Value] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            Instance._eventDictionary.Add(onEvent.Value, thisEvent);
        }
    }

    public static void StopListening(On onEvent, Action listener)
    {
        if (_eventManager is null || Instance is null) return;
        if (!Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent)) return;
        thisEvent -= listener;
        Instance._eventDictionary[onEvent.Value] = thisEvent;
    }

    public static void TriggerEvent(On onEvent, object message)
    {
        onEvent.SetMessage(message);

        if (Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent))
            thisEvent.Invoke();
    }
}


public sealed class On : SmartEnum<On>
{
    public static On StartGame { get; } = new("StartGame", 0);
    public static On UpdateTileAnimation { get; } = new("UpdateTileAnimation", 1);
    public static On SpawnedPoint { get; } = new("SpawnedPoint", 2);
    public static On DeSpawnedPoint { get; } = new("DeSpawnedPoint", 3);
    public static On LoadStage { get; } = new("LoadGame", 99);

    private object _message;
    
    private Type _type;

    private On(string name, int value, Type type = null) : base(name, value)
    {
        _type = type;
    }


    public void SetMessage(object message)
    {
        _message = message;
    }

    public object GetMessage()
    {
        return _message;
    }
}