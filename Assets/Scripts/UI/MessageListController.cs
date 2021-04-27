using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;

public class MessageListController : MonoBehaviour
{
    [SerializeField]
    [ReorderableList]
    private List<TextMessageModel> m_models = new List<TextMessageModel>();

    [SerializeField]
    private GameObject m_textBubblePrefab = null;

    [SerializeField]
    private int m_maxCount = 512;
    private List<MessageController> m_textBubbleList = new List<MessageController>();
    private MessageListDatabase m_database = null;
    [ShowNonSerializedField]
    private string m_charID = "";

    public string CharID { get { return m_charID; } }

    [SerializeField]
    private GameObject m_textTypingPrefab = null;
    private GameObject m_textTyping = null;
    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    private GameObject m_paddingPrefab = null;

    [SerializeField]
    private OptionListController m_optionController = null;
    private CustomDialogueUI m_dialogueUI = null;
    [SerializeField]
    private ContactListController m_contactList = null;
    bool m_shown = false;

    // Start is called before the first frame update
    void Start()
    {
        var m_eventManager = EventManagement.Instance;

        m_textBubbleList = new List<MessageController>(m_maxCount);
        for (int i = 0; i < m_maxCount; i++)
        {
            var obj = GameObject.Instantiate(
                m_textBubblePrefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            obj.TryGetComponent(out MessageController t);
            m_textBubbleList.Add(t);
        }

        m_textTyping = GameObject.Instantiate(
            m_textTypingPrefab, Vector3.zero, Quaternion.identity, transform);
        m_textTyping.SetActive(false);
        GameObject.Instantiate(
            m_paddingPrefab, Vector3.zero, Quaternion.identity, transform);

        var appController = GetComponentInParent<AppController>();
        m_database = MessageListDatabase.Instance;

        m_eventManager.AddShowTypingListener(ShowTyping);
        m_eventManager.AddOptionsListener(OnOpts);

        appController.AddOnShowListner(() =>
        {
            m_shown = true;
            ScrollToBot();
            CheckForOptions();
        });

        appController.AddOnHideListner(() =>
        {
            m_shown = false;
            StopAllCoroutines();
            m_optionController.Hide();
            m_textTyping.SetActive(false);
        });
    }

    public void Init(string charID, CustomDialogueUI dialogueUI)
    {
        m_models = m_database.GetList(charID);
        m_charID = charID;
        m_dialogueUI = dialogueUI;
        InitListFromModel();
        InitListFromModel();
    }

    public void MaybeRefresh(string charID)
    {
        if (m_charID != charID)
            return;

        InitListFromModel();
        StartCoroutine(DoWait(2 * Time.deltaTime, () => ScrollToBot()));
    }

    public void ScrollToBot()
    {
        m_scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    [Button]
    private void InitListFromModel()
    {
        for (int i = 0; i < m_textBubbleList.Count; i++)
        {
            var c = m_textBubbleList[i];
            if (i >= m_models.Count)
            {
                c.gameObject.SetActive(false);
            }
            else
            {
                c.gameObject.SetActive(true);
                c.Init(m_models[i]);
            }
        }
    }

    private void ShowTyping(string id, int secAmt)
    {
        if (m_charID == id)
        {
            StopAllCoroutines();

            m_textTyping.SetActive(true);
            StartCoroutine(DoWait(secAmt, () => m_textTyping.SetActive(false)));
            // hack to scroll after a frame
            StartCoroutine(DoWait(2 * Time.deltaTime, () => ScrollToBot()));
        }
    }

    private void OnOpts(string id, OptionsModel[] options)
    {
        if (m_charID != id || !m_shown)
        {
            return;
        }

        if (options == null || options.Length == 0)
        {
            m_optionController.Hide();
        }
        else
        {
            m_optionController.ShowOptions(m_dialogueUI, options, m_charID);
        }
    }

    private void CheckForOptions()
    {
        var model = m_contactList.GetContact(m_charID);
        if (model.Options != null)
        {
            m_optionController.ShowOptions(m_dialogueUI, model.Options, m_charID);
        }
    }

    private IEnumerator DoWait(float amount, System.Action onComplete)
    {
        yield return new WaitForSeconds(amount);
        onComplete();
    }
}
