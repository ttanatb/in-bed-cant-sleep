#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(StringContactModelDictionary))]
public class StringContactModelDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
#endif