namespace Octopath_Traveler;

public static class DamageTypes
{
    public const string Sword = "Sword";
    public const string Spear = "Spear";
    public const string Dagger = "Dagger";
    public const string Axe = "Axe";
    public const string Bow = "Bow";
    public const string Stave = "Stave";

    public const string Fire = "Fire";
    public const string Ice = "Ice";
    public const string Lightning = "Lightning";
    public const string Wind = "Wind";
    public const string Light = "Light";
    public const string Dark = "Dark";

    public static readonly IReadOnlyList<string> Physical =
    [
        Sword,
        Spear,
        Dagger,
        Axe,
        Bow,
        Stave
    ];

    public static bool IsPhysical(
        string damageType)
    {
        return Physical.Contains(
            damageType);
    }
}