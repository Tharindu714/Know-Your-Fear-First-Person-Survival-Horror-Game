using UnityEngine;

public class PlayerGazeDetector : MonoBehaviour
{
    [Header("Gaze Settings")]
    [Tooltip("How often (in seconds) to raycast forward and check for monster hits.")]
    public float gazeCheckInterval = 0.2f;

    [Tooltip("Amount of fear to add PER SECOND while looking at a monster.")]
    public float fearPerSecond = 10f;

    [Tooltip("Amount of health to remove PER SECOND while looking at a monster.")]
    public float healthDamagePerSecond = 5f;

    [Tooltip("LayerMask that contains your monster objects.")]
    public LayerMask monsterLayer;

    private float gazeTimer = 0f;
    private PlayerHealth playerHealth;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth == null)
            Debug.LogError("PlayerGazeDetector: No PlayerHealth found in scene.");
    }

    private void Update()
    {
        gazeTimer += Time.deltaTime;
        if (gazeTimer >= gazeCheckInterval)
        {
            gazeTimer = 0f;
            CheckGaze();
        }
    }

    private void CheckGaze()
    {
        if (cam == null)
        {
            Debug.LogWarning("PlayerGazeDetector: Camera.main is null.");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, monsterLayer))
        {
            // We hit a collider on "monsterLayer"
            // Add fear directly to PlayerHealth:
            if (playerHealth != null)
                playerHealth.IncreaseFear(fearPerSecond * gazeCheckInterval);

            // Damage player health over time
            if (playerHealth != null)
                playerHealth.TakeDamage(healthDamagePerSecond * gazeCheckInterval);
        }
    }
}
