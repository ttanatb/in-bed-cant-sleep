using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class Constants
{
    public static readonly HashSet<char> Space = new HashSet<char> { ' ', '\t' };
}

// [System.Serializable]
public class IdMessageListDictionary : SerializableDictionary<string, List<TextMessageModel>> { }

public class MessageListDatabase : Utilr.Singleton<MessageListDatabase>
{
    [SerializeField]
    private IdMessageListDictionary m_database = new IdMessageListDictionary();

    private EventManagement m_eventManager = null;

    public List<TextMessageModel> GetList(string id)
    {
        return m_database[id];
    }

    public void Append(string charID, TextMessageModel model)
    {
        m_database[charID].Add(model);
    }

    public void InitLog(string charID, string nodeName, DialogueRunner runner, DialogueUI ui)
    {
        UnityAction<string> updateCb = (line) =>
        {
            Append(charID, Parse(line));
        };
        UnityAction startCb = () =>
        {
            m_database.Add(charID, new List<TextMessageModel>());
        };
        UnityAction lineDoneCb = () =>
        {
            StartCoroutine(DoWaitEndOfFrame(() => ui.MarkLineComplete()));
        };
        ui.onLineUpdate.AddListener(updateCb);
        ui.onDialogueStart.AddListener(startCb);
        ui.onLineFinishDisplaying.AddListener(lineDoneCb);
        ui.onDialogueEnd.AddListener(() =>
        {
            // Debug.Log("end cb");
            ui.onDialogueStart.RemoveListener(startCb);
            ui.onLineUpdate.RemoveListener(updateCb);
            ui.onLineFinishDisplaying.RemoveListener(lineDoneCb);
            ui.onDialogueEnd.RemoveAllListeners();
            m_eventManager.TriggerCharEvent(charID, EventFlag.DoneLoadingLog);
        });
        runner.StartDialogue(nodeName);
    }

    public static TextMessageModel Parse(string line)
    {
        TextMessageModel m = new TextMessageModel();
        int indexOfColon = line.IndexOf(':');
        if (indexOfColon != -1)
        {
            var identifier = line.Substring(0, indexOfColon);
            identifier = RemoveLeadingTrailingWhiteSpace(identifier);
            switch (identifier)
            {
                case "l":
                case "L":
                    m.Position = TextMessageModel.Pos.Left;
                    break;
                case "c":
                case "C":
                case "m":
                case "M":
                    m.Position = TextMessageModel.Pos.Middle;
                    break;
                case "r":
                case "R":
                    m.Position = TextMessageModel.Pos.Right;
                    break;
                case "t":
                case "T":
                    m.Position = TextMessageModel.Pos.Thought;
                    break;
                default:
                    Debug.LogErrorFormat("Unrecognized identifier: {0}", identifier);
                    break;
            }

            line = line.Substring(indexOfColon + 1, line.Length - indexOfColon - 1);
            m.Text = RemoveLeadingTrailingWhiteSpace(line);
        }
        else
        {
            Debug.LogError("no : lines aren't supported yet: " + line);
        }
        return m;
    }
    private IEnumerator DoWaitEndOfFrame(System.Action onComplete)
    {
        yield return new WaitForEndOfFrame();
        onComplete();
    }

    /// <summary>
    /// Removes trailing and leading white space from a string.
    /// </summary>
    /// <param name="text">Text to trim.</param>
    /// <returns>Trimmed version of the string.</returns>
    private static string RemoveLeadingTrailingWhiteSpace(string text)
    {
        int removeCount = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (Constants.Space.Contains(text[i]))
            {
                removeCount++;
            }
            else
            {
                break;
            }
        }

        if (removeCount == text.Length)
            return "";

        if (removeCount > 0)
            text = text.Substring(removeCount);

        removeCount = 0;
        for (int i = text.Length - 1; i >= 0; i--)
        {
            if (Constants.Space.Contains(text[i]))
            {
                removeCount++;
            }
            else
            {
                break;
            }
        }

        if (removeCount > 0)
            text = text.Substring(0, text.Length - removeCount);

        return text;
    }

    private void Start()
    {
        m_eventManager = EventManagement.Instance;
        // m_database = new IdMessageListDictionary();
        // var list = new List<TextMessageModel>(){
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "July 2nd 1923",
        //         Position = TextMessageModel.Pos.Middle,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "July 2nd 1923",
        //         Position = TextMessageModel.Pos.Middle,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel() {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        // };
        // var list2 = new List<TextMessageModel>(){
        //     new TextMessageModel() {
        //         Text = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "July 2nd 1923",
        //         Position = TextMessageModel.Pos.Middle,
        //     },
        //     new TextMessageModel() {
        //         Text = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum lorem ipsum",
        //         Position = TextMessageModel.Pos.Right,
        //     },
        //     new TextMessageModel() {
        //         Text = "July 2nd 1923",
        //         Position = TextMessageModel.Pos.Middle,
        //     },
        //     new TextMessageModel()
        //     {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel()
        //     {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        //     new TextMessageModel()
        //     {
        //         Text = "asdfsdaf awe f",
        //         Position = TextMessageModel.Pos.Left,
        //     },
        // };
        // m_database.Add("test", list);
        // m_database.Add("test2", list2);
    }
}
