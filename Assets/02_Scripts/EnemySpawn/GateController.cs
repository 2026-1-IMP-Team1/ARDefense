using UnityEngine;

public class GateController : MonoBehaviour
{
    [Tooltip("The game object responsible for the visual representation of the gate")]
    public GameObject gateVisual; // Parent object containing the gate model, particles, etc.

    // Opens the gate (activates it).
    public void OpenGate()
    {
        if (gateVisual != null) gateVisual.SetActive(true);
    }

    // Closes the gate (deactivates it).
    public void CloseGate()
    {
        if (gateVisual != null) gateVisual.SetActive(false);
    }
}