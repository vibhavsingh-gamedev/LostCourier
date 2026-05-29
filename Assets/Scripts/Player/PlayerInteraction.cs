using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private Transform holdPoint;

    private IInteractable _currentTarget;
    private PickupItem _heldItem;
    private PlayerController _playerController;
    private Camera _cam;

    public Transform HoldPoint => holdPoint;
    public PickupItem HeldItem => _heldItem;
    public bool IsCarrying => _heldItem != null;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _cam = Camera.main;
    }

    private void Update()
    {
        FindClosestInteractable();
        HandleInput();
        if (_playerController != null)
            _playerController.IsCarrying = IsCarrying;
    }

    private void FindClosestInteractable()
    {
        // OverlapSphere — player ke around sphere mein sab dhundo
        Collider[] colliders = Physics.OverlapSphere(
            transform.position, interactionRange
        );

        IInteractable closest = null;
        float closestAngle = 60f; // sirf 60 degree cone ke andar wala detect hoga

        foreach (Collider col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            // Check karo — player ka camera us object ki taraf dekh raha hai?
            Vector3 dirToObject = (col.transform.position - transform.position).normalized;
            Vector3 camForward = _cam.transform.forward;
            float angle = Vector3.Angle(camForward, dirToObject);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                closest = interactable;
            }
        }

        if (closest != null)
        {
            _currentTarget = closest;
            UIManager.Instance?.ShowInteractionPrompt(closest.InteractionPrompt);
        }
        else
        {
            _currentTarget = null;
            UIManager.Instance?.HideInteractionPrompt();
        }
    }

    private void HandleInput()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (_currentTarget == null) return;

        _currentTarget.Interact(this);

        if (_currentTarget is PickupItem pickup)
            _heldItem = pickup.IsHeld ? pickup : null;
    }

    // Scene view mein detection range dikhega
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}