using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Cursed Object", menuName = "Items/Cursed_toys")]

public class CursedObject : ItemObject
{
    public int FearLevel;
    public void Awake()
    {
        type = ItemType.Cursed_Toys;
    }
}

