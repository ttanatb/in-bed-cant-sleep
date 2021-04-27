using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class PhoneObj : MonoBehaviour
{
    [SerializeField]
    Animator m_animator = null;

    [SerializeField]
    [AnimatorParam("m_animator")]
    int m_isRingingParam = 1;
    InputMapping m_mapping = null;
    GameStateManager m_gameStateManager = null;

    [Button]
    public void Ring()
    {
        m_animator.SetBool(m_isRingingParam, true);
    }

    [Button]
    public void Snooze()
    {
        m_animator.SetBool(m_isRingingParam, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mapping = ControlScheme.Instance.InputMapping;
        m_mapping.Gameplay.Click.performed += ReadMouse;
        m_gameStateManager = GameStateManager.Instance;
    }

    private void OnDestroy()
    {
        m_mapping.Gameplay.Click.performed -= ReadMouse;
    }


    // Update is called once per frame
    void Update()
    {

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
                m_gameStateManager.ChangeState(GameStateManager.GameState.Phone);
            }
        }
    }
}
