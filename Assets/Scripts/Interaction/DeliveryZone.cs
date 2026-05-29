
// Attach to the delivery zone trigger area.
// Has two modes: manual (player presses E) and auto (walk into zone while holding artifact).
// Implements IInteractable for manual delivery.

using UnityEngine;

public class DeliveryZone : MonoBehaviour, IInteractable
{
    [Header("Visual")]
    [SerializeField] private GameObject glowIndicator;      // Glowing ring / particle under zone
    [SerializeField] private ParticleSystem successEffect;

    private bool _completed = false;

    public string InteractionPrompt => "Deliver the Artifact [E]";

    private void Start()
    {
        glowIndicator?.SetActive(true);
    }

    // Manual delivery: player presses E while looking at zone
    public void Interact(PlayerInteraction player)
    {
        if (_completed) return;

        if (player.HeldItem != null)
            Complete(player);
        else
            UIManager.Instance?.ShowMessage("You need to be carrying the artifact!", 2f);
    }

    // Auto delivery: walk into zone while holding artifact
    private void OnTriggerEnter(Collider other)
    {
        if (_completed) return;

        PlayerInteraction player = other.GetComponent<PlayerInteraction>();
        if (player != null && player.HeldItem != null)
            Complete(player);
    }

    private void Complete(PlayerInteraction player)
    {
        _completed = true;

        // Drop item and place it on the altar
        PickupItem artifact = player.HeldItem;
        artifact.Drop();
        artifact.transform.position = transform.position + Vector3.up * 0.5f;
        artifact.GetComponent<Rigidbody>().isKinematic = true;  // Lock it in place

        // Visual feedback
        successEffect?.Play();
        glowIndicator?.SetActive(false);

        GameManager.Instance?.OnDeliveryComplete();
    }
}