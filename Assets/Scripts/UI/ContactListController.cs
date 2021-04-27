using System.Collections;
using Yarn.Unity;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class StringContactModelDictionary : SerializableDictionary<string, ContactModel> { }

public class ContactListController : MonoBehaviour
{
    [SerializeField]
    [ReorderableList]
    private List<string> m_sortedIndex = new List<string>();

    [SerializeField]
    private StringContactModelDictionary m_database = new StringContactModelDictionary();

    [SerializeField]
    private GameObject m_contactsPrefab = null;

    [SerializeField]
    private int m_maxCount = 32;

    private List<ContactController> m_contactList = new List<ContactController>();

    [SerializeField]
    private AppController m_textingApp = null;
    [SerializeField]
    private TextingTopPanel m_textingAppPanel = null;
    private MessageListController m_messageListController = null;

    private YarnRunnerRunner m_runner = null;
    private MessageListDatabase m_messageDatabase = null;
    private EventManagement m_eventManager = null;
    private TimeManager m_timeManager = null;
    private MessageListDatabase m_msgListDatabase = null;

    [SerializeField]
    private YarnProgram m_program = null;

    [SerializeField]
    private ContactsModelSO m_contactsModelSO = null;

    // Start is called before the first frame update
    void Start()
    {
        m_eventManager = EventManagement.Instance;
        m_msgListDatabase = MessageListDatabase.Instance;
        m_timeManager = TimeManager.Instance;

        m_contactList = new List<ContactController>(m_maxCount);
        for (int i = 0; i < m_maxCount; i++)
        {
            var obj = GameObject.Instantiate(m_contactsPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            obj.TryGetComponent(out ContactController c);
            m_contactList.Add(c);
        }

        var appController = GetComponentInParent<AppController>();
        appController.AddOnShowListner(() => Refresh());

        m_textingApp.AddOnHideListner(() =>
        {
            var m = m_database[m_messageListController.CharID];
            m.HasUnreadMsg = false;
            m_database[m_messageListController.CharID] = m;
            Refresh();
        });
        m_messageListController = m_textingApp.GetComponentInChildren<MessageListController>();

        // m_database.Add("test", new ContactModel()
        // {
        //     Name = "TEST NAME",
        //     CharID = "test",
        //     LogNode = "test_person_log"
        // });
        for (int i = 0; i < m_contactsModelSO.Models.Count; i++)
        {
            var m = m_contactsModelSO.Models[i];
            m_database.Add(m.CharID, m);
        }

        m_runner = YarnRunnerRunner.Instance;
        m_messageDatabase = MessageListDatabase.Instance;
        m_sortedIndex = new List<string>(m_database.Count);

        Dictionary<string, ContactModel> copy = new Dictionary<string, ContactModel>();
        foreach (var key in m_database.Keys)
        {
            var model = m_database[key];
            m_runner.Register(model.CharID, out model.DialogueRunner, out model.DialogueUI);
            model.DialogueRunner.Add(m_program);
            copy.Add(key, model);
            m_messageDatabase.InitLog(model.CharID, model.LogNode, model.DialogueRunner, model.DialogueUI);

            m_sortedIndex.Add(key);
        }

        m_database.CopyFrom(copy);

        m_eventManager.AddCharEventFlagListener(EventFlag.DoneLoadingLog, OnLogDoneLoading);
        m_eventManager.AddCharEventFlagListener(EventFlag.SetUnread, SetUnread);
        m_eventManager.AddSetTimestampListener(OnTimestamp);
        m_eventManager.AddOptionsListener(OnOpts);
    }

    private void OnTimestamp(string charID, System.DateTime dateTime)
    {
        var model = m_database[charID];
        model.LastMessageReceivedTime = dateTime;
        m_database[charID] = model;
        Refresh();
    }

    private void SetUnread(string charID)
    {
        var model = m_database[charID];
        model.HasUnreadMsg = true;
        m_database[charID] = model;
        Refresh();
    }

    private void OnOpts(string charID, OptionsModel[] opts)
    {
        Debug.LogFormat("OnOpts with id {0}: {1}", charID, opts);
        var model = m_database[charID];
        model.Options = opts;
        m_database[charID] = model;
    }

    public ContactModel GetContact(string charID)
    {
        return m_database[charID];
    }

    private void OnLogDoneLoading(string charID)
    {
        Debug.Log("Done loading log for : " + charID);
        UpdateModel(charID,/*hasUnread=*/ false, /*useCurrentTime=*/false);
        var model = m_database[charID];

        if (model.StartingNode != "")
        {
            model.DialogueUI.onLineUpdate.AddListener((line) =>
            {
                var msg = MessageListDatabase.Parse(line);
                if (msg.Position == TextMessageModel.Pos.Thought)
                {
                    m_eventManager.TriggerThought(msg.Text);
                    StartCoroutine(DoWaitEndOfFrame(() => model.DialogueUI.MarkLineComplete()));
                }
                else
                {
                    m_msgListDatabase.Append(charID, msg);
                    if (msg.Position != TextMessageModel.Pos.Middle)
                    {
                        UpdateModel(charID, /*hasUnread=*/ true, /*useCurrentTime=*/true);
                        Refresh();
                        m_messageListController.MaybeRefresh(charID);
                        StartCoroutine(DoWaitEndOfFrame(() => model.DialogueUI.MarkLineComplete()));
                        Caw.Instance.Play();
                    }
                    else
                    {
                        StartCoroutine(DoWait(Random.Range(1.0f, 2.0f), () => model.DialogueUI.MarkLineComplete()));
                    }
                }
            });
            model.DialogueRunner.startNode = model.StartingNode;

            //hack to get things to work
            StartCoroutine(DoWaitEndOfFrame(() => model.DialogueRunner.StartDialogue(model.StartingNode)));
        }
        else
        {
            Debug.LogWarningFormat("Model with id {0} has no starting node", model.CharID);
        }
    }

    private IEnumerator DoWait(float sec, System.Action onComplete)
    {
        yield return new WaitForSeconds(sec);
        onComplete();
    }

    private IEnumerator DoWaitEndOfFrame(System.Action onComplete)
    {
        yield return new WaitForEndOfFrame();
        onComplete();
    }

    private void UpdateModel(string charID, bool hasUnread, bool useCurrentTime)
    {
        var model = m_database[charID];
        var list = m_messageDatabase.GetList(model.CharID);
        model.LastMessage = list[list.Count - 1].Text;
        model.HasUnreadMsg = hasUnread;
        if (useCurrentTime)
            model.LastMessageReceivedTime = m_timeManager.GetCurrentTime();
        m_database[charID] = model;
    }

    public void Refresh()
    {
        SortModels();
        InitListFromModel();
    }

    [Button]
    public void SortModels()
    {
        Debug.LogFormat("Sorting");
        foreach (var v in m_sortedIndex)
            Debug.LogFormat("data: {0}", v);

        m_sortedIndex.Sort(delegate (string lhsKey, string rhsKey)
        {
            var lhs = m_database[lhsKey];
            var rhs = m_database[rhsKey];
            Debug.LogFormat("lhs ts {0} vs rhs ts {1}",
                lhs.LastMessageReceivedTime, rhs.LastMessageReceivedTime);
            return rhs.LastMessageReceivedTime.CompareTo(lhs.LastMessageReceivedTime);
        });

        foreach (var v in m_sortedIndex)
            Debug.LogFormat("data: {0}", v);

    }

    [Button]
    public void InitListFromModel()
    {
        for (int i = 0; i < m_contactList.Count; i++)
        {
            var c = m_contactList[i];
            if (i >= m_sortedIndex.Count)
            {
                c.gameObject.SetActive(false);
            }
            else
            {
                c.gameObject.SetActive(true);
                c.Init(m_database[m_sortedIndex[i]], m_textingApp, m_messageListController, m_textingAppPanel);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
