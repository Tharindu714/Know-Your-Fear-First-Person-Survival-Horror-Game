using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CandleController : MonoBehaviour
{
    [Header("References")]
    public Light flameLight;          // Drag your Point Light here (the candle’s flame).
    // public Renderer flameRenderer;    // Drag the MeshRenderer (or ParticleRenderer) of the flame here.
    public PlayerHealth playerHealth; // Drag the Player (with PlayerHealth) here.

    [Header("Settings")]
    public float healPerSecond = 1f;   // How many HP to restore per second while near a lit candle.
    public float activationRadius = 2f; // How close the player must be to toggle the candle or be healed.

    [HideInInspector]
    public bool isLit = false;         // Starts off (dark).

    private bool playerInRange = false;
    private Coroutine healCoroutine;

    private void Start()
    {
        // Ensure the candle starts OFF:
        if (flameLight  != null) flameLight.enabled  = false;
        // if (flameRenderer != null) flameRenderer.enabled = false;

        // If you didn’t manually assign PlayerHealth, try to find it by tag:
        if (playerHealth == null && GameObject.FindWithTag("Player") != null)
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
            Debug.LogError($"CandleController on {name}: PlayerHealth reference is missing!");
    }

    private void Update()
    {
        // Only allow toggling if the player is within activationRadius:
        if (playerInRange && Input.GetKeyDown(KeyCode.L))
        {
            ToggleCandle();
        }
    }

    public void ToggleCandle()
    {
        isLit = !isLit;

        // Show or hide the flame visuals:
        if (flameLight != null)      flameLight.enabled  = isLit;
        // if (flameRenderer != null)   flameRenderer.enabled = isLit;

        // If we just lit it, start healing if the player is standing here.
        // If we just blew it out, stop healing.
        if (isLit && playerInRange)
        {
            StartHealing();
        }
        else
        {
            StopHealing();
        }
    }

    private void StartHealing()
    {
        if (healCoroutine == null && playerHealth != null)
        {
            healCoroutine = StartCoroutine(HealOverTime());
        }
    }

    private void StopHealing()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    private IEnumerator HealOverTime()
    {
        // Heal once per second until the player steps away or candle is blown out:
        while (isLit && playerInRange && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            playerHealth.Heal(healPerSecond);
            playerHealth.DecreaseFear(healPerSecond); // Optional: decrease fear slightly while healing
            yield return new WaitForSeconds(1f);
        }
        healCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        // If the candle is already lit when the player enters, start healing immediately:
        if (isLit)
        {
            StartHealing();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        // Player walked away; stop healing
        StopHealing();
    }
}
