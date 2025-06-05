using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using StarterAssets; // Ensure you have the StarterAssets package for FirstPersonController

public class MonsterController : MonoBehaviour
{
    enum State { Crawl, React, Dead }
    private State currentState = State.Crawl;

    [Header("Speeds & Ranges")]
    public float crawlSpeed = 1.5f;
    public float hearingRadius = 5f;        // unused now (no chase)
    public float reactAngle = 60f;          // degrees within which the player “sees” the monster
    public float reactDuration = 1.0f;      // how many seconds monster sidesteps before disappearing

    [Header("References")]
    public Animator animator;               // drag the Animator in prefab
    public NavMeshAgent navAgent;           // drag the NavMeshAgent in prefab

    [Header("Audio")]
    public AudioClip _clickerSFX;           // one‐time click at spawn

    // runtime‐only
    private Transform _player;
    private AudioSource _aSource;
    private bool _hasReacted = false;
    private float _spawnTime;

    /// <summary>
    /// Must be called immediately after instantiation to set the player target and clicker SFX.
    /// This version ensures _aSource is assigned before use.
    /// </summary>
    public void InitializeForSpawn(Transform playerTransform, AudioClip clickSFX)
    {
        _player = playerTransform;
        _clickerSFX = clickSFX;
        currentState = State.Crawl;

        // Ensure we have a valid AudioSource reference, in case Start() didn't run yet:
        if (_aSource == null)
        {
            _aSource = GetComponent<AudioSource>();
            if (_aSource == null)
            {
                // Add one if it truly didn't exist
                _aSource = gameObject.AddComponent<AudioSource>();
                _aSource.spatialBlend = 1f;
                _aSource.playOnAwake = false;
            }
        }

        // Play a single click so the player knows it's right behind them
        if (_clickerSFX != null && _aSource != null)
        {
            _aSource.PlayOneShot(_clickerSFX);
        }
    }


    void Start()
    {
        // Cache components
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        _aSource = GetComponent<AudioSource>();
        if (_aSource == null)
        {
            _aSource = gameObject.AddComponent<AudioSource>();
            _aSource.spatialBlend = 1f; // 3D
            _aSource.playOnAwake = false;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Crawl:
                CrawlBehavior();
                break;
            case State.React:
                // React behavior is driven by the coroutine
                break;
            case State.Dead:
                // Nothing
                break;
        }
    }

    void CrawlBehavior()
    {
        animator.SetBool("isCrawling", true);

        // Move forward
        navAgent.isStopped = false;
        navAgent.speed = crawlSpeed;
        Vector3 forwardMove = transform.forward * crawlSpeed * Time.deltaTime;
        navAgent.Move(forwardMove);

        // If player looks at us (angle within reactAngle), react
        if (!_hasReacted && _player != null)
        {
            Vector3 toPlayer = (_player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, toPlayer);
            if (angle <= reactAngle)
            {
                _hasReacted = true;
                StartCoroutine(ReactAndDisappear());
                return;
            }
        }
    }

    IEnumerator ReactAndDisappear()
    {
        currentState = State.React;
        animator.SetBool("isCrawling", false);
        animator.SetTrigger("React"); // assumes a “React” trigger & animation exist

        // Sideways step for reactDuration
        navAgent.isStopped = false;
        navAgent.speed = crawlSpeed;

        Vector3 randomDir = Quaternion.Euler(0f, Random.Range(-90f, 90f), 0f) * transform.forward;
        Vector3 target = transform.position + randomDir * 2f;

        // Project to NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
            target = hit.position;

        float timer = 0f;
        while (timer < reactDuration)
        {
            timer += Time.deltaTime;
            navAgent.SetDestination(target);
            yield return null;
        }

        // Destroy the monster
        currentState = State.Dead;
        Destroy(gameObject);
    }
}
