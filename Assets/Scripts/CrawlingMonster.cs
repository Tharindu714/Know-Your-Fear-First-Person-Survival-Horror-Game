using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CrawlingMonster : MonoBehaviour
{
    [Header("References")]
    public Transform playerCameraTransform;   // → drag Main Camera.transform here
    public Animator monsterAnimator;          // → drag the Animator on this prefab here
    public Transform[] hidePositions;         // → drag an array of empty Transforms around the scene

    [Header("Settings")]
    public float appearDistance = 5f;       // how close the player must be before it can spawn behind them
    public float behindDistance = 1.5f;     // how far behind the player’s camera to teleport
    public float timeBeforeDisperse = 0.5f; // seconds after jumpscare before vanishing

    private bool isActive = false;
    private bool hasJumpscared = false;

    private void Start()
    {
        // Start disabled; it will enable itself when spawn logic runs
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActive)
        {
            // Monster not yet spawned. Check if we can appear behind player.
            if (PlayerNotLooking() && PlayerWithinAppearDistance())
            {
                AppearBehindPlayer();
            }
        }
        else if (isActive && !hasJumpscared)
        {
            // Waiting for the player to look at it
            if (PlayerLookingAtMonster())
            {
                StartCoroutine(DoJumpscareThenVanish());
            }
        }
    }

    private bool PlayerNotLooking()
    {
        Vector3 toMonster = transform.position - playerCameraTransform.position;
        float angle = Vector3.Angle(playerCameraTransform.forward, toMonster);
        return angle > 90f; // if angle > 90°, player’s back is to the spawn spot
    }

    private bool PlayerWithinAppearDistance()
    {
        float dist = Vector3.Distance(playerCameraTransform.position, transform.position);
        return dist <= appearDistance;
    }

    private void AppearBehindPlayer()
    {
        Vector3 behindPos = playerCameraTransform.position - playerCameraTransform.forward * behindDistance;
        behindPos.y = playerCameraTransform.position.y - 0.5f; // slightly lower so it’s on the floor
        transform.position = behindPos;

        // Face the player’s head
        Vector3 lookPoint = new Vector3(playerCameraTransform.position.x, transform.position.y, playerCameraTransform.position.z);
        transform.LookAt(lookPoint);

        gameObject.SetActive(true);
        isActive = true;
    }

    private bool PlayerLookingAtMonster()
    {
        Vector3 toMonster = transform.position - playerCameraTransform.position;
        float angle = Vector3.Angle(playerCameraTransform.forward, toMonster);
        float dist = Vector3.Distance(playerCameraTransform.position, transform.position);
        return (angle < 15f && dist <= appearDistance + 2f);
    }

    private IEnumerator DoJumpscareThenVanish()
    {
        hasJumpscared = true;

        // Trigger the jumpscare animation (make sure your Animator has a "Jumpscare" trigger)
        if (monsterAnimator != null)
            monsterAnimator.SetTrigger("Jumpscare");

        // Register with JumpScareManager
        JumpScareManager.Instance?.RegisterJumpScare();

        // Wait a short moment, then vanish
        yield return new WaitForSeconds(timeBeforeDisperse);

        // Teleport to a random hide spot (if any exist)
        if (hidePositions != null && hidePositions.Length > 0)
        {
            int idx = Random.Range(0, hidePositions.Length);
            Transform chosen = hidePositions[idx];
            transform.position = chosen.position;
            transform.rotation = chosen.rotation;
        }

        // Reset so it can spawn again if you want:
        isActive = false;
        hasJumpscared = false;
        gameObject.SetActive(false);
    }
}
