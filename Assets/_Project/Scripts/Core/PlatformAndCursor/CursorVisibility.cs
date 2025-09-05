using UnityEngine;

public class CursorVisibility
{
    private const CursorLockMode LOCK_MODE_UI = CursorLockMode.None;
    private const CursorLockMode LOCK_MODE_GAMEPLAY = CursorLockMode.Locked;

    private readonly PlatformDefinition platform;

    public CursorVisibility(PlatformDefinition platform)
    {
        this.platform = platform;
    }

    public void ShowCursor()
    {
        if (platform.IsMobile)
            return;

        Cursor.visible = true;
        Cursor.lockState = LOCK_MODE_UI;
    }

    public void HideCursor()
    {
        if (platform.IsMobile)
            return;

        Cursor.visible = false;
        Cursor.lockState = LOCK_MODE_GAMEPLAY;
    }
}