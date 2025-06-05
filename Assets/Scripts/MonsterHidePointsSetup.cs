// File: MonsterHidePointsSetup.cs
using UnityEngine;
using System.Collections.Generic;

public class MonsterHidePointsSetup : MonoBehaviour
{
    [Header("Drag this into each script that needs hide points")]
    public Transform hidePointsParent;  // → drag "HidePointsParent" here

    [HideInInspector] public Transform[] hidePositions;

    private void Awake()
    {
        if (hidePointsParent == null)
        {
            Debug.LogError("MonsterHidePointsSetup: hidePointsParent is null!");
            return;
        }

        List<Transform> points = new List<Transform>();
        foreach (Transform child in hidePointsParent)
        {
            points.Add(child);
        }
        hidePositions = points.ToArray();

        // Now you can assign hidePositions to CrawlingMonster or MonsterSpawnManager:
        CrawlingMonster[] monsters = FindObjectsOfType<CrawlingMonster>();
        foreach (CrawlingMonster cm in monsters)
        {
            cm.hidePositions = hidePositions;
        }

        MonsterSpawnManager mngr = FindObjectOfType<MonsterSpawnManager>();
        if (mngr != null)
            mngr.hidePositions = hidePositions;
    }
}
