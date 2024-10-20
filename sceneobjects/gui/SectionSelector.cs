namespace invaders.sceneobjects.gui;

public class SectionSelector() : SceneObject
{
    private List<Section> _sections = new();

    public void AddSection(Section section) => _sections.Add(section);
    
    public void ActivateSection(string name)
    {
        _sections.ForEach(s =>
        {
            if (s.Name == name) s.SetActive();
            else s.SetInactive();
        });
    }
    
    public class Section(string name)
    {
        public string Name = name;
        private List<ISectionable> _section = new();
        public void AddSectionObject(ISectionable sectionable) => _section.Add(sectionable);
        public void SetInactive() => _section.ForEach(s =>
        {
            s.SetInactiveSelection();
        });
        public void SetActive() => _section.ForEach(s => s.SetActiveSelection());
    }
}
