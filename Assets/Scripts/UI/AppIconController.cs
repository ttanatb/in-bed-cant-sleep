using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;

public class AppIconController : MonoBehaviour
{
    [SerializeField]
    private AppController m_appController = null;

    [SerializeField]
    private Image m_loadingCircle = null;
    [SerializeField]
    private Image m_loadingCircleBG = null;
    [SerializeField]
    private AnimationCurve m_loadingCurve = null;
    [SerializeField]
    private Button m_button = null;
    [SerializeField]
    private Image m_notifBubble = null;

    [ShowNonSerializedField]
    private float m_loadingTimer = 0.0f;
    [ShowNonSerializedField]
    private float m_loadingDuration = 0.0f;
    private UnityAction m_onLoadingComplete = null;
    public void LaunchApp()
    {
        m_appController.SetShouldShow(true);
    }

    public void SetInteractible(bool shouldEnable)
    {
        m_button.interactable = shouldEnable;
        m_loadingCircleBG.enabled = !shouldEnable;
    }

    public void SetLoading(float duration)
    {
        SetInteractible(false);
        m_loadingTimer = 0.0f;
        m_loadingDuration = duration;
        m_onLoadingComplete = () => { SetInteractible(true); };
    }

    private void SetShowNotifIcon(bool shouldShow)
    {
        m_notifBubble.enabled = shouldShow;
    }

    [Button]
    private void TestLoading()
    {
        SetLoading(10.0f);
    }


    [Button]
    private void TestSetShowNotifIcon()
    {
        SetShowNotifIcon(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_appController != null)
        {
            m_button.onClick.AddListener(() => m_appController.SetShouldShow(true));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_loadingTimer > m_loadingDuration || m_loadingDuration <= 0.0f)
            return;

        float value = m_loadingCurve.Evaluate(m_loadingTimer / m_loadingDuration);
        m_loadingCircle.fillAmount = value;

        m_loadingTimer += Time.deltaTime;
        if (m_loadingTimer >= m_loadingDuration)
        {
            m_loadingCircle.fillAmount = 0.0f;
            m_onLoadingComplete?.Invoke();
        }
    }
}
