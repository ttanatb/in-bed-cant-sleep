using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;
using Yarn.Unity;

public class OptionListController : MonoBehaviour
{
    OptionButtonController[] m_buttons = null;

    // Start is called before the first frame update
    void Start()
    {
        m_buttons = GetComponentsInChildren<OptionButtonController>();
        Hide();
    }

    public void ShowOptions(DialogueUI ui, OptionsModel[] model, string charID)
    {
        if (model.Length > m_buttons.Length)
        {
            Debug.LogErrorFormat(" More than {0} options, can't display all", m_buttons.Length);
        }

        int count = Mathf.Min(m_buttons.Length, model.Length);
        for (int i = 0; i < count; i++)
        {
            var o = model[i];
            var b = m_buttons[i];

            b.Init(o.Text, () =>
            {
                ui.SelectOption(o.ID);
                EventManagement.Instance.TriggerOptions(charID, null);
            });
            b.SetDisplay(true);
        }

        for (int i = count; i < m_buttons.Length; i++)
        {
            m_buttons[i].SetDisplay(false);
        }
    }

    public void Hide()
    {
        foreach (var b in m_buttons)
        {
            b.SetDisplay(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
