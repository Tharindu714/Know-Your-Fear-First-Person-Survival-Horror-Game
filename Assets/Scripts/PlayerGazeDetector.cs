using UnityEngine;

public class PlayerGazeDetector : MonoBehaviour
{
    [Header("Gaze Settings")]
    public float gazeCheckInterval = 0.2f;       // how often to raycast
    public float fearPerSecond = 10f;            // fear/sec when looking
    public float healthDamagePerSecond = 5f;     // health/sec when looking

    public LayerMask monsterLayer;               // assign “MonsterLayer” in Inspector

    private float gazeTimer = 0f;
    private PlayerHealth playerHealth;
    private Camera cam;                          // reference to main camera

    private void Awake()
    {
        // Instead of GetComponent<Camera>(), use Camera.main:
        cam = Camera.main;
        playerHealth = FindObjectOfType<PlayerHealth>();
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
            Debug.LogWarning("PlayerGazeDetector: no Camera.main found.");
            return;
        }

        // Raycast from the camera’s position forward
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, monsterLayer))
        {
            // We hit something on the “monsterLayer”
            // e.g. the monster GameObject is on that layer
            FearManager.I.NotifyGaze(fearPerSecond * gazeCheckInterval);

            if (playerHealth != null)
                playerHealth.TakeDamage(healthDamagePerSecond * gazeCheckInterval);
        }
    }
}
