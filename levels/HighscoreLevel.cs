using System.Diagnostics;
using invaders.saving;
using invaders.sceneobjects;
using invaders.sceneobjects.gui;
using SFML.System;

namespace invaders.levels;

public class HighscoreLevel() : Level("highscores")
{
    private List<TextGUI> _loadedScores = new List<TextGUI>();
    
    protected override void LoadObjects()
    {
        if (!Scene.FindByType(out Background background))
        {
            AddObject(new Background());
        }
        else
        {
            background.Unpause();
        }

        SectionSelector selector = new SectionSelector();
        SectionSelector.Section tabs = new SectionSelector.Section("tabs");
        SectionSelector.Section back = new SectionSelector.Section("back");
        
        TabNavigator scoreBoardSelectTabs = new TabNavigator(false, true);
        
        TextButtonGUI standardScores = new TextButtonGUI("standard");
        standardScores.Position = new Vector2f(Settings.MarginSide, Settings.MarginSide);
        AddObject(standardScores);
        
        TextButtonGUI endlessScores = new TextButtonGUI("endless");
        endlessScores.Position = new Vector2f(Settings.MarginSide * 2 + standardScores.Bounds.Width, Settings.MarginSide);
        AddObject(endlessScores);
        
        
        scoreBoardSelectTabs.AddTab(standardScores, () => DisplayScores(false).Wait());
        scoreBoardSelectTabs.AddTab(endlessScores, () => DisplayScores(true).Wait());
        

        ButtonNavigator backButtonManager = new ButtonNavigator(false, true);
        AddObject(backButtonManager);
        
        TextButtonGUI backButton = new TextButtonGUI("back");
        backButton.Position = new Vector2f(
            Settings.MarginSide,
            Program.ScreenHeight - backButton.Bounds.Height - Settings.MarginSide);
        AddObject(backButton);
        
        
        backButtonManager.AddButton(backButton, () => Scene.LoadLevel("mainmenu"));

        scoreBoardSelectTabs.OrthogonalExit += (down) =>
        {
            if (down)
            {
                selector.ActivateSection("back");
            }
        };
        backButtonManager.OrthogonalExit += (down) =>
        {
            if (!down)
            {
                selector.ActivateSection("tabs");
            }
        };
        
        AddObject(scoreBoardSelectTabs);
        
        // navigators handle enabling and disabling of their INavigatables
        tabs.AddSectionObject(scoreBoardSelectTabs);
        back.AddSectionObject(backButtonManager);
        
        selector.AddSection(tabs);
        selector.AddSection(back);
        selector.ActivateSection("tabs");
        AddObject(selector);
    }

    private async Task DisplayScores(bool endless)
    {
        Scene.QueueDestroy(_loadedScores.Select(t => t as SceneObject).ToList());
        _loadedScores = await LoadScores(endless);
        Scene.QueueSpawn(_loadedScores.Take(10).Select(t => t as SceneObject).ToList());
    }
    
    private async Task<List<TextGUI>> LoadScores(bool endless)
    {
        List<TextGUI> scoreTexts = new();
        
        ScoresSaveObject s = await SaveManager.LoadSave<ScoresSaveObject>();
        Dictionary<string, int> scores = endless ? s.EndlessScores : s.Scores;

        float nextEntryYPosition = new TextGUI("A").Bounds.Height + Settings.MarginSide * 3;
        
        // LINQ queries taken from these resources
        // https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries
        // https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
        Dictionary<string, int> sortedScores = (
            from entry in scores
            orderby entry.Value ascending 
            select entry
            ).ToDictionary();
        
        foreach (KeyValuePair<string,int> pair in sortedScores)
        {
            TextGUI name = new TextGUI(pair.Key);
            TextGUI score = new TextGUI(pair.Value.ToString());

            name.Position = new Vector2f(Settings.MarginSide, nextEntryYPosition);
            score.Position = new Vector2f(
                Program.ScreenWidth - Settings.MarginSide - score.Bounds.Width,
                nextEntryYPosition
            );
            
           scoreTexts.Add(name);
           scoreTexts.Add(score);

           nextEntryYPosition += name.Bounds.Height + Settings.MarginSide;
        }

        return scoreTexts;
    }
    
}