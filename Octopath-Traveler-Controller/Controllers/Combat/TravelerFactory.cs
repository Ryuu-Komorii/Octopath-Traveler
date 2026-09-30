namespace Octopath_Traveler;

public class TravelerFactory
{
    private readonly IReadOnlyList<TravelerCatalogEntry>
        travelerCatalog;

    private readonly PassiveStatEffectFactory
        passiveStatEffectFactory;

    public TravelerFactory(
        IReadOnlyList<TravelerCatalogEntry> travelerCatalog)
    {
        this.travelerCatalog = travelerCatalog;

        passiveStatEffectFactory =
            new PassiveStatEffectFactory();
    }

    public Traveler Create(
        TravelerTeamMember teamMember)
    {
        TravelerCatalogEntry catalogEntry =
            FindCatalogEntry(teamMember.Name);

        TravelerStatValues statValues =
            CreateStatValues(
                catalogEntry,
                teamMember);

        return new Traveler(
            catalogEntry,
            teamMember,
            statValues);
    }

    private TravelerStatValues CreateStatValues(
        TravelerCatalogEntry catalogEntry,
        TravelerTeamMember teamMember)
    {
        TravelerStatValues statValues =
            new TravelerStatValues(
                catalogEntry.Stats);

        ApplyPassiveStatEffects(
            statValues,
            teamMember.PassiveSkillNames);

        return statValues;
    }

    private void ApplyPassiveStatEffects(
        TravelerStatValues statValues,
        IReadOnlyList<string> passiveSkillNames)
    {
        foreach (string passiveSkillName in passiveSkillNames)
        {
            ApplyPassiveStatEffect(
                statValues,
                passiveSkillName);
        }
    }

    private void ApplyPassiveStatEffect(
        TravelerStatValues statValues,
        string passiveSkillName)
    {
        PassiveStatEffect passiveEffect =
            passiveStatEffectFactory.Create(
                passiveSkillName);

        passiveEffect.Apply(statValues);
    }

    private TravelerCatalogEntry FindCatalogEntry(
        string travelerName)
    {
        return travelerCatalog.First(
            catalogEntry =>
                HasName(
                    catalogEntry,
                    travelerName));
    }

    private bool HasName(
        TravelerCatalogEntry catalogEntry,
        string travelerName)
    {
        return catalogEntry.Name ==
               travelerName;
    }
}