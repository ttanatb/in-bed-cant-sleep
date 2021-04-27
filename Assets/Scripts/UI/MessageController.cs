using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class MessageController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_leftText = null;
    [SerializeField]
    TextMeshProUGUI m_rightText = null;
    [SerializeField]
    TextMeshProUGUI m_dateText = null;
    [SerializeField]
    RectTransform m_leftImgRectTransform = null;
    [SerializeField]
    RectTransform m_rightImgRectTransform = null;

    RectTransform m_leftTextRectTransform = null;
    RectTransform m_rightTextRectTransform = null;
    RectTransform m_dateTextRectTransform = null;
    RectTransform m_rectTransform = null;
    HorizontalLayoutGroup m_layoutGroup = null;

    private void Awake()
    {
        TryGetComponent(out m_rectTransform);
        TryGetComponent(out m_layoutGroup);
        m_leftText.TryGetComponent(out m_leftTextRectTransform);
        m_rightText.TryGetComponent(out m_rightTextRectTransform);
        m_dateText.TryGetComponent(out m_dateTextRectTransform);
    }

    public void Init(TextMessageModel model)
    {
        if (model.Position == TextMessageModel.Pos.Left)
        {
            SetModel(model, m_leftText, m_leftTextRectTransform, m_leftImgRectTransform);
            m_layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            m_rightImgRectTransform.gameObject.SetActive(false);
            m_dateTextRectTransform.gameObject.SetActive(false);
        }
        else if (model.Position == TextMessageModel.Pos.Right)
        {
            SetModel(model, m_rightText, m_rightTextRectTransform, m_rightImgRectTransform);
            m_layoutGroup.childAlignment = TextAnchor.MiddleRight;
            m_leftImgRectTransform.gameObject.SetActive(false);
            m_dateTextRectTransform.gameObject.SetActive(false);
        }
        else if (model.Position == TextMessageModel.Pos.Middle)
        {
            m_dateText.text = model.Text;
            m_layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            LayoutRebuilder.MarkLayoutForRebuild(m_dateTextRectTransform);
            m_dateTextRectTransform.gameObject.SetActive(true);
            m_leftImgRectTransform.gameObject.SetActive(false);
            m_rightImgRectTransform.gameObject.SetActive(false);
        }
        LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rectTransform);
    }

    private void SetModel(TextMessageModel model,
        TextMeshProUGUI text, RectTransform textTransform, RectTransform imageTransform)
    {
        imageTransform.gameObject.SetActive(true);
        text.text = model.Text;
        LayoutRebuilder.MarkLayoutForRebuild(textTransform);
        LayoutRebuilder.MarkLayoutForRebuild(imageTransform);
    }
}
