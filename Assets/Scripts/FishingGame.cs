using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

[System.Serializable]
public class IntStringDictionary : SerializableDictionary<int, string> { }

public class FishingGame : MonoBehaviour
{
    int m_counter = 0;
    [SerializeField]
    TextMeshProUGUI m_text = null;

    [SerializeField]
    Button m_button = null;

    [SerializeField]
    IntStringDictionary m_scoreToThought = new IntStringDictionary();

    private void UpdateText()
    {
        m_text.text = string.Format("Fish Caught: {0}", m_counter);
        if (m_scoreToThought.ContainsKey(m_counter))
        {
            EventManagement.Instance.TriggerThought(m_scoreToThought[m_counter]);
        }

        if (m_counter > 100)
        {
            GameStateManager.Instance.SetLastThought("fish");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_button.onClick.AddListener(() => { m_counter++; UpdateText(); });
    }
}
