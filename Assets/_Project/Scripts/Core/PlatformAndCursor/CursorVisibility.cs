using UnityEngine;

public class CursorVisibility : ICursorVisible
{
    private const CursorLockMode LOCK_MODE_UI = CursorLockMode.None;
    private const CursorLockMode LOCK_MODE_GAMEPLAY = CursorLockMode.Locked;

    private readonly IPlatform platform;

    public CursorVisibility(IPlatform platform)
    {
        this.platform = platform;
    }

    void ICursorVisible.ShowCursor()
    {
        if (platform.IsMobile)
            return;

        Cursor.visible = true;
        Cursor.lockState = LOCK_MODE_UI;
    }

    void ICursorVisible.HideCursor()
    {
        if (platform.IsMobile)
            return;

        Cursor.visible = false;
        Cursor.lockState = LOCK_MODE_GAMEPLAY;
    }
}