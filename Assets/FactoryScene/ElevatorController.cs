using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class ElevatorController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Button that appears when a player enters elevator trigger.")]
    [SerializeField] private Button callButton;
    [Tooltip("Text that shows the joke after teleport.")]
    [SerializeField] private Text trollText;

    [Header("Teleport Settings")]
    [Tooltip("World position to teleport the player to.")]
    [SerializeField] private Vector3 teleportDestination = new Vector3(0f, 5f, 0f);

    [Header("Player Tags")]
    [Tooltip("Tag of Player One.")]
    [SerializeField] private string playerOneTag = "Player";
    [Tooltip("Tag of Player Two.")]
    [SerializeField] private string playerTwoTag = "PlayerTwo";

    private GameObject currentRider;

    private void Awake()
    {
        if (callButton == null || trollText == null)
        {
            Debug.LogError("ElevatorController: UI references (callButton or trollText) are not assigned!");
            enabled = false;
            return;
        }

        // Hide UI on start
        callButton.gameObject.SetActive(false);
        trollText.gameObject.SetActive(false);

        // Hook up the click event
        //callButton.onClick.AddListener(OnCallButtonClicked);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerOneTag) || other.CompareTag(playerTwoTag))
        {
            currentRider = other.gameObject;
            trollText.gameObject.SetActive(false);      // hide any old joke
            callButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentRider)
        {
            callButton.gameObject.SetActive(false);
            trollText.gameObject.SetActive(false);
            currentRider = null;
        }
    }

    public void OnCallButtonClicked()
    {
        if (currentRider == null) return;

        // Teleport the rider
        currentRider.transform.position = teleportDestination;

        // Hide the button until someone enters again
        callButton.gameObject.SetActive(false);

        currentRider = null;
    }

    public void OnCallButtonClickedTroll()
    {
        if (currentRider == null) return;

        // Teleport the rider
        currentRider.transform.position = teleportDestination;

        // Hide the button until someone enters again
        callButton.gameObject.SetActive(false);

        // Show your little joke
        trollText.text = "Lol, you thought it was gonna work?";
        trollText.gameObject.SetActive(true);

        currentRider = null;
    }
}
