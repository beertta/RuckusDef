using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootInputHandler : MonoBehaviour
{
    public ShapeRecognizerVR shapeRecognizer;
    public XRController rightController;

    void Update()
    {
        if (rightController.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool pressed) && pressed)
        {
            shapeRecognizer.DetectAndInstantiateShape();
        }
    }
}
