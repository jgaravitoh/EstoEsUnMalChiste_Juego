using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SelectedTrianlge : MonoBehaviour
{
    private string colorGreen = "#7FE144";
    private string colorRed = "#E14444";


    // This code assumes the health bar starts at the center of the sprite
    private float addedHeight = 0.4f;
    private Vector3 initialPosition;
    private float bounceAmplitude = 0.1f; // How high it bounces
    private float bounceFrequency = 8f;   // How fast it bounces

    private Image imageComponent;
    [SerializeField] private GameObject referenceObjectHeight;

    private void Start()
    {
        imageComponent = gameObject.GetComponent<Image>();
    }
    private void OnEnable()
    {
        PlaceHeight();
        initialPosition = transform.position;
    }

    private void PlaceHeight()
    {
        if (referenceObjectHeight == null)
        {
            Debug.LogWarning("Reference Object not found.");
            return;
        }
        // Get the top Y position of the parent object
        float parentTopY = referenceObjectHeight.transform.position.y;

        // Set this object's position to be right above the parent
        transform.position = new Vector3(
            transform.position.x,
            parentTopY + addedHeight,
            transform.position.z
        );
    }

    private void Update()
    {
        // Apply vertical bouncing effect based on sine wave
        float bounceOffset = Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;
        transform.position = new Vector3(
            initialPosition.x,
            initialPosition.y + bounceOffset,
            initialPosition.z
        );
    }

    public void ChangeTriangleColor(bool useGreen = true) //If useGreen is false the color will be red
    {
        if (imageComponent == null)
        {
            Debug.LogWarning("Image component not found.");
            return;
        }
        string colorToUse = useGreen ? colorGreen : colorRed;

        if (ColorUtility.TryParseHtmlString(colorToUse, out Color newColor))
        {
            imageComponent.color = newColor;
        }
        else
        {
            Debug.LogWarning("Invalid hex color: " + colorToUse);
        }
    }



}
