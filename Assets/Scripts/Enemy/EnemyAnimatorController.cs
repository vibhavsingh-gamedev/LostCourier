using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private float _smoothSpeed;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _enemyAI = GetComponent<EnemyAI>();

        if (_animator == null)
            Debug.LogError("Enemy Animator nahi mila!");
        if (_agent == null)
            Debug.LogError("NavMeshAgent nahi mila!");
    }

    private void Update()
    {
        if (_animator == null || _agent == null) return;

        // NavMesh desired velocity use karo — actual velocity se zyada reliable
        float speed = _agent.desiredVelocity.magnitude;

        // Smoothly blend
        _smoothSpeed = Mathf.Lerp(_smoothSpeed, speed, Time.deltaTime * 20f);

        // Max speed se normalize — patrol=2.5, chase=5.5
        float normalized = Mathf.Clamp01(_smoothSpeed / 5.5f);
        _animator.SetFloat("Speed", normalized);
    }

    // EnemyAI.cs se call hoga
    public void TriggerAttack()
    {
        if (_animator != null)
            _animator.SetTrigger("Attack");  // Bool nahi — Trigger!
    }
}