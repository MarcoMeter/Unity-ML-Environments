using UnityEngine;
using UnityEngine.UI;

public class TestMovement : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    private Text valueText;
    [SerializeField]
    private GameObject _helper;
    [SerializeField]
    private Slider _moveSlider;
    [SerializeField]
    private Rigidbody _rigidbody;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Updates the UI.
    /// </summary>
    private void Update()
    {
        valueText.text = _moveSlider.value.ToString();
    }

    /// <summary>
    /// Executes the movement based on the slider's value.
    /// </summary>
    void FixedUpdate ()
    {
        // Get point from unit circle using an angle specified by the slider.
        Vector3 circumferencePoint = new Vector3((Mathf.Cos(_moveSlider.value * 180 * Mathf.Deg2Rad)),
                                                        (Mathf.Sin(_moveSlider.value * 180 * Mathf.Deg2Rad)),
                                                        0);
        // Apply velocity based on direction (coming from the unit circle)
        _rigidbody.velocity = circumferencePoint.normalized * 3;
        // Set the helper GameObject's position to visualize the direction.
        _helper.transform.position = transform.position + _rigidbody.velocity;
    }
    #endregion
}
