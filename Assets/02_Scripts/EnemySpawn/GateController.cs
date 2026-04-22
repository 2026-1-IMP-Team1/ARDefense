using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject gateVisual;


    public void OpenGate()
    {
        if (gateVisual != null) gateVisual.SetActive(true);
    }

    public void CloseGate()
    {
        if (gateVisual != null) gateVisual.SetActive(false);
    }
}