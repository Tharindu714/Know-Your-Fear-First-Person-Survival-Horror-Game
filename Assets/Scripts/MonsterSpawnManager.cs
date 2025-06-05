// File: MonsterSpawnManager.cs
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    [Header("References")]
    public GameObject crawlingMonsterPrefab;  // → drag your CrawlingMonster prefab here
    public Transform[] hidePositions;         // → same hide spots array you used above
    public Transform playerCameraTransform;   // → drag MainCamera.transform here

    private GameObject monsterInstance;

    private void Awake()
    {
        if (crawlingMonsterPrefab == null || playerCameraTransform == null || hidePositions.Length == 0)
        {
            Debug.LogError("MonsterSpawnManager: Missing references!");
            return;
        }

        // Instantiate it disabled
        monsterInstance = Instantiate(crawlingMonsterPrefab, Vector3.zero, Quaternion.identity);
        monsterInstance.SetActive(false);

        // Assign references on the component
        CrawlingMonster cm = monsterInstance.GetComponent<CrawlingMonster>();
        if (cm != null)
        {
            cm.playerCameraTransform = playerCameraTransform;
            cm.monsterAnimator = monsterInstance.GetComponent<Animator>();
            cm.hidePositions = hidePositions;
        }
        else
        {
            Debug.LogError("MonsterSpawnManager: CrawlingMonster component missing on prefab!");
        }
    }
}
