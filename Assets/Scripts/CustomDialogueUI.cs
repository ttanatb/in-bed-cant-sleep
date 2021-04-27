using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;
using Yarn.Unity;
using Yarn;
using System;

public class CustomDialogueUI : DialogueUI
{
    private EventManagement m_eventManager = null;
    public string CharID { get; set; }

    public override void RunOptions(
        OptionSet optionSet, ILineLocalisationProvider localisationProvider, Action<int> onOptionSelected)
    {
        StartCoroutine(DoRunOptions(optionSet, localisationProvider, onOptionSelected));
    }


    /// Show a list of options, and wait for the player to make a
    /// selection.
    private IEnumerator DoRunOptions(
        Yarn.OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, System.Action<int> selectOption)
    {
        // Display each option in a button, and make it visible
        int i = 0;

        waitingForOptionSelection = true;
        currentOptionSelectionHandler = selectOption;

        OptionsModel[] model = new OptionsModel[optionsCollection.Options.Length];
        for (int j = 0; j < model.Length; j++)
        {
            model[j] = new OptionsModel()
            {
                Text = localisationProvider.GetLocalisedTextForLine(optionsCollection.Options[j].Line),
                ID = optionsCollection.Options[j].ID,
            };
        }

        EventManagement.Instance.TriggerOptions(CharID, model);

        // foreach (var optionString in optionsCollection.Options)
        // {
        //     optionButtons[i].gameObject.SetActive(true);

        //     // When the button is selected, tell the dialogue about it
        //     optionButtons[i].onClick.RemoveAllListeners();
        //     optionButtons[i].onClick.AddListener(() => SelectOption(optionString.ID));

        //     var optionText = localisationProvider.GetLocalisedTextForLine(optionString.Line);

        //     if (optionText == null)
        //     {
        //         Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
        //         optionText = optionString.Line.ID;
        //     }

        //     var unityText = optionButtons[i].GetComponentInChildren<Text>();
        //     if (unityText != null)
        //     {
        //         unityText.text = optionText;
        //     }

        //     var textMeshProText = optionButtons[i].GetComponentInChildren<TMPro.TMP_Text>();
        //     if (textMeshProText != null)
        //     {
        //         textMeshProText.text = optionText;
        //     }

        //     i++;
        // }

        onOptionsStart?.Invoke();

        // Wait until the chooser has been used and then removed 
        while (waitingForOptionSelection)
        {
            yield return null;
        }

        // Hide all the buttons
        // foreach (var button in optionButtons)
        // {
        //     button.gameObject.SetActive(false);
        // }

        onOptionsEnd?.Invoke();

    }
}
