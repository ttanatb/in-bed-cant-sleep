using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[System.Serializable]
public struct TextMessageModel
{
    public enum Pos
    {
        Invalid = 0,
        Left,
        Middle,
        Right,
        Thought
    }

    public string Text;// { get; set; }
    // public System.DateTime Timestamp;
    public Pos Position;
}
