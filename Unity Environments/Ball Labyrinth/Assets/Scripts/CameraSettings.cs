using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSettings : MonoBehaviour
{
    private float previousShadowDistance;

    /// <summary>
    /// Disables shadows for this camera.
    /// </summary>
    private void OnPreRender()
    {
        previousShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    /// <summary>
    /// Resets the shadow distance for the other cameras.
    /// </summary>
    private void OnPostRender()
    {
        QualitySettings.shadowDistance = previousShadowDistance;
    }
}
