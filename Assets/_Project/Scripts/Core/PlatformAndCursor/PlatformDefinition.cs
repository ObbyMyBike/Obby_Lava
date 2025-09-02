public class PlatformDefinition : IPlatform
{
    private readonly bool isMobile;

    public PlatformDefinition(bool isMobile)
    {
        this.isMobile = isMobile;
    }

    bool IPlatform.IsMobile => isMobile;
}