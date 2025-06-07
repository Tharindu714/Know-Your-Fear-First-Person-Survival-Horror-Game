using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandleGhostManager : MonoBehaviour
{
    [Header("Timing (between candle hauntings)")]
    public float minInterval = 10f;
    public float maxInterval = 30f;

    [Header("Candle List (auto‐populated)")]
    private List<CandleController> candles = new List<CandleController>();

    [Header("Whisper SFX")]
    public AudioClip whisperClip;           // whisper sound (for haunt)
    public AudioSource audioSourcePrefab;   // prefab containing an AudioSource (3D)

    [Header("Shadow Settings")]
    public GameObject shadowPrefab;         // your shadow‐silhouette prefab
    public float spawnDistance = 3f;        // how far in front of the player
    public float horizontalJitter = 1f;     // small random left/right
    public float verticalJitter = 0.5f;     // small random up/down

    private Transform playerCam;
    public PlayerHealth playerHealth;       // assign your Player (with PlayerHealth) here

    void Awake()
    {
        // 1) Find every CandleController in the scene
        candles.AddRange(FindObjectsOfType<CandleController>());

        // 2) Cache main camera’s transform for whisper + shadow
        if (Camera.main != null)
            playerCam = Camera.main.transform;
    }

    void Start()
    {
        StartCoroutine(GhostLoop());
    }

    private IEnumerator GhostLoop()
    {
        while (true)
        {
            // 1) Wait a random interval
            float delay = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(delay);

            while (Instruction.IsReading)
                yield return null;

            // 2) Gather all currently lit candles
            var litCandles = candles.FindAll(c => c != null && c.isLit);
            if (litCandles.Count == 0)
                continue;

            // 3) Pick one at random & extinguish it
            CandleController victim = litCandles[Random.Range(0, litCandles.Count)];

            // ← Insert jumpscare cue 2 seconds before:
            MusicManager.I?.PlayJumpCue();
            yield return new WaitForSeconds(3f);

            victim.ToggleCandle(); // extinguishes if currently lit

            // 4) Remove from our list and destroy the candle GameObject
            candles.Remove(victim);
            Destroy(victim.gameObject);

            // 5) Play a whisper at the player's position (the haunting event)
            if (whisperClip != null && audioSourcePrefab != null && playerCam != null)
            {
                AudioSource src = Instantiate(audioSourcePrefab, playerCam.position, Quaternion.identity);
                src.clip = whisperClip;
                src.spatialBlend = 1f; // fully 3D
                src.Play();
                Destroy(src.gameObject, whisperClip.length + 0.1f);
            }

            // 6) Register the jumpscare
            JumpScareManager.Instance.RegisterJumpScare();

            // 7) Spawn a “shadow” silhouette in front of the player
            if (shadowPrefab != null && playerCam != null)
            {
                Vector3 spawnPos = playerCam.position
                    + playerCam.forward * spawnDistance
                    + playerCam.right * Random.Range(-horizontalJitter, horizontalJitter)
                    + playerCam.up * Random.Range(-verticalJitter, verticalJitter);

                GameObject shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

                // Rotate to face the camera
                Vector3 dirToCam = (playerCam.position - spawnPos).normalized;
                shadow.transform.rotation = Quaternion.LookRotation(dirToCam, Vector3.up);
            }
        }
    }
}
