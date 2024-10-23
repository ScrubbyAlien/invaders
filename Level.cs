using invaders.enums;
using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects;
using invaders.sceneobjects.renderobjects.gui;
using SFML.Graphics;
using static invaders.Utility;
using SFML.System;
using SFML.Window;

namespace invaders;

public abstract class Level(string name)
{
    public readonly string Name = name;
    private readonly List<SceneObject> _initialObjects = new();

    public List<SceneObject> GetInitialObjects() => _initialObjects;

    protected void AddObject(SceneObject o) => _initialObjects.Add(o);

    public void CreateLevel() {
        ClearObjects();
        LoadObjects();
    }

    protected abstract void LoadObjects();

    private void ClearObjects() => _initialObjects.Clear();

    protected static void SetBackgroundMusic(string music = "") {
        if (music == "") {
            MusicManager.StopMusic();
            return;
        }

        MusicManager.ChangeMusic(music);
    }

    protected void SetBackground() {
        if (!Scene.FindByType(out Background? background)) {
            AddObject(new Background());
        }
        else {
            background!.Unpause();
        }
    }

    protected void CreateTopGuiBackground() {
        SpriteGUI blackBar = new(TextureRects["blackSquare"]);
        blackBar.Position = new Vector2f(0, 0);
        blackBar.SetScale(new Vector2f(Program.ScreenWidth, Settings.TopGuiHeight));
        blackBar.SetZIndex(290);
        AddObject(blackBar);

        SpriteGUI guiBackgroundMiddle = new(TextureRects["guiBackgroundMiddle"]);
        guiBackgroundMiddle.AddTag(SceneObjectTag.GuiBackgroundMiddle);
        guiBackgroundMiddle.Position = new Vector2f(
            (Program.ScreenWidth - guiBackgroundMiddle.Bounds.Width) / 2f,
            Settings.TopGuiHeight - guiBackgroundMiddle.Bounds.Height
        );
        guiBackgroundMiddle.SetZIndex(300);
        guiBackgroundMiddle.SetAvailableArea(new IntRect(7, 6, 82, 12));

        SpriteGUI guiBackgroundLeft = new(TextureRects["guiBackgroundLeft"]);
        guiBackgroundLeft.AddTag(SceneObjectTag.GuiBackgroundLeft);
        guiBackgroundLeft.Position = new Vector2f(0, Settings.TopGuiHeight - guiBackgroundLeft.Bounds.Height);
        guiBackgroundLeft.SetAvailableArea(new IntRect(7, 6, 34, 12));
        guiBackgroundLeft.SetZIndex(300);

        SpriteGUI guiBackgroundRight = new(TextureRects["guiBackgroundRight"]);
        guiBackgroundRight.AddTag(SceneObjectTag.GuiBackgroundRight);
        guiBackgroundRight.Position = new Vector2f(
            Program.ScreenWidth - guiBackgroundRight.Bounds.Width,
            Settings.TopGuiHeight - guiBackgroundRight.Bounds.Height
        );
        guiBackgroundRight.SetAvailableArea(new IntRect(2, 6, 34, 12));
        guiBackgroundRight.SetZIndex(300);
        AddObject(guiBackgroundMiddle);
        AddObject(guiBackgroundLeft);
        AddObject(guiBackgroundRight);
    }

    protected void CreatePauseMenu(string restartLevel) {
        AddObject(new PauseManager(Keyboard.Key.Escape));
        ButtonNavigator pauseMenu = new();
        pauseMenu.AddTag(SceneObjectTag.PauseMenuItem);

        SpriteGUI transparentScreen = new(TextureRects["blackSquare"]);
        transparentScreen.AddTag(SceneObjectTag.PauseMenuItem);
        transparentScreen.Position = new Vector2f(0, 0);
        transparentScreen.SetScale(new Vector2f(Program.ScreenWidth, Program.ScreenHeight));
        transparentScreen.SetColor(new Color(0, 0, 0, 170));
        transparentScreen.SetZIndex(1000);
        AddObject(transparentScreen);

        TextButtonGUI restartButton = new("restart");
        restartButton.AddTag(SceneObjectTag.PauseMenuItem);
        restartButton.Position = MiddleOfScreen(restartButton.Bounds, new Vector2f(0, -45));
        restartButton.SetZIndex(1100);
        AddObject(restartButton);

        TextButtonGUI quitButton = new("main menu");
        quitButton.AddTag(SceneObjectTag.PauseMenuItem);
        quitButton.Position = MiddleOfScreen(quitButton.Bounds, new Vector2f(0, 45));
        quitButton.SetZIndex(1100);
        AddObject(quitButton);

        pauseMenu.AddButton(restartButton, () => {
            GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
            Scene.LoadLevel(restartLevel);
            MusicManager.StopMusic();
        });
        pauseMenu.AddButton(quitButton, () => {
            GlobalEventManager.PublishBackgroundSetScrollSpeed(Settings.AmbientScrollInLevel, 1f);
            Scene.LoadLevel("mainmenu");
        });
        AddObject(pauseMenu);
    }
}