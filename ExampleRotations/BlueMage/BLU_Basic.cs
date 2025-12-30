using System.ComponentModel;

namespace ExampleRotations.BlueMage;

[Rotation("Blue Mage", CombatType.PvE, GameVersion = "7.25")]
[SourceCode(Path = "main/ExampleRotations/BlueMage/BLU_Basic.cs")]
[Api(5)]
public sealed class BLU_Basic : BlueMageRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Profile")]
    public BluProfile Profile { get; set; } = BluProfile.SingleTarget_Basic;

    [RotationConfig(CombatType.PvE, Name = "Use offensive oGCDs")]
    public bool UseOffensiveOgcds { get; set; } = true;

    [RotationConfig(CombatType.PvE, Name = "Use defensive oGCDs")]
    public bool UseDefensiveOgcds { get; set; } = true;

    [RotationConfig(CombatType.PvE, Name = "Stop if missing spells")]
    public bool StopIfMissingSpells { get; set; } = true;

    [Range(2, 10, ConfigUnitType.None)]
    [RotationConfig(CombatType.PvE, Name = "AoE target threshold")]
    public int AoeTargetThreshold { get; set; } = 3;

    public enum BluProfile : byte
    {
        [Description("Single Target - Basic")]
        SingleTarget_Basic,

        [Description("AoE - Basic")]
        AoE_Basic,
    }
    #endregion

    // With the current SDK surface only expose an empty action list so the
    // rotation registers cleanly without relying on unavailable helpers.
    protected override IBaseAction[] ActiveActions => Array.Empty<IBaseAction>();

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        act = null;
        return false;
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;
        return false;
    }
}
