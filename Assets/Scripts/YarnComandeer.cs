using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnComandeer : MonoBehaviour
{
    [SerializeField]
    private TimeManager m_timeManager = null;
    private EventManagement m_eventManager = null;

    public void InitRunner(DialogueRunner runner)
    {
        runner.AddCommandHandler("wait_min", WaitMin);
        runner.AddCommandHandler("wait_sec", WaitSec);
        runner.AddCommandHandler("wait_time", WaitTime);
        runner.AddCommandHandler("show_typing", ShowTyping);
        runner.AddCommandHandler("set_timestamp", SetTimestamp);
        runner.AddCommandHandler("set_unread", SetUnread);
        runner.AddCommandHandler("set_ending", SetEnding);
    }

    public void Start()
    {
        m_timeManager = TimeManager.Instance;
        m_eventManager = EventManagement.Instance;
    }

    private void WaitTime(string[] parameters, System.Action onComplete)
    {
        int hr = 0;
        int m = 0;
        try
        {
            int.TryParse(parameters[0], out hr);
            int.TryParse(parameters[1], out m);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Unable to parse param {0} {1} as int", parameters[0], parameters[1]);
        }
        System.DateTime current = m_timeManager.GetCurrentTime();
        var waitUntil = current.AddHours(hr);
        waitUntil = waitUntil.AddMinutes(m);
        var waitAmount = (float)(waitUntil - current).TotalSeconds;

        Debug.LogFormat("(WaitTime) waiting for {0} min", waitAmount / 60.0f);
        StartCoroutine(DoWait(waitAmount, onComplete));
    }

    private void WaitMin(string[] parameters, System.Action onComplete)
    {
        int amount = 0;
        try
        {
            int.TryParse(parameters[0], out amount);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Unable to parse param {0} as int", parameters[0]);
        }
        Debug.LogFormat("(WaitMin) waiting for {0} min", amount);
        StartCoroutine(DoWait(amount * 60.0f, onComplete));
    }

    private void WaitSec(string[] parameters, System.Action onComplete)
    {
        int amount = 0;
        try
        {
            int.TryParse(parameters[0], out amount);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Unable to parse param {0} as int", parameters[0]);
        }
        Debug.LogFormat("(WaitMin) waiting for {0} sec", amount);
        StartCoroutine(DoWait(amount, onComplete));
    }

    private IEnumerator DoWait(float amount, System.Action onComplete)
    {
        yield return new WaitForSeconds(amount);
        onComplete();
    }

    private void ShowTyping(string[] parameters, System.Action onComplete)
    {
        int amount = 0;
        try
        {
            int.TryParse(parameters[1], out amount);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Unable to parse param {0} as int", parameters[1]);
        }
        Debug.LogFormat("(ShowTyping) ({0}) for {1} sec", parameters[0], amount);

        m_eventManager.TriggerShowTypingEvent(parameters[0], amount);
        StartCoroutine(DoWait(amount, onComplete));
    }

    private void SetTimestamp(string[] parameters, System.Action onComplete)
    {
        Debug.Log("params: " + parameters);
        foreach (string s in parameters)
            Debug.Log("params: " + s);

        string charID = parameters[0];
        int y = ParseString(parameters[1]);
        int m = ParseString(parameters[2]);
        int d = ParseString(parameters[3]);
        int h = ParseString(parameters[4]);
        int mm = ParseString(parameters[5]);
        var date = new System.DateTime(y, m, d, h, mm, 0);
        m_eventManager.TriggerSetTimestampEvent(charID, date);
        onComplete.Invoke();
    }


    private void SetUnread(string[] parameters, System.Action onComplete)
    {
        string charID = parameters[0];
        m_eventManager.TriggerCharEvent(charID, EventFlag.SetUnread);
        onComplete.Invoke();
    }

    private int ParseString(string s)
    {
        int r = 0;
        try
        {
            int.TryParse(s, out r);
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Unable to parse param {0} as int", s);
        }
        return r;
    }

    private void SetEnding(string[] parameters, System.Action onComplete)
    {
        GameStateManager.Instance.SetLastThought(parameters[0]);
        onComplete.Invoke();
    }
}
