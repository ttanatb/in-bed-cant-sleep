using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class GameStateManager : Utilr.Singleton<GameStateManager>
{

    public enum GameState
    {
        Invalid = 0,
        Title,// = 1,
        Bed,// = 2,
        Phone,// = 3,
        Idle,
    }

    private GameState m_state = GameState.Invalid;

    public UnityEvent<GameState> m_onStateChanged = new UnityEvent<GameState>();

    public void AddOnStateChangedCb(UnityAction<GameState> cb)
    {
        m_onStateChanged.AddListener(cb);
    }

    public void ChangeState(GameState target)
    {
        if (target == m_state)
            return;

        m_state = target;
        m_onStateChanged.Invoke(m_state);
    }

    [ShowNonSerializedField]
    float m_timer = 0.0f;

    [SerializeField]
    float m_stillDuration = 60.0f;

    [SerializeField]
    string m_lastThought = "Guess it's time to sleep";

    [SerializeField]
    StringStringDictionary m_lastThoughtDict = new StringStringDictionary();

    public void SetLastThought(string id)
    {
        if (!m_lastThoughtDict.ContainsKey(id))
        {
            Debug.LogErrorFormat("NO ending with id {0}", id);
            return;
        }
        m_lastThought = m_lastThoughtDict[id];
    }

    private void Update()
    {
        if (m_state != GameState.Bed)
        {
            m_timer = 0.0f;
            return;
        }

        if (CheckInput())
        {
            m_timer = 0.0f;
        }
        else
        {
            m_timer += Time.deltaTime;
        }

        if (m_timer > m_stillDuration)
        {
            ChangeState(GameState.Idle);
            EventManagement.Instance.TriggerThought(m_lastThought);
        }
        prevPos = Mouse.current.position.ReadValue();
    }

    private void Start()
    {
        m_state = GameState.Title;
    }

    Vector2 prevPos = Vector2.zero;

    private bool CheckInput()
    {
        // var gamepad = Gamepad.current;
        // if (gamepad.IsPressed())
        //     return true;
        // var kb = Keyboard.current;
        // if (kb.IsPressed())
        //     return true;
        // var mouse = Mouse.current;
        // if (mouse.IsPressed())
        //     return true;
        if (Vector2.Distance(Mouse.current.position.ReadValue(), prevPos) > Mathf.Epsilon)
        {
            Debug.Log("mouse move");
            return true;
        }

        Debug.Log("idle");
        return false;
    }
}
