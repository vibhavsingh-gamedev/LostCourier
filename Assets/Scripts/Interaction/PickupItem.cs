
// Attach to the artifact GameObject.
// Implements IInteractable so it works with our modular interaction system.
// The item parents itself to the player's hold point when picked up.

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "Ancient Artifact";
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, -0.2f, 0.6f);
    [SerializeField] private Vector3 holdRotation = new Vector3(0f, 0f, 0f);

    private Rigidbody _rb;
    private bool _isHeld = false;

    // IInteractable — prompt changes based on current state
    public string InteractionPrompt => _isHeld ? $"Drop {itemName} [E]" : $"Pick up {itemName} [E]";
    public bool IsHeld => _isHeld;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    // Called by PlayerInteraction when player presses E
    public void Interact(PlayerInteraction player)
    {
        if (_isHeld) Drop();
        else PickUp(player.HoldPoint);
    }

    public void PickUp(Transform holdPoint)
    {
        _isHeld = true;
        _rb.isKinematic = true;     // Disable physics while held
        _rb.useGravity = false;
        transform.SetParent(holdPoint);
        transform.localPosition = holdOffset;
        transform.localRotation = Quaternion.Euler(holdRotation);

        GameManager.Instance?.OnItemPickedUp(this);
    }

    public void Drop()
    {
        _isHeld = false;
        transform.SetParent(null);
        _rb.isKinematic = false;    // Re-enable physics on drop
        _rb.useGravity = true;

        GameManager.Instance?.OnItemDropped(this);
    }
}