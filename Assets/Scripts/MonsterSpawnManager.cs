using UnityEngine;
using System.Collections;

public class MonsterSpawnManager : MonoBehaviour
{
    [Header("References")]
    public GameObject monsterPrefab;         // Drag your crawler‐monster prefab here
    public Transform player;                 // Drag in the Player transform
    public AudioClip clickerSFX;             // TLOU “Clicker” sound effect

    [Header("Spawn Timing")]
    public float minSpawnInterval = 20f;     // minimum seconds between spawns
    public float maxSpawnInterval = 40f;     // maximum seconds between spawns

    [Header("Spawn Distance")]
    public float spawnMinRadius = 10f;       // minimal distance behind player
    public float spawnMaxRadius = 20f;       // maximal distance behind player
    public float heightOffset = 0f;          // if your monster should spawn at some Y offset

    private bool _isSpawning = true;

    private void Start()
    {
        if (player == null && Camera.main != null)
            player = Camera.main.transform;  // fallback if you forgot to assign

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (_isSpawning)
        {
            float delay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);

            SpawnBehindPlayer();
        }
    }

    private void SpawnBehindPlayer()
    {
        if (monsterPrefab == null || player == null) return;

        // 1) Pick a random angle between 150° and 210° (i.e. “behind” the player)
        float angleDeg = Random.Range(150f, 210f);
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // 2) Pick a random radius between spawnMinRadius and spawnMaxRadius
        float radius = Random.Range(spawnMinRadius, spawnMaxRadius);

        // 3) Compute a spawn point in world space
        Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * radius;
        Vector3 spawnPos = player.position + offset + new Vector3(0f, heightOffset, 0f);

        // 4) Raycast down to terrain or floor so monster lands on ground
        RaycastHit hit;
        if (Physics.Raycast(spawnPos + Vector3.up * 20f, Vector3.down, out hit, 50f, LayerMask.GetMask("Default", "Ground")))
        {
            spawnPos.y = hit.point.y;
        }

        // 5) Instantiate the monster
        GameObject monsterGO = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        MonsterController mc = monsterGO.GetComponent<MonsterController>();
        if (mc != null)
        {
            mc.InitializeForSpawn(player, clickerSFX);
        }
    }

    /// <summary>
    /// Call this to stop spawning (e.g. on Game Over).
    /// </summary>
    public void StopSpawning()
    {
        _isSpawning = false;
        StopAllCoroutines();
    }
}
