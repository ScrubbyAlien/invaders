using invaders.saving;
using invaders.sceneobjects;
using invaders.sceneobjects.renderobjects.gui;
using SFML.System;

namespace invaders.levels;

public class HighscoreLevel() : Level("highscores")
{
    private List<TextGUI> _loadedScores = new();
    
    protected override void LoadObjects()
    {
        SetBackgroundMusic("mainmenu");
        
        SetBackground();
        
        TabNavigator scoreBoardSelectTabs = new TabNavigator(true,true, true);
        AddObject(scoreBoardSelectTabs);
        
        TextButtonGUI standard = new TextButtonGUI("standard");
        standard.Position = new Vector2f(Settings.MarginSide, Settings.MarginSide);
        AddObject(standard);
        
        TextButtonGUI endless = new TextButtonGUI("endless");
        endless.Position = new Vector2f(Settings.MarginSide * 2 + standard.Bounds.Width, Settings.MarginSide);
        AddObject(endless);

        Dictionary<string, int> standardScores = LoadScores(false).Result;
        Dictionary<string, int> endlessScores = LoadScores(true).Result;
        
        scoreBoardSelectTabs.AddTab(standard, () => DisplayScores(standardScores));
        scoreBoardSelectTabs.AddTab(endless, () => DisplayScores(endlessScores));
        

        ButtonNavigator bottomButtons = new ButtonNavigator(false, false, true);
        AddObject(bottomButtons);
        
        TextButtonGUI backButton = new TextButtonGUI("Main menu");
        backButton.Position = new Vector2f(
            Settings.MarginSide,
            Program.ScreenHeight - backButton.Bounds.Height - Settings.MarginSide
        );
        AddObject(backButton);

        TextButtonGUI resetButton = new TextButtonGUI("reset");
        resetButton.Position = new Vector2f(
            Program.ScreenWidth - Settings.MarginSide - resetButton.Bounds.Width,
            Program.ScreenHeight - Settings.MarginSide - resetButton.Bounds.Height
        );
        AddObject(resetButton);
        
        bottomButtons.AddButton(backButton, () => Scene.LoadLevel("mainmenu"));
        bottomButtons.AddButton(resetButton, () =>
        {
            ConfirmationPrompt p = new(
                "Reset all scoreboards?",
                () =>
                {
                    SaveManager.WriteSave(new ScoresSaveObject()).Wait();
                    Scene.LoadLevel("highscores");
                },
                () => { }
                );
            p.Prompt();
        });
        
        // create section selector to change between tabs and buttons
        SectionSelector selector = new SectionSelector();
        AddObject(selector);
        
        SectionSelector.Section tabs = new SectionSelector.Section("tabs");
        SectionSelector.Section back = new SectionSelector.Section("back");
        
        scoreBoardSelectTabs.OrthogonalExit += (down) => { if (down) selector.ActivateSection("back"); };
        bottomButtons.OrthogonalExit += (down) => { if (!down) selector.ActivateSection("tabs"); };
        
        // navigators handle enabling and disabling of their INavigatables
        tabs.AddSectionObject(scoreBoardSelectTabs);
        back.AddSectionObject(bottomButtons);
        
        selector.AddSection(tabs);
        selector.AddSection(back);
        selector.ActivateSection("tabs");

        if (LevelInfo<bool>.Catch("endless", false))
        {
            Scene.DeferredCall(scoreBoardSelectTabs, "SetIndex", [1]);
        }
       
    }

    private void DisplayScores(Dictionary<string, int> scores)
    {
        Scene.QueueDestroy(_loadedScores.Select(t => t as SceneObject).ToList());
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
        
        foreach (KeyValuePair<string,int> pair in sortedScores)
        {
            TextGUI rank = new TextGUI(rankNumber.ToString());
            TextGUI name = new TextGUI(pair.Key);
            TextGUI score = new TextGUI(pair.Value.ToString());

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
        Scene.QueueSpawn(_loadedScores.Take(30).Select(t => t as SceneObject).ToList());
    }
    
    private async Task<Dictionary<string, int>> LoadScores(bool endless)
    {
        ScoresSaveObject s = await SaveManager.LoadSave<ScoresSaveObject>();
        Dictionary<string, int> scores = endless ? s.EndlessScores : s.Scores;
        return scores;
    }
    
}