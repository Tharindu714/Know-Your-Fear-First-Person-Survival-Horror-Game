// File: MonsterSpawnManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterSpawnManager : MonoBehaviour
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
    [Tooltip("How far in front of the player (in world units) the ghost should spawn.")]
    public float spawnDistanceAhead = 5f;

    [Tooltip("How much random horizontal jitter to add to the spawn position.")]
    public float horizontalJitter = 2f;

    [Tooltip("How much random vertical jitter to add to the spawn position.")]
    public float verticalJitter = 1f;

    private Transform _playerTransform;

    private void Awake()
    {
        GameObject playerGO = GameObject.FindWithTag("Player");
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

            // 2) Calculate a spawn position in front of the player (plus random jitter)
            Vector3 ahead = _playerTransform.position 
                          + (_playerTransform.forward * spawnDistanceAhead);
            Vector3 jittered = ahead
                + (_playerTransform.right * Random.Range(-horizontalJitter, horizontalJitter))
                + (Vector3.up * Random.Range(-verticalJitter, verticalJitter));

            // 3) Force the ghost’s Y to exactly 0.054:
            Vector3 spawnPos = new Vector3(jittered.x, 0.05400002f, jittered.z);

            // 4) Instantiate the ghost prefab at spawnPos:
            GameObject ghostGO = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);

            // 5) Ensure it faces the player right away:
            Vector3 toPlayer = (_playerTransform.position - spawnPos).normalized;
            ghostGO.transform.rotation = Quaternion.LookRotation(toPlayer, Vector3.up);

            // 6) (Optional) play ambient hiss from an AudioSource prefab at spawnPos
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

            // 7) Tell the FollowingGhost component which Transform to chase:
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
