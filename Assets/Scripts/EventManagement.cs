using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using System;

public enum EventFlag
{
    kInvalid = 0,
    DoneLoadingLog,
    SetUnread,
}

public class EventManagement : Utilr.Singleton<EventManagement>
{
    UnityEvent<EventFlag> m_onEventFlag = new UnityEvent<EventFlag>();
    UnityEvent<string, EventFlag> m_onCharIDEventlag = new UnityEvent<string, EventFlag>();
    UnityEvent<string, int> m_onShowTyping = new UnityEvent<string, int>();
    UnityEvent<string, System.DateTime> m_onSetTimpestamp = new UnityEvent<string, DateTime>();
    UnityEvent<string, OptionsModel[]> m_onOptions = new UnityEvent<string, OptionsModel[]>();
    UnityEvent<string> m_onThought = new UnityEvent<string>();

    public void AddEventFlagListener(EventFlag flag, UnityAction cb)
    {
        m_onEventFlag.AddListener((f) =>
        {
            if (f == flag)
                cb.Invoke();
        });
    }

    public void TriggerEvent(EventFlag flag)
    {
        m_onEventFlag.Invoke(flag);
    }

    public void AddCharEventFlagListener(EventFlag flag, UnityAction<string> cb)
    {
        m_onCharIDEventlag.AddListener((id, f) =>
        {
            Debug.LogFormat("flag to listen for {0}, flag hit {1}", flag, f);
            if (f == flag)
                cb.Invoke(id);
        });
    }

    public void TriggerCharEvent(string charID, EventFlag flag)
    {
        m_onCharIDEventlag.Invoke(charID, flag);
    }

    public void TriggerShowTypingEvent(string charID, int amtSec)
    {
        m_onShowTyping.Invoke(charID, amtSec);
    }

    public void AddShowTypingListener(UnityAction<string, int> cb)
    {
        m_onShowTyping.AddListener(cb);
    }

    [SerializeField]
    string m_testCharID = "test";
    [SerializeField]
    int m_typingDuration = 2;

    [Button]
    private void TestTriggerShowTyping()
    {
        TriggerShowTypingEvent(m_testCharID, m_typingDuration);
    }

    public void TriggerSetTimestampEvent(string charID, System.DateTime dateTime)
    {
        m_onSetTimpestamp.Invoke(charID, dateTime);
    }

    public void AddSetTimestampListener(UnityAction<string, System.DateTime> cb)
    {
        m_onSetTimpestamp.AddListener(cb);
    }

    public void TriggerOptions(string charID, OptionsModel[] opts)
    {
        m_onOptions.Invoke(charID, opts);
    }

    public void AddOptionsListener(UnityAction<string, OptionsModel[]> cb)
    {
        m_onOptions.AddListener(cb);
    }

    public void AddThoughtListner(UnityAction<string> cb)
    {
        m_onThought.AddListener(cb);
    }

    public void TriggerThought(string text)
    {
        m_onThought.Invoke(text);
    }
}