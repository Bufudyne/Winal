using System;
using System.Collections.Generic;
using Ardalis.SmartEnum;
using DTT.Singletons;

public class EventManager : SingletonBehaviour<EventManager>
{
    private static EventManager _eventManager;
    private Dictionary<int, Event> _eventDictionary;
    
    
    private struct Event
    {
        public Action Action;
        public bool Persistence;
    }

    protected override void Awake()
    {
        _eventDictionary ??= new Dictionary<int, Event>();
    }


    public  void Reset()
    {
        Instance._eventDictionary.Clear();
        UIGlobalCanvas.Instance.Reset();
        StageManager.Instance.OnEnable();
    }

    public static void StartListening(On onEvent, Action listener, bool persistence =false)
    {
        if (Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent))
        {
            thisEvent.Action += listener;
            thisEvent.Persistence = persistence;
            Instance._eventDictionary[onEvent.Value] = thisEvent;
        }
        else
        {
            thisEvent.Action += listener;
            thisEvent.Persistence = persistence;
            Instance._eventDictionary.Add(onEvent.Value, thisEvent);
        }
    }

    public static void StopListening(On onEvent, Action listener)
    {
        if (_eventManager is null || Instance is null) return;
        if (!Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent)) return;
        thisEvent.Action -= listener;
        Instance._eventDictionary[onEvent.Value] = thisEvent;
    }

    public static void TriggerEvent(On onEvent, object message)
    {
        onEvent.SetMessage(message);

        if (Instance._eventDictionary.TryGetValue(onEvent.Value, out var thisEvent))
            thisEvent.Action.Invoke();
    }
}


public sealed class On : SmartEnum<On>
{
    public static On StartGame { get; } = new("StartGame", 0);
    public static On UpdateTileAnimation { get; } = new("UpdateTileAnimation", 1);
    public static On SpawnedPoint { get; } = new("SpawnedPoint", 2);
    public static On PointObtained { get; } = new("PointObtained", 3);
    public static On PointTimedOut { get; } = new("PointTimedOut", 4);
    public static On DeSpawnedPoint { get; } = new("DeSpawnedPoint", 5);
    public static On TookDamage { get; } = new("TookDamage", 6);
    public static On StageComplete { get; } = new("StageComplete", 7);
    public static On LoadScene { get; } = new("LoadScene", 8);
    public static On ShowScreenTransition { get; } = new("ShowScreenTransition", 9);
    public static On HideScreenTransition { get; } = new("HideScreenTransition", 10);
    public static On LoadStageData { get; } = new("LoadStageData", 20);
    public static On BeginStage { get; } = new("BeginStage", 21);
    public static On TimerRanOut { get; } = new("TimerRanOut", 23);
    
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