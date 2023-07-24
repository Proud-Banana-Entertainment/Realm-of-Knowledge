using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject backToMenuCanvas;

    [Header("Button")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private AudioSource buttonClickedSFX;

    [Header("Player")]
    [SerializeField] private GameObject playerCursor;
    [SerializeField] private FirstPersonController player;
    [SerializeField] private GameObject footstepsSFX;
    [SerializeField] private GameObject jumpSfx;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;

    private bool hasTriggered;

    private IEnumerator LoadingScreentoMenu()
    {
        loadingScreen.SetActive(true);

       AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }

    void PlayerInteraction (bool enabled)
    {
        playerCursor.SetActive (enabled);
        player.cameraCanMove = enabled;
        footstepsSFX.SetActive (enabled);
        jumpSfx.SetActive (enabled);
    }
    void Start()
    {
        hasTriggered = false;
        SetCursorState(false);
        backToMenuCanvas.SetActive(false);

        mainMenuButton.onClick.AddListener(BackToMainMenu);
        resumeButton.onClick.AddListener(Resume);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
        }
    }

    void BackToMainMenu()
    {
        buttonClickedSFX.Play();
        Time.timeScale = 1f;
        StartCoroutine(LoadingScreentoMenu());
    }

    void Resume()
    {
        buttonClickedSFX.Play();
        Time.timeScale = 1f;
        PlayerInteraction(true);
        SetCursorState(false);
        backToMenuCanvas.SetActive(false);
    }

    private void SetCursorState(bool enabled)
    {
        Cursor.visible = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && hasTriggered)
        {
            Time.timeScale = 0f;
            SetCursorState(true);
            backToMenuCanvas.SetActive(true);
            PlayerInteraction(false);
        }
    }
}
