using UnityEngine;
using UnityEngine.UI;
using Vuforia;

// Handles the event when a Vuforia target is detected.
// Spawns the AR world and disables UI elements once the target is recognized.
public class MyTargetHandler : MonoBehaviour
{
    public GameObject world;
    public GameObject uiCanvasToDisable;
    public GameObject eventSystemToDisable;
    public GameObject playerCapsule;
    public Text debugText;

    private bool hasSpawned = false;
    private ObserverBehaviour observerBehaviour;
    // Initialize observer and UI states
    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;

        if (world != null)
            world.SetActive(false);

        if (debugText != null)
            debugText.text = "Status: Waiting...";
    }
    // Triggered when the Vuforia target status changes
    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (debugText != null)
            debugText.text = "Status: " + status.Status.ToString();

        if (!hasSpawned &&
            (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED))
        {
            hasSpawned = true;
        // Spawn world only once when target is tracked
            if (world != null)
            {
                world.SetActive(true);
                world.transform.position = transform.position;
                world.transform.SetParent(null, true);
                Debug.Log("World activated and detached from Model Target.");
            }
            // Disable UI and event system
            if (uiCanvasToDisable != null)
                uiCanvasToDisable.SetActive(false);

            if (eventSystemToDisable != null)
                eventSystemToDisable.SetActive(false);
            // Disable tracking once world is spawned
            if (observerBehaviour != null && world.activeSelf)
            {
                observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
                observerBehaviour.enabled = false;
                Debug.Log("Observer disabled.");
            }

            if (debugText != null)
                debugText.text = "Cube recognized!";
        }
    }
}
