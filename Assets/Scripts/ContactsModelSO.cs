using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ContactsModel", order = 1)]
public class ContactsModelSO : ScriptableObject
{
    [ReorderableList]
    public List<ContactModel> Models;
}
