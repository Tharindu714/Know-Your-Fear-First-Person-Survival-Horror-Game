using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Good Toy Object", menuName = "Items/Good_toys")]

public class GoodToyObject : ItemObject
{
    public int HealthLevel;
    public void Awake()
    {
        type = ItemType.Toys;
    }
}
