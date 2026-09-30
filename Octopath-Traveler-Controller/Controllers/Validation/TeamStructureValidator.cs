namespace Octopath_Traveler;

public class TeamStructureValidator
{
    private const int MinimumTravelerCount = 1;
    private const int MaximumTravelerCount = 4;
    private const int MinimumBeastCount = 1;
    private const int MaximumBeastCount = 5;
    private const int MaximumActiveSkillCount = 8;
    private const int MaximumPassiveSkillCount = 4;

    public bool IsValid(TeamDefinition teamDefinition)
    {
        return HasValidTravelerCount(teamDefinition)
            && HasValidBeastCount(teamDefinition)
            && HasNoRepeatedTravelers(teamDefinition)
            && HasNoRepeatedBeasts(teamDefinition)
            && TravelersHaveValidSkillCounts(teamDefinition)
            && TravelersHaveNoRepeatedSkills(teamDefinition);
    }

    private bool HasValidTravelerCount(TeamDefinition teamDefinition)
    {
        int travelerCount = teamDefinition.Travelers.Count;

        return travelerCount >= MinimumTravelerCount
            && travelerCount <= MaximumTravelerCount;
    }

    private bool HasValidBeastCount(TeamDefinition teamDefinition)
    {
        int beastCount = teamDefinition.BeastNames.Count;

        return beastCount >= MinimumBeastCount
            && beastCount <= MaximumBeastCount;
    }

    private bool HasNoRepeatedTravelers(TeamDefinition teamDefinition)
    {
        IEnumerable<string> travelerNames =
            teamDefinition.Travelers.Select(traveler => traveler.Name);

        return HasNoRepeatedNames(travelerNames);
    }

    private bool HasNoRepeatedBeasts(TeamDefinition teamDefinition)
    {
        return HasNoRepeatedNames(teamDefinition.BeastNames);
    }

    private bool HasNoRepeatedNames(IEnumerable<string> names)
    {
        int nameCount = names.Count();
        int distinctNameCount = names.Distinct().Count();

        return nameCount == distinctNameCount;
    }

    private bool TravelersHaveValidSkillCounts(TeamDefinition teamDefinition)
    {
        return teamDefinition.Travelers.All(HasValidSkillCounts);
    }

    private bool HasValidSkillCounts(TravelerTeamMember traveler)
    {
        return traveler.ActiveSkillNames.Count <= MaximumActiveSkillCount
            && traveler.PassiveSkillNames.Count <= MaximumPassiveSkillCount;
    }

    private bool TravelersHaveNoRepeatedSkills(TeamDefinition teamDefinition)
    {
        return teamDefinition.Travelers.All(HasNoRepeatedSkills);
    }

    private bool HasNoRepeatedSkills(TravelerTeamMember traveler)
    {
        return HasNoRepeatedNames(traveler.ActiveSkillNames)
            && HasNoRepeatedNames(traveler.PassiveSkillNames);
    }
}