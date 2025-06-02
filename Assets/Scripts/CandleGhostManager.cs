using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleGhostManager : MonoBehaviour
{
    [Header("Timing (between candle hauntings)")]
    public float minInterval = 10f;
    public float maxInterval = 30f;

    [Header("Candle List (auto-populated)")]
    private List<CandleLightController> candles = new List<CandleLightController>();

    [Header("Whisper SFX")]
    public AudioClip whisperClip;
    public AudioSource audioSourcePrefab;  // prefab with AudioSource (3D if you like)

    [Header("Shadow Settings")]
    public GameObject shadowPrefab;        // your shadow?silhouette prefab
    public float spawnDistance = 3f;       // distance in front of player
    public float horizontalJitter = 1f;
    public float verticalJitter = 0.5f;
    public float screamRadius = 20f; // how far the sound reaches

    private Transform playerCam;

    void Awake()
    {
        // Cache all your candles
        candles.AddRange(FindObjectsOfType<CandleLightController>());
        playerCam = Camera.main.transform;
    }

    void Start()
    {
        StartCoroutine(GhostLoop());
    }
    
        private void OnEnable()
    {
        Candle.OnCandleLit += HandleCandleLit;
    }
    private void OnDisable()
    {
        Candle.OnCandleLit -= HandleCandleLit;
    }

    private void HandleCandleLit()
    {
        // Play scream SFX at camera (so player hears it clearly)
        AudioSource.PlayClipAtPoint(whisperClip, Camera.main.transform.position);

        // Find the monster and force it into chase
        MonsterController monster = FindObjectOfType<MonsterController>();
        if (monster != null)
            monster.ForceChase();
    }

    private IEnumerator GhostLoop()
    {
        while (true)
        {
            // 1) Wait a random time
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            // 2) Pick a random lit candle
            var lit = candles.FindAll(c => c.isLit);
            if (lit.Count == 0)
                continue;

            var victim = lit[Random.Range(0, lit.Count)];

            // 3) Extinguish it
            victim.Extinguish();

            // 4) Play whisper at the player
            if (whisperClip != null && audioSourcePrefab != null)
            {
                var src = Instantiate(audioSourcePrefab, playerCam.position, Quaternion.identity);
                src.clip = whisperClip;
                src.Play();
                Destroy(src.gameObject, whisperClip.length + 0.1f);
            }

            // after you Play whisper and Extinguish candle:
            FearManager.I.NotifyJumpscare();

            FindObjectOfType<FearEffectsManager>()?.OnFearUpdated(FearManager.I._jumpscareCount);


            // 5) Spawn a shadow in front of the player
            if (shadowPrefab != null)
            {
                Vector3 spawnPos = playerCam.position
                    + playerCam.forward * spawnDistance
                    + playerCam.right * Random.Range(-horizontalJitter, horizontalJitter)
                    + playerCam.up * Random.Range(-verticalJitter, verticalJitter);

                var shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

                // AFTER instantiating shadow at spawnPos:
                Vector3 directionToCam = (playerCam.position - spawnPos).normalized;
                shadow.transform.rotation = Quaternion.LookRotation(directionToCam, Vector3.up);

            }
        }
    }
}

