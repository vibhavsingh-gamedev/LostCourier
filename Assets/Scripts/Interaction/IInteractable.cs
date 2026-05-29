
// This interface makes the interaction system modular.
// Any object that can be interacted with implements this.
// That way PlayerInteraction doesn't need to know WHAT it's interacting with.

public interface IInteractable
{
    string InteractionPrompt { get; }   // Text shown on screen ("Pick up Artifact [E]")
    void Interact(PlayerInteraction player);
}