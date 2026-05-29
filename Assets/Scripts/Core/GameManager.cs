
// Singleton. Tracks game state — active, won, lost.
// Other scripts call Instance.OnDeliveryComplete() etc. to trigger state changes.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameActive { get; private set; } = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        IsGameActive = true;
        UIManager.Instance?.SetObjective("Find the ancient artifact.");
    }

    // Called by PickupItem 

    public void OnItemPickedUp(PickupItem item)
    {
        UIManager.Instance?.SetObjective("Carry the artifact to the glowing altar.");
    }

    public void OnItemDropped(PickupItem item)
    {
        UIManager.Instance?.SetObjective("Pick up the ancient artifact again.");
    }

    // Called by DeliveryZone 

    public void OnDeliveryComplete()
    {
        IsGameActive = false;
        UIManager.Instance?.ShowWinScreen();
    }

    // Called by PlayerHealth

    public void OnPlayerDied()
    {
        IsGameActive = false;
        UIManager.Instance?.ShowFailScreen();
    }

    // UI Buttons call these 

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}