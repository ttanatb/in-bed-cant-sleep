using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;
using Yarn.Unity;

public class OptionButtonController : MonoBehaviour
{
    [SerializeField]
    private Button m_button = null;

    [SerializeField]
    private Image m_image = null;

    [SerializeField]
    private TextMeshProUGUI m_text = null;

    private List<RectTransform> m_dirtyRects = new List<RectTransform>();
    private RectTransform m_rectTransform = null;

    [SerializeField]
    Animator m_animator = null;
    [SerializeField]
    [AnimatorParam("m_animator")]
    int m_showParam = 1;

    private void Start()
    {
        m_dirtyRects.Add(m_button.GetComponent<RectTransform>());
        m_dirtyRects.Add(m_image.GetComponent<RectTransform>());
        m_dirtyRects.Add(m_text.GetComponent<RectTransform>());

        TryGetComponent(out m_rectTransform);
        TryGetComponent(out m_animator);
    }

    public void Init(string text, UnityAction onClick)
    {
        m_text.text = text;
        m_button.onClick.AddListener(onClick);
    }

    public void SetDisplay(bool shouldShow)
    {
        Debug.Log("SetDisplay " + shouldShow);
        m_image.enabled = shouldShow;
        m_text.enabled = shouldShow;
        m_animator.SetBool(m_showParam, shouldShow);
        if (!shouldShow)
        {
            m_button.onClick.RemoveAllListeners();
        }
        else
        {
            StartCoroutine(DoNextFrame(() => Rebuild()));
        }
    }

    private void Rebuild()
    {
        foreach (var d in m_dirtyRects)
            LayoutRebuilder.MarkLayoutForRebuild(d);

        LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rectTransform);
    }

    IEnumerator DoNextFrame(System.Action cb)
    {
        yield return new WaitForEndOfFrame();
        cb.Invoke();
    }
}
