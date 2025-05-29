using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Note Object", menuName = "Items/Notes")]

public class NoteObject : ItemObject
{
    public float StoreKnowledgeValue;
    public void Awake()
    {
        type = ItemType.Notes;
    }
}
