using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class PhoneUI : MonoBehaviour
{
    private GameStateManager m_gameStateManager;
    private InputMapping m_mapping = null;

    [SerializeField]
    Vector3 m_offScreenPos = new Vector3(0, -10, 0);
    [SerializeField]
    Vector3 m_onScreenPos = new Vector3(0, 0, 0);
    [SerializeField]
    AnimationCurve m_movementCurve = null;
    [SerializeField]
    float m_animDuration = 1.0f;
    [SerializeField]
    SpriteRenderer m_dimmerSprite = null;
    bool m_isOnScreen = false;

    float m_timer = 0.0f;
    Vector3 m_startingPos = Vector3.zero;
    Vector3 m_targetPos = Vector3.zero;

    float m_startingAlpha = 0.0f;
    float m_targetAlpha = 0.0f;

    UnityAction m_movementCompletedCb = null;
    UnityAction m_movementCanceledCb = null;

    [Button]
    void MoveToScreen()
    {
        Move(transform.position, m_onScreenPos);
    }

    [Button]
    void MoveOffScreen()
    {
        Move(transform.position, m_offScreenPos);
    }

    void Move(Vector3 startingPos, Vector3 targetPos)
    {
        if (m_timer > 0.0f)
            m_movementCanceledCb?.Invoke();

        m_timer = m_animDuration;
        m_startingPos = startingPos;
        m_targetPos = targetPos;

        m_startingAlpha = m_dimmerSprite.color.a;
        m_targetAlpha = m_isOnScreen ? 1 : 0;

        m_movementCompletedCb = () => { m_isOnScreen = !m_isOnScreen; };
        m_movementCanceledCb = () => { m_isOnScreen = !m_isOnScreen; };
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timer > 0.0f)
        {
            float value = m_movementCurve.Evaluate((m_animDuration - m_timer) / m_animDuration);
            transform.position = Vector3.Lerp(m_startingPos, m_targetPos, value);

            // dim the background
            var color = m_dimmerSprite.color;
            color.a = Mathf.Lerp(m_startingAlpha, m_targetAlpha, value);
            m_dimmerSprite.color = color;

            m_timer -= Time.deltaTime;
            if (m_timer < 0.0f)
            {
                m_movementCompletedCb?.Invoke();
            }
        }
    }

    [SerializeField]
    AudioSource m_OnScreen = null;

    [SerializeField]
    AudioSource m_OffScreen = null;

    void CheckMovePhone(GameStateManager.GameState state)
    {
        if (state == GameStateManager.GameState.Phone)
        {
            Move(transform.position, m_onScreenPos);
            m_OnScreen.Play();
        }
        else// if (state == GameStateManager.GameState.Bed)
        {
            Move(transform.position, m_offScreenPos);
            m_OffScreen.Play();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameStateManager = GameStateManager.Instance;
        m_gameStateManager.AddOnStateChangedCb(CheckMovePhone);

        m_mapping = ControlScheme.Instance.InputMapping;
        m_mapping.Gameplay.Click.performed += ReadMouse;
    }

    private void ReadMouse(InputAction.CallbackContext ctx)
    {
        if (Physics.Raycast(
                Camera.main.ScreenPointToRay(
                    Mouse.current.position.ReadValue()),
                out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                m_gameStateManager.ChangeState(GameStateManager.GameState.Bed);
            }
        }
    }
}
