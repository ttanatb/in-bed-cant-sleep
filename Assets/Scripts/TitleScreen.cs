using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    Button m_startBtn = null;

    [SerializeField]
    Button m_quitBtn = null;

    [SerializeField]
    TextMeshProUGUI m_startText = null;

    [SerializeField]
    CinemachineVirtualCamera m_titleCam = null;
    [SerializeField]
    CinemachineVirtualCamera m_bedCam = null;

    [SerializeField]
    List<GameObject> m_objectsToDisable = new List<GameObject>();
    [SerializeField]
    CinemachineVirtualCamera m_farCam = null;

    // Start is called before the first frame update
    void Start()
    {

        m_startBtn.onClick.AddListener(() =>
        {
            GameStateManager.Instance.ChangeState(GameStateManager.GameState.Bed);
        });
        m_quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        ControlScheme.Instance.InputMapping.Gameplay.Escape.performed += (ctx) =>
        {
            GameStateManager.Instance.ChangeState(GameStateManager.GameState.Title);
        };

        GameStateManager.Instance.AddOnStateChangedCb((state) =>
        {
            switch (state)
            {
                case GameStateManager.GameState.Title:
                    m_titleCam.Priority = 10;
                    m_bedCam.Priority = 0;
                    m_startText.text = "resume";
                    break;
                case GameStateManager.GameState.Bed:
                    m_titleCam.Priority = 0;
                    m_bedCam.Priority = 10;
                    break;
                case GameStateManager.GameState.Idle:
                    Camera.main.TryGetComponent(out CinemachineBrain brain);
                    brain.m_DefaultBlend.m_Time = 120;
                    m_farCam.Priority = 100;
                    foreach (var obj in m_objectsToDisable)
                    {
                        obj.SetActive(false);
                    }
                    break;
                default: break;
            }
        });
    }
}
