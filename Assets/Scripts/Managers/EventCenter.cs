using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter 
{
    Dictionary<string, IEventInfo> _events = new();
    static EventCenter instance;
    public static EventCenter Instance
    {
        get { 
            if (instance == null)
                instance = new EventCenter();
            return instance; 
        }
    }
    public void AddEvent(string key,UnityAction action)
    {
        if (_events.ContainsKey(key)){
            (_events[key] as EventInfo).mEvent += action;
        }
        else
        {
            _events[key]=new EventInfo(action);
        }
    }
    public void RemoveEvent(string key,UnityAction action)
    {
        if( !_events.ContainsKey(key)) return;

        (_events[key] as EventInfo).mEvent -= action;
    }
    public void Invoke(string key)
    {
        if(!_events.ContainsKey(key)) return;
        (_events[key] as EventInfo).mEvent?.Invoke();
    }
    public void AddEvent<T>(string key, UnityAction<T> action)
    {
        if (_events.ContainsKey(key))
        {
            (_events[key] as EventInfo<T>).mEvent += action;
        }
        else
        {
            _events[key] = new EventInfo<T>(action);
        }
    }
    public void RemoveEvent<T>(string key, UnityAction<T> action)
    {
        if (!_events.ContainsKey(key)) return;

        (_events[key] as EventInfo<T>).mEvent -= action;
    }
    public void Invoke<T>(string key,T p)
    {
        if (!_events.ContainsKey(key)) return;
        (_events[key] as EventInfo<T>).mEvent?.Invoke(p);
    }
}
interface IEventInfo{

}
public class EventInfo : IEventInfo
{
    public UnityAction mEvent;
    public EventInfo(UnityAction mEvent)
    {
        this.mEvent = mEvent;
    }
}
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> mEvent;
    public EventInfo(UnityAction<T> mEvent)
    {
        this.mEvent = mEvent;
    }
}
public static class EventName
{
    public const string playerMoveX=nameof(playerMoveX);
    public const string dead=nameof(dead);
}