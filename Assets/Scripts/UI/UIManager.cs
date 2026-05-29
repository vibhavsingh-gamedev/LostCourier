
// Singleton. Manages all on-screen UI elements.
// Attach to a Canvas GameObject or a persistent "Managers" empty GameObject.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private TextMeshProUGUI floatingMessageText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image damageFlashImage;   // Full-screen red panel, alpha 0 normally

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;

    private bool _paused;
    private Coroutine _msgCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        // Pause toggle on Escape
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance?.IsGameActive == true)
            TogglePause();
    }

    // Objective 
    public void SetObjective(string text)
    {
        if (objectiveText) objectiveText.text = $"Objective: {text}";
    }

    // Interaction Prompt
    public void ShowInteractionPrompt(string text)
    {
        if (!interactionPromptText) return;
        interactionPromptText.gameObject.SetActive(true);
        interactionPromptText.text = text;
    }

    public void HideInteractionPrompt()
    {
        interactionPromptText?.gameObject.SetActive(false);
    }

    // Floating Messages 
    public void ShowMessage(string text, float duration = 3f)
    {
        if (_msgCoroutine != null) StopCoroutine(_msgCoroutine);
        _msgCoroutine = StartCoroutine(MessageRoutine(text, duration));
    }

    private IEnumerator MessageRoutine(string text, float duration)
    {
        if (!floatingMessageText) yield break;
        floatingMessageText.gameObject.SetActive(true);
        floatingMessageText.text = text;
        yield return new WaitForSeconds(duration);
        floatingMessageText.gameObject.SetActive(false);
    }

    // Health 
    public void UpdateHealthBar(int current, int max)
    {
        if (!healthBar) return;
        healthBar.maxValue = max;
        healthBar.value = current;
    }

    // Damage Flash 
    public void TriggerDamageFlash()
    {
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (!damageFlashImage) yield break;
        Color startColor = new Color(1f, 0f, 0f, 0.5f);
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            damageFlashImage.color = Color.Lerp(startColor, Color.clear, t / 0.4f);
            yield return null;
        }
        damageFlashImage.color = Color.clear;
    }

    // Pause 
    public void TogglePause()
    {
        _paused = !_paused;
        Time.timeScale = _paused ? 0f : 1f;
        pausePanel?.SetActive(_paused);
        Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _paused;
    }

    // Game End Screens
    public void ShowWinScreen()
    {
        winPanel?.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowFailScreen()
    {
        failPanel?.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Resume Button (wire to Pause Panel Resume button) 
    public void OnResumePressed()
    {
        if (_paused) TogglePause();
    }
}