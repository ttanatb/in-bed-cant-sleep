#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(IdMessageListDictionary))]
public class IdMessageListDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
#endif