using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilr;

public class TimeManager : Singleton<TimeManager>
{
    private bool m_isStarted = false;
    private UnityEvent<DateTime> m_onTimerTick = new UnityEvent<DateTime>();
    private DateTime m_startingDateTime = new DateTime(2020, 07, 5, 0, 32, 0);
    private DateTime m_dateTime = new DateTime(2020, 07, 5, 0, 32, 0);

    public DateTime GetCurrentTime()
    {
        return m_dateTime;
    }
    public void StartTime()
    {
        m_dateTime = m_startingDateTime;
        m_isStarted = true;
    }

    public void AddOnTimerTickListener(UnityAction<DateTime> cb)
    {
        m_onTimerTick.AddListener(cb);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isStarted) return;

        m_dateTime = m_dateTime.AddSeconds(Time.deltaTime);
        m_onTimerTick.Invoke(m_dateTime);
    }
}
