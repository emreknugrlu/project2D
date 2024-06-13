using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    public RectTransform uiElement; // The UI element you want to follow the player
    public Transform player; // The player object to follow
    public Camera mainCamera; // The camera rendering the UI

    void Update()
    {
        if (player != null && uiElement != null && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position);

            // Add an offset if necessary
            Vector3 offset = new Vector3(0, 50, 0);
            uiElement.position = screenPos + offset;

            // Optional: Clamp the position to keep it on the screen
            screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);

            uiElement.position = screenPos;
        }
    }
}

