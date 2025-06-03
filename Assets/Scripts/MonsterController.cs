using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using StarterAssets; // Ensure you have the StarterAssets package for FirstPersonController

public class MonsterController : MonoBehaviour
{
    enum State { Crawl, Chase, Attack, React, Vanish }
    private State currentState = State.Crawl;

    [Header("Speeds & Ranges")]
    public float crawlSpeed = 1.5f;
    public float chaseSpeed = 4f;
    public float hearingRadius = 5f;        // radius for noise detection
    public float attackDistance = 2f;       // distance to trigger attack

    [Header("React Settings")]
    public float reactAngle = 60f;          // degrees within which player “seeing” triggers reaction
    public float reactDuration = 1.0f;      // how long the react movement lasts (in seconds)

    [Header("References")]
    public Animator animator;               // assign your Animator
    public NavMeshAgent navAgent;           // assign your NavMeshAgent component
    public ParticleSystem vanishEffect;     // optional: particle when vanishing

    [Header("Audio")]
    public AudioClip clickerClip;           // TLOU clicker SFX
    public AudioClip growlClip;             // optional growl loop
    public float growlCooldown = 5f;        // seconds between growl loops

    private Transform _player;              // set via InitializeForSpawn()
    private AudioSource _aSource;           // cached AudioSource
    private AudioClip _clickerSFX;          // clicker assigned at spawn
    private float _lastGrowlTime = -999f;   // timestamp of last growl
    private bool _hasReacted = false;       // ensures react happens only once per spawn

    void Start()
    {
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // Ensure an AudioSource is present
        _aSource = GetComponent<AudioSource>();
        if (_aSource == null)
        {
            _aSource = gameObject.AddComponent<AudioSource>();
            _aSource.spatialBlend = 1f; // 3D sound
            _aSource.playOnAwake = false;
        }

        currentState = State.Crawl;
    }

    /// <summary>
    /// Must be called immediately after instantiation to set the player target and clicker SFX.
    /// </summary>
    public void InitializeForSpawn(Transform playerTransform, AudioClip clickSFX)
    {
        _player = playerTransform;
        _clickerSFX = clickSFX;
        currentState = State.Crawl;

        // Play a single click so the player knows it's right behind them
        if (_clickerSFX != null && _aSource != null)
        {
            _aSource.PlayOneShot(_clickerSFX);
        }

        // Optional: start a looping growl if desired
        if (growlClip != null)
        {
            _aSource.clip = growlClip;
            _aSource.loop = true;
            _aSource.Play();
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Crawl:
                CrawlBehavior();
                break;
            case State.Chase:
                ChaseBehavior();
                break;
            case State.Attack:
                // handled by DoAttack coroutine
                break;
            case State.React:
                // handled by ReactAndVanish coroutine
                break;
            case State.Vanish:
                // handled by Vanish coroutine
                break;
        }
    }

    // ─── 1) Crawl Behavior ────────────────────────────────────────────────────────────
    void CrawlBehavior()
    {
        animator.SetBool("isCrawling", true);
        animator.SetBool("isChasing", false);

        navAgent.isStopped = false;
        navAgent.speed = crawlSpeed;

        // Move forward manually so we stay on the NavMesh
        Vector3 forwardMove = transform.forward * crawlSpeed * Time.deltaTime;
        navAgent.Move(forwardMove);


        // 1.1) Hearing check: if player is noisy and within hearingRadius, switch to Chase
        if (_player != null && FirstPersonController.I != null)
        {
            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist <= hearingRadius && FirstPersonController.I.IsMakingNoise)
            {
                currentState = State.Chase;
                return;
            }
        }

        // 1.2) React if the player “looks” at us (angle within reactAngle)
        if (!_hasReacted && _player != null)
        {
            Vector3 toPlayer = (_player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, toPlayer);
            if (angleToPlayer <= reactAngle)
            {
                _hasReacted = true;
                StartCoroutine(ReactAndVanish());
                return;
            }
        }

        // 1.3) Growl loop check
        TryGrowl();
    }


    // ─── 3) Chase Behavior ─────────────────────────────────────────────────────────────
    void ChaseBehavior()
    {
        animator.SetBool("isCrawling", false);
        animator.SetBool("isChasing", true);
        navAgent.isStopped = false;
        navAgent.speed = chaseSpeed;

        if (_player != null)
        {
            navAgent.SetDestination(_player.position);
        }

        // If close enough, attack
        if (_player != null && Vector3.Distance(transform.position, _player.position) <= attackDistance)
        {
            StartCoroutine(DoAttack());
        }

        // Continue growl while chasing
        TryGrowl();
    }

    // ─── 4) Attack Behavior ─────────────────────────────────────────────────────────────
    IEnumerator DoAttack()
    {
        currentState = State.Attack;
        navAgent.isStopped = true;
        animator.SetTrigger("Attack");

        // Wait for animation to land (e.g. 1 second)
        yield return new WaitForSeconds(1.0f);

        // Trigger player fall
        if (Camera.main != null)
            Camera.main.GetComponent<CameraFall>()?.FallDown();

        // Deduct a life
        GameManager.I.LoseLife();

        // Give a moment before vanishing
        yield return new WaitForSeconds(1.0f);

        currentState = State.Vanish;
        StartCoroutine(Vanish());
    }

    // ─── 5) React + Vanish Behavior ────────────────────────────────────────────────────
    IEnumerator ReactAndVanish()
    {
        currentState = State.React;
        animator.SetBool("isCrawling", false);
        animator.SetTrigger("React");    // assumes you have a "React" trigger/animation

        // React movement: step sideways/backward for reactDuration
        navAgent.isStopped = false;
        navAgent.speed = crawlSpeed;

        // Pick a random angle ±90° from forward
        Vector3 randomDir = Quaternion.Euler(0f, Random.Range(-90f, 90f), 0f) * transform.forward;
        Vector3 reactTarget = transform.position + randomDir * 2f;

        // Project onto NavMesh
        NavMeshHit hm;
        if (NavMesh.SamplePosition(reactTarget, out hm, 2f, NavMesh.AllAreas))
        {
            reactTarget = hm.position;
        }

        float timer = 0f;
        while (timer < reactDuration)
        {
            timer += Time.deltaTime;
            navAgent.SetDestination(reactTarget);
            yield return null;
        }

        StartCoroutine(Vanish());
    }

    // ─── 6) Vanish Behavior ────────────────────────────────────────────────────────────
    IEnumerator Vanish()
    {
        animator.SetTrigger("Vanish");
        if (_aSource != null && _aSource.isPlaying)
            _aSource.Stop();

        if (vanishEffect != null)
            Instantiate(vanishEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    // ─── 7) Growl Logic ────────────────────────────────────────────────────────────────
    private void TryGrowl()
    {
        if (_player == null || _aSource == null || growlClip == null)
            return;

        float distToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distToPlayer <= hearingRadius && Time.time - _lastGrowlTime >= growlCooldown)
        {
            _aSource.PlayOneShot(growlClip);
            _lastGrowlTime = Time.time;
        }
    }

    // ─── 8) Called by MonsterSpawnManager Immediately After Instantiation ─────────────
    public void ForceChase()
    {
        if (currentState != State.Vanish && currentState != State.Attack)
        {
            currentState = State.Chase;
        }
    }
}
