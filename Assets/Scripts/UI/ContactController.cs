using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class ContactController : MonoBehaviour
{
    [SerializeField]
    private Image m_image = null;
    [SerializeField]
    private TextMeshProUGUI m_name = null;
    [SerializeField]
    private TextMeshProUGUI m_lastMsg = null;
    [SerializeField]
    private Image m_notifIcon = null;
    [SerializeField]
    private TextMeshProUGUI m_timestamp = null;
    [SerializeField]
    private Button m_button = null;

    TimeManager m_timerManager = null;
    CustomDialogueUI m_dialogueUI = null;

    private void Awake()
    {
        m_timerManager = TimeManager.Instance;
        if (m_timerManager == null)
        {
            Debug.LogWarningFormat("TimeManager singleton is null during awake of {0}", name);
        }
    }

    public void Init(
        ContactModel model,
        AppController textingApp,
        MessageListController messageListController,
        TextingTopPanel panel)
    {
        m_image.sprite = model.ProfilePic;
        m_name.text = model.HasUnreadMsg ?
            string.Format("<b>{0}</b>", model.Name) : model.Name;
        m_lastMsg.text = model.HasUnreadMsg ?
             string.Format("<b>{0}</b>", model.LastMessage) : model.LastMessage;
        m_dialogueUI = model.DialogueUI;
        m_dialogueUI.CharID = model.CharID;
        m_notifIcon.enabled = model.HasUnreadMsg;

        m_timestamp.text = GetDateTextFromTimeDiff(m_timerManager.GetCurrentTime(), model.LastMessageReceivedTime);

        m_button.onClick.RemoveAllListeners();
        m_button.onClick.AddListener(() =>
        {
            panel.Init(model);
            messageListController.Init(model.CharID, m_dialogueUI);
            textingApp.SetShouldShow(true);
        });
    }

    private string GetDateTextFromTimeDiff(System.DateTime now, System.DateTime lastMsgTime)
    {
        var diff = now - lastMsgTime;
        var days = diff.Days;
        var years = days / 365;
        if (years > 0)
        {
            return string.Format("{0}y", years);
        }
        var weeks = days / 7;
        if (weeks > 0)
        {
            return string.Format("{0}w", weeks);
        }
        if (days > 0)
        {
            return string.Format("{0}d", days);
        }
        var hours = diff.Hours;
        if (hours > 0)
        {
            return string.Format("{0}h", hours);
        }

        var mins = diff.Minutes;
        if (mins > 0)
        {
            return string.Format("{0}m", mins);
        }

        return "1m";
    }
}
