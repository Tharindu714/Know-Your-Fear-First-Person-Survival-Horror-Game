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
            _playerHealth = _playerTransform.GetComponent<PlayerHealth>();
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

        // If we never hit the player, just destroy ourselves now
        if (!_hasHitPlayer)
        {
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
            }

            // 3) Destroy this ghost shortly after the hit sound finishes
            float destroyDelay = (hitClip != null) ? hitClip.length + 0.1f : 0f;
            Destroy(gameObject, destroyDelay);
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
