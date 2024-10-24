using invaders.saving;
using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects.gui;
using SFML.System;

namespace invaders.levels;

public class HighscoreLevel() : Level("highscores")
{
    private readonly List<TextGUI> _loadedScores = new();

    protected override void LoadObjects() {
        SetBackgroundMusic("mainmenu");

        SetBackground();

        TabNavigator scoreBoardSelectTabs = new(true, true, true);
        AddObject(scoreBoardSelectTabs);

        TextButtonGUI standard = new("standard");
        standard.Position = new Vector2f(Settings.MarginSide, Settings.MarginSide);
        AddObject(standard);

        TextButtonGUI endless = new("endless");
        endless.Position = new Vector2f(Settings.MarginSide * 2 + standard.Bounds.Width, Settings.MarginSide);
        AddObject(endless);

        Dictionary<string, int> standardScores = LoadScores(false).Result;
        Dictionary<string, int> endlessScores = LoadScores(true).Result;

        scoreBoardSelectTabs.AddTab(standard, () => DisplayScores(standardScores));
        scoreBoardSelectTabs.AddTab(endless, () => DisplayScores(endlessScores));

        ButtonNavigator bottomButtons = new(false, false, true);
        AddObject(bottomButtons);

        TextButtonGUI backButton = new("Main menu");
        backButton.Position = new Vector2f(
            Settings.MarginSide,
            Program.ScreenHeight - backButton.Bounds.Height - Settings.MarginSide
        );
        AddObject(backButton);

        TextButtonGUI resetButton = new("reset");
        resetButton.Position = new Vector2f(
            Program.ScreenWidth - Settings.MarginSide - resetButton.Bounds.Width,
            Program.ScreenHeight - Settings.MarginSide - resetButton.Bounds.Height
        );
        AddObject(resetButton);

        bottomButtons.AddButton(backButton, () => Scene.LoadLevel("mainmenu"));
        bottomButtons.AddButton(resetButton, () => {
            ConfirmationPrompt p = new(
                "Reset all scoreboards?",
                () => {
                    SaveManager.WriteSave(new ScoresSaveObject()).Wait();
                    Scene.LoadLevel("highscores");
                },
                () => { }
            );
            p.Prompt();
        });

        // create section selector to change between tabs and buttons
        SectionSelector selector = new();
        AddObject(selector);

        SectionSelector.Section tabs = new("tabs");
        SectionSelector.Section back = new("back");

        scoreBoardSelectTabs.OrthogonalExit += down => {
            if (down) selector.ActivateSection("back");
        };
        bottomButtons.OrthogonalExit += down => {
            if (!down) selector.ActivateSection("tabs");
        };

        // navigators handle enabling and disabling of their INavigatables
        tabs.AddSectionObject(scoreBoardSelectTabs);
        back.AddSectionObject(bottomButtons);

        selector.AddSection(tabs);
        selector.AddSection(back);
        selector.ActivateSection("tabs");

        if (LevelInfo<bool>.Catch("endless", false)) {
            Scene.DeferredCall(scoreBoardSelectTabs, "SetIndex", [1]);
        }
    }

    private void DisplayScores(Dictionary<string, int> scores) {
        Scene.QueueDestroy(_loadedScores.Select(t => (SceneObject)t).ToList());
        _loadedScores.Clear();

        // LINQ queries taken from these resources
        // https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries
        // https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
        Dictionary<string, int> sortedScores = (
            from entry in scores
            orderby entry.Value descending
            select entry
        ).ToDictionary();

        float nextEntryYPosition = new TextGUI("A").Bounds.Height + Settings.MarginSide * 3;
        int rankNumber = 1;

        foreach (KeyValuePair<string, int> pair in sortedScores) {
            TextGUI rank = new(rankNumber.ToString());
            TextGUI name = new(pair.Key);
            TextGUI score = new(pair.Value.ToString());

            rank.Position = new Vector2f(Settings.MarginSide, nextEntryYPosition);
            name.Position = new Vector2f(Settings.MarginSide * 5, nextEntryYPosition);
            score.Position = new Vector2f(
                Program.ScreenWidth - Settings.MarginSide - score.Bounds.Width,
                nextEntryYPosition
            );

            _loadedScores.Add(rank);
            _loadedScores.Add(name);
            _loadedScores.Add(score);

            nextEntryYPosition += name.Bounds.Height + Settings.MarginSide;
            rankNumber++;
        }

        // we take the first 30 texts so we only display the first 10 entries since each entry has three texts
        _loadedScores
            .Take(30)
            .ForEach(o => o.Spawn());
    }

    private static async Task<Dictionary<string, int>> LoadScores(bool endless) {
        ScoresSaveObject s = await SaveManager.LoadSave<ScoresSaveObject>();
        Dictionary<string, int> scores = endless ? s.EndlessScores : s.Scores;
        return scores;
    }
}