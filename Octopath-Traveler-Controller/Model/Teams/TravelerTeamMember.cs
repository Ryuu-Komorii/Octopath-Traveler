namespace Octopath_Traveler;

public class TravelerTeamMember
{
    public string Name { get; }
    public IReadOnlyList<string> ActiveSkillNames { get; }
    public IReadOnlyList<string> PassiveSkillNames { get; }

    public TravelerTeamMember(
        string name,
        IReadOnlyList<string> activeSkillNames,
        IReadOnlyList<string> passiveSkillNames)
    {
        Name = name;
        ActiveSkillNames = activeSkillNames;
        PassiveSkillNames = passiveSkillNames;
    }
}