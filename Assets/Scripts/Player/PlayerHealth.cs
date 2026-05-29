
// Tracks player HP. When hit, triggers UIManager's damage flash.
// Includes invincibility frames so one attack doesn't chain into instant death.

using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibilityTime = 1f;

    private int _hp;
    private bool _invincible;

    private void Start()
    {
        _hp = maxHealth;
        UIManager.Instance?.UpdateHealthBar(_hp, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (_invincible) return;

        _hp = Mathf.Max(0, _hp - amount);
        UIManager.Instance?.UpdateHealthBar(_hp, maxHealth);
        UIManager.Instance?.TriggerDamageFlash();
        StartCoroutine(IFrames());

        if (_hp <= 0) GameManager.Instance?.OnPlayerDied();
    }

    public void Heal(int amount)
    {
        _hp = Mathf.Min(maxHealth, _hp + amount);
        UIManager.Instance?.UpdateHealthBar(_hp, maxHealth);
    }

    private IEnumerator IFrames()
    {
        _invincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        _invincible = false;
    }
}