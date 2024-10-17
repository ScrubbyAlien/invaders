using invaders.sceneobjects;

namespace invaders;

public static class Settings
{
    public const int FrameLimit = 144;
    public const string StartLevel = "mainmenu";
    public const int MarginSide = 24;
    public const int TopGuiHeight = 23 * (int)RenderObject.Scale;
    public const int SpawnInterval = 100 - TopGuiHeight;
    public const float AmbientScrollInLevel = 30;
    public const float AmbientScrollInTransition = 3000;
}