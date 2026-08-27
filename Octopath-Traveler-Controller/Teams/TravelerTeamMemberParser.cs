using System.Text.RegularExpressions;

namespace Octopath_Traveler;

public class TravelerTeamMemberParser
{
    private const string TravelerLinePattern =
        @"^(?<name>.*?)(?:\s*\((?<activeSkills>.*?)\))?(?:\s*\[(?<passiveSkills>.*?)\])?$";

    private const string NameGroup = "name";
    private const string ActiveSkillsGroup = "activeSkills";
    private const string PassiveSkillsGroup = "passiveSkills";
    private const char SkillSeparator = ',';

    public TravelerTeamMember Parse(string travelerLine)
    {
        Match travelerMatch = Regex.Match(travelerLine, TravelerLinePattern);

        string travelerName = GetGroupValue(travelerMatch, NameGroup);
        string[] activeSkillNames = GetSkillNames(travelerMatch, ActiveSkillsGroup);
        string[] passiveSkillNames = GetSkillNames(travelerMatch, PassiveSkillsGroup);

        return new TravelerTeamMember(
            travelerName,
            activeSkillNames,
            passiveSkillNames);
    }

    private string GetGroupValue(Match travelerMatch, string groupName)
    {
        return travelerMatch.Groups[groupName].Value.Trim();
    }

    private string[] GetSkillNames(Match travelerMatch, string groupName)
    {
        string skillSection = GetGroupValue(travelerMatch, groupName);

        return skillSection.Split(
            SkillSeparator,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}