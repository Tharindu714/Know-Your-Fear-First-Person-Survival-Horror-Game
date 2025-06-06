using UnityEngine;
using System.Collections;

public class FollowingGhostManager : MonoBehaviour
{
    [Header("Spawn Timing")]
    [Tooltip("Minimum seconds to wait before spawning the next stalker-ghost.")]
    public float minSpawnInterval = 60f;

    [Tooltip("Maximum seconds to wait before spawning the next stalker-ghost.")]
    public float maxSpawnInterval = 120f;

    [Header("Ghost Prefab & Sound")]
    [Tooltip("Drag in a prefab that has the FollowingGhost component on it.")]
    public GameObject ghostPrefab;

    [Tooltip("Optional: a prefab or GameObject that contains an AudioSource set up for the ghost’s ambient/follow sound.")]
    public AudioSource ghostAudioSourcePrefab;

    [Header("Follow Settings")]
    [Tooltip("How far behind the player (in world units) the ghost should spawn.")]
    public float spawnDistanceBehind = 5f;

    [Tooltip("How much random horizontal jitter to add to the spawn position.")]
    public float horizontalJitter = 2f;

    [Tooltip("How much random vertical jitter to add to the spawn position.")]
    public float verticalJitter = 1f;

    private Transform _playerTransform;

    private void Awake()
    {
        GameObject playerGO = GameObject.FindWithTag("Player"); // or "Player" if you use that tag
        if (playerGO != null)
            _playerTransform = playerGO.transform;
        else
            Debug.LogError("FollowingGhostManager: No GameObject tagged 'Player' found in scene!");
    }

    private void Start()
    {
        if (_playerTransform == null || ghostPrefab == null)
        {
            enabled = false;
            return;
        }
        // Kick off the endless spawn loop:
        StartCoroutine(SpawnGhostLoop());
    }

    private IEnumerator SpawnGhostLoop()
    {
        while (true)
        {
            // 1) Wait between minSpawnInterval and maxSpawnInterval seconds:
            float delay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);

            // 3) Calculate a spawn position behind the player (plus random jitter)
            Vector3 behind = _playerTransform.position
                           - (_playerTransform.forward * spawnDistanceBehind);
            Vector3 jittered = behind
                + (_playerTransform.right * Random.Range(-horizontalJitter, horizontalJitter))
                + (Vector3.up * Random.Range(-verticalJitter, verticalJitter));

            // 4) Force the ghost’s Y to exactly 0.054
            Vector3 spawnPos = new Vector3(jittered.x, 0.05400002f, jittered.z);

            MusicManager.I?.PlayJumpCue();
            yield return new WaitForSeconds(2f);


            // 5) Instantiate the ghost prefab at spawnPos (not jittered)
            GameObject ghostGO = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);

            // 6) Rotate it so it faces the player (use spawnPos here too)
            Vector3 toPlayer = (_playerTransform.position - spawnPos).normalized;
            ghostGO.transform.rotation = Quaternion.LookRotation(toPlayer, Vector3.up);

            // 7) (Optional) play ambient hiss from an AudioSource prefab at spawnPos
            if (ghostAudioSourcePrefab != null)
            {
                AudioSource audio = Instantiate(
                    ghostAudioSourcePrefab,
                    spawnPos,
                    Quaternion.identity
                );
                audio.transform.SetParent(ghostGO.transform, worldPositionStays: false);
                audio.spatialBlend = 1f; // fully 3D
                audio.Play();
                Destroy(audio.gameObject, audio.clip.length + 0.1f);
            }

            // 8) Tell the FollowingGhost component which Transform to chase
            FollowingGhost fg = ghostGO.GetComponent<FollowingGhost>();
            if (fg != null)
            {
                fg.Initialize(_playerTransform);
            }
            else
            {
                Debug.LogWarning("Spawned ghostPrefab but no FollowingGhost component was found on it. Destroying.");
                Destroy(ghostGO);
            }
        }
    }
}
