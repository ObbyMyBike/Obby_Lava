public class PlatformDefinition
{
    private readonly bool isMobile;

    public PlatformDefinition(bool isMobile)
    {
        this.isMobile = isMobile;
    }

    public bool IsMobile => isMobile;
}