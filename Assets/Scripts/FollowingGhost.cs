using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class FollowingGhost : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast (units/sec) the ghost moves toward the player.")]
    public float moveSpeed = 5f;

    [Tooltip("How many seconds the ghost should chase before disappearing.")]
    public float followDuration = 5f;

    [Header("Damage & Fear")]
    [Tooltip("Damage to deal to the player on contact.")]
    public float damageOnHit = 25f;

    [Tooltip("Amount of fear to add to playerHealth on contact.")]
    public float fearOnHit = 20f;

    [Header("Miss Penalty (if never hits)")]
    [Tooltip("Health penalty to apply if the ghost never hits the player.")]
    public float missDamage = 5f;

    [Tooltip("Fear penalty to apply if the ghost never hits the player.")]
    public float missFear = 5f;

    [Header("Hit Sound")]
    [Tooltip("A short AudioClip to play when the ghost actually hits the player.")]
    public AudioClip hitClip;

    private Transform _playerTransform;
    private PlayerHealth _playerHealth;
    private AudioSource _audioSource;
    private bool _hasHitPlayer = false;

    /// <summary>
    /// Call this from the manager right after instantiating the ghost,
    /// so it knows which Transform to chase.
    /// </summary>
    public void Initialize(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        if (_playerTransform != null)
        {
            // If PlayerHealth is on a parent (e.g. CinemachineTarget is child), use GetComponentInParent
            _playerHealth = _playerTransform.GetComponentInParent<PlayerHealth>();
            if (_playerHealth == null)
                Debug.LogWarning("FollowingGhost: PlayerHealth not found on player!");
        }
        else
        {
            Debug.LogError("FollowingGhost: Initialize was called with a null playerTransform!");
        }

        // Set up own AudioSource for hit sound:
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = hitClip;
        _audioSource.spatialBlend = 1f; // fully 3D

        // Start the follow + self-destruct coroutine
        StartCoroutine(FollowAndVanish());
    }

    private IEnumerator FollowAndVanish()
    {
        float elapsed = 0f;
        while (elapsed < followDuration && !_hasHitPlayer)
        {
            elapsed += Time.deltaTime;

            if (_playerTransform != null)
            {
                // Move directly toward the player's current position
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Also rotate to face the player
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            yield return null;
        }

        // If we never hit the player during followDuration:
        if (!_hasHitPlayer)
        {
            // Apply a small miss penalty to the player
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(missDamage);
                _playerHealth.IncreaseFear(missFear);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only register a hit the first time
        if (_hasHitPlayer) return;

        if (other.CompareTag("Player"))
        {
            _hasHitPlayer = true;

            // 1) Deal damage + add fear
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(damageOnHit);
                _playerHealth.IncreaseFear(fearOnHit);
            }

            // 2) Play hit sound
            if (_audioSource != null && hitClip != null)
            {
                _audioSource.Play();
                // Destroy this ghost shortly after the hit sound finishes
                float destroyDelay = hitClip.length + 0.1f;
                Destroy(gameObject, destroyDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Reset()
    {
        // Ensure the Collider is set up properly
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        // Ensure a kinematic Rigidbody is present
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}
