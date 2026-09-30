namespace Octopath_Traveler;

public class PassiveStatEffectFactory
{
    private const string ElementalAugmentationName =
        "Elemental Augmentation";

    private const string SummonStrengthName =
        "Summon Strength";

    private const string HaleAndHeartyName =
        "Hale and Hearty";

    private const string FleefootName =
        "Fleefoot";

    private const string InnerStrengthName =
        "Inner Strength";

    public PassiveStatEffect Create(string passiveSkillName)
    {
        return passiveSkillName switch
        {
            ElementalAugmentationName =>
                new ElementalAugmentationEffect(),

            SummonStrengthName =>
                new SummonStrengthEffect(),

            HaleAndHeartyName =>
                new HaleAndHeartyEffect(),

            FleefootName =>
                new FleefootEffect(),

            InnerStrengthName =>
                new InnerStrengthEffect(),

            _ =>
                new NoPassiveStatEffect()
        };
    }
}