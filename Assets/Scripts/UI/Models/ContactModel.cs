using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Yarn.Unity;

[System.Serializable]
public struct ContactModel
{
    public Sprite ProfilePic;// { get; set; }
    public string Name;// { get; set; }
    public string LastMessage;// { get; set; }
    public bool HasUnreadMsg;// { get; set; }
    public System.DateTime LastMessageReceivedTime;
    public string CharID;// { get; set; }
    public string LogNode;
    public string StartingNode;
    public DialogueRunner DialogueRunner;
    public CustomDialogueUI DialogueUI;
    public OptionsModel[] Options;
}


[System.Serializable]
public struct OptionsModel
{
    public string Text;
    public int ID;
}
