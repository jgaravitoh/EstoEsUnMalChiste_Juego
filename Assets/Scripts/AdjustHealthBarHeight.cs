using UnityEngine;

public class AdjustHealthBarHeight : MonoBehaviour
{
    // This code assumes the health bar starts at the center of the sprite
    private float addedHeight = 0.2f;
    RectTransform m_RectTransform;
    private bool calledOnce = false;
    [SerializeField] Renderer spriteRenderer;
    private void Start()
    {
        ChangeHealthBarHeight();
        calledOnce = true;
    }

    private void OnEnable()
    {
        if (calledOnce)
        {
            ChangeHealthBarHeight();
        }
    }

    private void ChangeHealthBarHeight()
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
