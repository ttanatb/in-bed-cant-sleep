using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class ThoughtController : MonoBehaviour
{

    Queue<string> m_thoughtQueue = new Queue<string>();
    EventManagement m_eventManger = null;

    [SerializeField]
    private Image m_image = null;
    [SerializeField]
    TextMeshProUGUI m_text = null;
    [SerializeField]
    Animator m_animator = null;

    [AnimatorParam("m_animator")]
    [SerializeField]
    int m_isShowingParam = 1;

    Button m_button = null;

    bool m_isDiplaying = false;
    private List<RectTransform> m_dirtyRects = new List<RectTransform>();
    private RectTransform m_rectTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        m_eventManger = EventManagement.Instance;

        m_eventManger.AddThoughtListner(OnThought);
        TryGetComponent(out m_button);

        m_button.onClick.AddListener(RequestNextThought);
        DismissCurrentThought();

        TryGetComponent(out m_rectTransform);
        m_dirtyRects.Add(m_button.GetComponent<RectTransform>());
        m_dirtyRects.Add(m_image.GetComponent<RectTransform>());
        m_dirtyRects.Add(m_text.GetComponent<RectTransform>());
    }

    private void OnThought(string thought)
    {
        m_thoughtQueue.Enqueue(thought);
        if (!m_isDiplaying)
            DisplayThought();
    }

    private void DisplayThought()
    {
        string thought = m_thoughtQueue.Dequeue();
        m_text.text = thought;
        m_animator.SetBool(m_isShowingParam, true);
        m_button.interactable = true;
        m_isDiplaying = true;
        StartCoroutine(DoNextFrame(() => Rebuild()));

    }

    private void RequestNextThought()
    {
        if (m_thoughtQueue.Count == 0)
        {
            DismissCurrentThought();
            return;
        }

        string thought = m_thoughtQueue.Dequeue();
        m_text.text = thought;
        m_animator.SetBool(m_isShowingParam, true);
        m_button.interactable = true;
        StartCoroutine(DoNextFrame(() => Rebuild()));

    }

    private void DismissCurrentThought()
    {
        m_button.interactable = false;
        m_animator.SetBool(m_isShowingParam, false);
        m_isDiplaying = false;
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
