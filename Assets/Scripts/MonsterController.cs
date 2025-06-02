using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using StarterAssets; // Assuming you have a FirstPersonController script for player reference

public class MonsterController : MonoBehaviour
{
    enum State { Crawl, Listen, Chase, Attack, Vanish }
    private State currentState = State.Crawl;

    [Header("Speeds & Ranges")]
    public float crawlSpeed = 1.5f;
    public float chaseSpeed = 4f;
    public float hearingRadius = 5f;        // should match HearingSphere radius
    public float attackDistance = 2f;

    [Header("Audio")]
public AudioClip growlClip;        // drag your growl clip here
public float growlRadius = 8f;     // how close the player must be to hear the growl
public float growlCooldown = 5f;   // seconds between growls

private AudioSource _audioSource;  // cache reference to our AudioSource
private float _lastGrowlTime = -999f;

    [Header("References")]
    public Animator animator;               // assign your Animator
    public NavMeshAgent navAgent;           // assign your NavMeshAgent component
    public Transform player;                // assign the Player transform in Inspector
    public ParticleSystem vanishEffect;     // optional: particle when vanishing

    private void Start()
    {
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
            // Cache the AudioSource (assumes one is attached to the same GameObject)
    _audioSource = GetComponent<AudioSource>();
        currentState = State.Crawl;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Crawl:
                CrawlBehavior();
                break;
            case State.Listen:
                // Listening coroutine is already running
                break;
            case State.Chase:
                ChaseBehavior();
                break;
            case State.Attack:
                // Attack coroutine is running
                break;
            case State.Vanish:
                // Vanish coroutine is running
                break;
        }
    }

    // ─── 1) Crawl Behavior ────────────────────────────────────────────────────────────
    void CrawlBehavior()
    {
        animator.SetBool("isCrawling", true);
        animator.SetBool("isChasing", false);

        // Manually move the agent forward each frame, instead of SetDestination
        navAgent.isStopped = false;
        navAgent.speed = crawlSpeed;

        // Move forward in the agent's local forward direction
        Vector3 forwardMovement = transform.forward * crawlSpeed * Time.deltaTime;
        navAgent.Move(forwardMovement);

        // Random chance to enter Listen state
        if (Random.value < Time.deltaTime * 0.05f)
        {
            StartCoroutine(StopAndListen());
        }

        // ─── 1.1) Hearing check ─────────────────────────────────────────────────────────
        // Ensure 'player' reference is assigned before using it
        if (player != null && FirstPersonController.I != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= hearingRadius && FirstPersonController.I.IsMakingNoise)
            {
                currentState = State.Chase;
            }
        }
        TryGrowl();
    }

    private void TryGrowl()
{
    if (player == null || _audioSource == null || growlClip == null)
        return;

    float distToPlayer = Vector3.Distance(transform.position, player.position);

    // Only growl if closer than growlRadius AND cooldown has elapsed
    if (distToPlayer <= growlRadius && Time.time - _lastGrowlTime >= growlCooldown)
    {
        _audioSource.PlayOneShot(growlClip);
        _lastGrowlTime = Time.time;
    }
}


    // ─── 2) Listen Behavior ───────────────────────────────────────────────────────────
    IEnumerator StopAndListen()
    {
        currentState = State.Listen;
        animator.SetBool("isCrawling", false);
        navAgent.isStopped = true;

        // Listen for 2–4 seconds
        yield return new WaitForSeconds(Random.Range(2f, 4f));

        currentState = State.Crawl;
    }

    // ─── 3) Chase Behavior ────────────────────────────────────────────────────────────
    void ChaseBehavior()
    {
        animator.SetBool("isCrawling", false);
        animator.SetBool("isChasing", true);
        navAgent.isStopped = false;
        navAgent.speed = chaseSpeed;

        // Move the agent toward the player's position
        if (player != null)
        {
            navAgent.SetDestination(player.position);
        }

        // If close enough, attack
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            StartCoroutine(DoAttack());
        }
        TryGrowl();
    }

    // ─── 4) Attack Behavior ──────────────────────────────────────────────────────────
    IEnumerator DoAttack()
    {
        currentState = State.Attack;
        navAgent.isStopped = true;
        animator.SetTrigger("Attack");

        // Wait for the attack animation to land (e.g. 1 second)
        yield return new WaitForSeconds(1.0f);

        // Trigger camera fall
        if (Camera.main != null)
            Camera.main.GetComponent<CameraFall>()?.FallDown();

        // Lose a life
        FearManager.I.LoseLife();

        // Give the player a moment
        yield return new WaitForSeconds(1.0f);

        currentState = State.Vanish;
        StartCoroutine(Vanish());
    }

    // ─── 5) Vanish Behavior ──────────────────────────────────────────────────────────
    IEnumerator Vanish()
    {
        animator.SetTrigger("Vanish");
        if (vanishEffect != null)
            Instantiate(vanishEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    // ─── 6) Called by CandleGhost to immediately force chase ─────────────────────────
    public void ForceChase()
    {
        if (currentState != State.Vanish && currentState != State.Attack)
        {
            currentState = State.Chase;
        }
    }
}
