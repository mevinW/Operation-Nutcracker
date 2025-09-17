using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SceneChanger : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("The SpriteMask transform whose scale we will shrink.")]
    [SerializeField] private Transform maskTransform;

    [Tooltip("How long the circle shrink takes (seconds).")]
    [SerializeField] private float transitionDuration = 0.8f;

    // cache the starting scale of your mask
    private Vector3 maskStartScale;

    // the scene we're about to load
    private string sceneName;

    [Header("Acorn Collectors")]
    public AcornCollector acornCollectorOne, acornCollectorTwo;

    private void Awake()
    {
        if (maskTransform == null)
            Debug.LogError("SceneChanger: please assign Mask Transform!", this);

        // remember how big the circle was to start
        maskStartScale = maskTransform.localScale;
    }

    /// <summary>
    /// Replaces direct LoadScene with a shrinking‐circle transition.
    /// </summary>
    private void LoadWithTransition(string name)
    {
        sceneName = name;
        StartCoroutine(DoTransitionAndLoad());
    }

    private IEnumerator DoTransitionAndLoad()
    {
        // 0) Disable all UI canvases so they won't draw on top
        Canvas[] uiCanvases = FindObjectsOfType<Canvas>();
        foreach (var c in uiCanvases)
        {
            // if you have a Canvas dedicated to the transition itself, you can skip it here
            c.enabled = false;
        }

        // 1) Shrink from full size → zero
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            maskTransform.localScale = Vector3.Lerp(maskStartScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        maskTransform.localScale = Vector3.zero;

        // 2) Now everything is black (and UI is already off), load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    // ---------------------------------------------------------------------
    // Public methods you already had – just swap loadScene() for our new
    // LoadWithTransition(...) call
    // ---------------------------------------------------------------------

    private void loadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneChanger: sceneName is empty!");
            return;
        }
        LoadWithTransition(sceneName);
    }

    public void loadStart()
    {
        sceneName = "Start";
        loadScene();
    }

    public void loadBackstory()
    {
        sceneName = "Backstory";
        loadScene();
        AudioManager.Instance.PlayMusic(0);
    }

    public void loadTutorial()
    {
        sceneName = "Tutorial";
        loadScene();
        AudioManager.Instance.PlayMusic(1);
    }

    public void loadShop()
    {
        AudioManager.Instance.PlayMusic(7);
        GameManager.Instance.currentShop += 1;
        sceneName = "Shop";
        loadScene();

        if (GameManager.Instance.currentShop != 1)
        {
            GameManager.Instance.acornsCollectedOne += acornCollectorOne.getAcornCount();
            GameManager.Instance.acornsCollectedTwo += acornCollectorTwo.getAcornCount();
        }
        Debug.Log(GameManager.Instance.currentShop);
    }

    public void loadForest()
    {
        AudioManager.Instance.PlayMusic(2);
        sceneName = "Forest";
        loadScene();
    }

    public void loadPavilion()
    {
        AudioManager.Instance.PlayMusic(3);
        sceneName = "Pavilion";
        loadScene();
    }

    public void loadFactory()
    {
        AudioManager.Instance.PlayMusic(4);
        sceneName = "Factory";
        loadScene();
    }

    public void loadOffice()
    {
        AudioManager.Instance.PlayMusic(5);
        sceneName = "Office";
        loadScene();
    }

    public void loadEnd()
    {
        AudioManager.Instance.PlayMusic(6);
        sceneName = "End";
        loadScene();
    }

    public void loadMapAfterShop()
    {
        Debug.Log(GameManager.Instance.currentShop);
        if (GameManager.Instance.currentShop == 0)
        {
            loadTutorial();
        }
        else if (GameManager.Instance.currentShop == 1)
        {
            loadForest();
        }
        else if (GameManager.Instance.currentShop == 2)
        {
            loadPavilion();
        }
        else if (GameManager.Instance.currentShop == 3)
        {
            loadFactory();
        }
        else
        {
            loadOffice();
        }
    }

    public void restart()
    {
        loadStart();
        GameManager.Instance.ResetGameState();
    }
}
