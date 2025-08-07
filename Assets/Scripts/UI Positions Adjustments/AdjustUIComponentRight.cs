using UnityEngine;

public class AdjustUIComponentRight : MonoBehaviour
{
    // This code assumes the health bar starts at the center of the sprite
    private float addedPosRight = 1f;
    private float addedPosUp = 0.5f;
    RectTransform m_RectTransform;
    [SerializeField] Renderer spriteRenderer;
    private void Start()
    {
        ChangeUIComponentRight();
    }

    private void OnEnable()
    {
        ChangeUIComponentRight();
    }
    private void FixedUpdate()
    {
        ChangeUIComponentRight();
    }

    private void ChangeUIComponentRight()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Parent Renderer not found.");
            return;
        }
        // Get the top Y position of the parent object
        float parentTopX = spriteRenderer.bounds.max.x;
        float parentCenterY = spriteRenderer.bounds.center.y;
        // Set this object's position to be right above the parent
        transform.position = new Vector3(
            parentTopX + addedPosRight,
            parentCenterY + addedPosUp,
            transform.position.z
        );
    }

}
