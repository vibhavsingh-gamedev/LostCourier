using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator _animator;
    private float _smoothSpeed;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null)
            Debug.LogError("Animator nahi mila!");
    }

    private void Update()
    {
        if (_animator == null) return;

        // Direct Input se speed lo — CC velocity reliable nahi hai
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float inputMagnitude = new Vector2(h, v).magnitude;

        // Sprint check
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = sprinting ? 1f : (inputMagnitude > 0.1f ? 0.5f : 0f);

        // Smoothly blend
        _smoothSpeed = Mathf.Lerp(_smoothSpeed, targetSpeed, Time.deltaTime * 8f);
        _animator.SetFloat("Speed", _smoothSpeed);

        // Jump
        bool jumping = Input.GetButton("Jump");
        _animator.SetBool("IsJumping", jumping);

        Debug.Log("Input: " + inputMagnitude.ToString("F2") +
                  " | Speed set: " + _smoothSpeed.ToString("F2"));
    }

    public void TriggerHit()
    {
        if (_animator != null)
            _animator.SetTrigger("IsHit");
    }
}