using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class AppController : MonoBehaviour
{
    [SerializeField]
    Vector2 m_offScreenPos = new Vector2(765, 0.0f);
    [SerializeField]
    Vector2 m_onScreenPos = new Vector2(0.0f, 0.0f);
    [SerializeField]
    AnimationCurve m_movementCurve = null;
    [SerializeField]
    float m_animDuration = 1.0f;
    [SerializeField]
    Button m_backButton = null;

    RectTransform m_rectTransform = null;
    float m_timer = 0.0f;
    Vector2 m_startingPos = Vector3.zero;
    Vector2 m_targetPos = Vector3.zero;
    UnityAction m_movementCompletedCb = null;
    UnityAction m_movementCanceledCb = null;
    bool m_isOnScreen = false;

    private UnityEvent m_onShow = new UnityEvent();

    public void AddOnShowListner(UnityAction cb)
    {
        m_onShow.AddListener(cb);
    }

    private UnityEvent m_onHide = new UnityEvent();

    public void AddOnHideListner(UnityAction cb)
    {
        m_onHide.AddListener(cb);
    }


    [Button]
    private void TestShow()
    {
        SetShouldShow(true);
    }

    [Button]
    private void TestHide()
    {
        SetShouldShow(false);
    }

    public void SetShouldShow(bool shouldShow)
    {
        if (shouldShow)
        {
            Move(m_rectTransform.anchoredPosition, m_onScreenPos);
            m_onShow?.Invoke();
        }
        else
        {
            Move(m_rectTransform.anchoredPosition, m_offScreenPos);
            m_onHide?.Invoke();
        }
    }

    void Move(Vector2 startingPos, Vector2 targetPos)
    {
        if (m_timer > 0.0f)
            m_movementCanceledCb?.Invoke();

        m_timer = m_animDuration;
        m_startingPos = startingPos;
        m_targetPos = targetPos;

        m_movementCompletedCb = () => { m_isOnScreen = !m_isOnScreen; };
        m_movementCanceledCb = () => { m_isOnScreen = !m_isOnScreen; };
    }

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out m_rectTransform);
        if (m_backButton != null)
        {
            m_backButton.onClick.AddListener(() => SetShouldShow(false));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timer > 0.0f)
        {
            float value = m_movementCurve.Evaluate((m_animDuration - m_timer) / m_animDuration);
            m_rectTransform.anchoredPosition = Vector2.Lerp(m_startingPos, m_targetPos, value);

            m_timer -= Time.deltaTime;
            if (m_timer < 0.0f)
            {
                m_movementCompletedCb?.Invoke();
                m_rectTransform.anchoredPosition = Vector2.Lerp(m_startingPos, m_targetPos, 1.0f);
            }
        }
    }
}
