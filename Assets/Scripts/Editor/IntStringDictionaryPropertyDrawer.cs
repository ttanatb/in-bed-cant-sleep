#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(IntStringDictionary))]
public class IntStringDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
#endif