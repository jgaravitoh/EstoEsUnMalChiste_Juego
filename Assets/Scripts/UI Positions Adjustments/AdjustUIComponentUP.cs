using UnityEngine;

public class AdjustUIComponentUP : MonoBehaviour
{
    // This code assumes the health bar starts at the center of the sprite
    private float addedHeight = 0.2f;
    RectTransform m_RectTransform;
    [SerializeField] Renderer spriteRenderer;
    private void Start()
    {
        ChangeUIComponentHeight();
    }

    private void OnEnable()
    {
        ChangeUIComponentHeight();
    }
    private void FixedUpdate()
    {
        ChangeUIComponentHeight();
    }

    private void ChangeUIComponentHeight()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Parent Renderer not found.");
            return;
        }
        // Get the top Y position of the parent object
        float parentTopY = spriteRenderer.bounds.max.y;
        // Set this object's position to be right above the parent
        transform.position = new Vector3(
            transform.position.x,
            parentTopY + addedHeight,
            transform.position.z
        );
    }

}
