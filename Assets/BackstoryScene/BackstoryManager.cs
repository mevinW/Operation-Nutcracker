using UnityEngine;
using UnityEngine.SceneManagement;

public class BackstoryManager : MonoBehaviour
{
    [Header("Slides (GameObjects)")]
    [Tooltip("Drag each Slide GameObject here in order")]
    public GameObject[] slides;

    [Header("Settings")]
    public KeyCode advanceKey = KeyCode.Space;

    private int currentIndex = 0;
    [SerializeField] private SceneChanger sceneChanger;

    void Start()
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogError("No slides assigned!");
            return;
        }
        // Ensure only the first slide is active
        for (int i = 0; i < slides.Length; i++)
            slides[i].SetActive(i == 0);

        currentIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(advanceKey))
            AdvanceSlide();
    }

    private void AdvanceSlide()
    {
        // Turn off current
        slides[currentIndex].SetActive(false);

        // Next index?
        currentIndex++;

        if (currentIndex < slides.Length)
        {
            // Turn on next slide
            slides[currentIndex].SetActive(true);
        }
        else
        {
            // All slides shown → load next scene
            sceneChanger.loadTutorial();
        }
    }
}
