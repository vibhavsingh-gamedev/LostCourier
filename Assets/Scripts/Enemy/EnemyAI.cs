
// A simple 3-state enemy: Patrol > Chase > Attack > back to Patrol
// Uses Unity NavMesh for pathfinding.
// IMPORTANT: Bake NavMesh before testing (Window > AI > Navigation > Bake)

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitAtPoint = 1.5f;
    [SerializeField] private float patrolSpeed = 2.5f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float fieldOfView = 100f;   // Degrees
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Chase & Attack")]
    [SerializeField] private float chaseSpeed = 5.5f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float losePlayerDistance = 13f;  // Give up chasing after this far

    private NavMeshAgent _agent;
    private Transform _player;
    private State _state = State.Patrol;
    private int _patrolIndex = 0;
    private float _waitTimer, _attackTimer;
    private bool _waiting;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        if (patrolPoints.Length > 0)
            _agent.SetDestination(patrolPoints[0].position);

        _agent.speed = patrolSpeed;
    }

    private void Update()
    {
        if (_player == null) return;
        if (GameManager.Instance != null && !GameManager.Instance.IsGameActive) return;

        switch (_state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase: UpdateChase(); break;
            case State.Attack: UpdateAttack(); break;
        }
    }

    // PATROL
    private void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;

        if (_waiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _waiting = false;
                _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
                _agent.SetDestination(patrolPoints[_patrolIndex].position);
            }
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
        {
            _waiting = true;
            _waitTimer = waitAtPoint;
        }

        // Check if player is visible
        if (CanSeePlayer()) EnterChase();
    }

    private bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > detectionRadius) return false;

        // Field of view check
        Vector3 dir = (_player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) > fieldOfView * 0.5f) return false;

        // Line of sight check
        bool blocked = Physics.Raycast(
            transform.position + Vector3.up,
            dir,
            dist,
            obstacleLayer
        );
        return !blocked;
    }

    // CHASE 
    private void UpdateChase()
    {
        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist <= attackRange)
        {
            _state = State.Attack;
            _agent.ResetPath();
            return;
        }

        if (dist > losePlayerDistance)
        {
            // Lost the player — go back to patrol
            _state = State.Patrol;
            _agent.speed = patrolSpeed;
            if (patrolPoints.Length > 0)
                _agent.SetDestination(patrolPoints[_patrolIndex].position);
            UIManager.Instance?.ShowMessage("Enemy lost sight of you.", 2f);
            return;
        }

        _agent.SetDestination(_player.position);
    }

    private void EnterChase()
    {
        _state = State.Chase;
        _agent.speed = chaseSpeed;
        UIManager.Instance?.ShowMessage("Enemy spotted you!", 2f);
    }

    // ATTACK 
    private void UpdateAttack()
    {
        float dist = Vector3.Distance(transform.position, _player.position);

        // Player moved away — resume chasing
        if (dist > attackRange + 0.5f)
        {
            _state = State.Chase;
            _agent.speed = chaseSpeed;
            return;
        }

        // Face the player
        Vector3 look = new Vector3(_player.position.x - transform.position.x, 0, _player.position.z - transform.position.z);
        if (look != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), 10f * Time.deltaTime);

        // Attack on cooldown
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            _player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);

            // Animation trigger karo 
            GetComponent<EnemyAnimatorController>()?.TriggerAttack();

            _attackTimer = attackCooldown;
        }
    }

    // GIZMOS 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}