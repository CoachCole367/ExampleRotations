using Dalamud.Interface.Colors;
using System.ComponentModel;

namespace ExampleRotations.Healer;

[Rotation("Example", CombatType.PvE, GameVersion = "7.25")]
[SourceCode(Path = "main/ExampleRotations/Healer/WHM_Example.cs")]
[Api(5)]
public sealed class WHM_Example : WhiteMageRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Example true/false config")]
    public bool BoolExample { get; set; } = true;

    [Range(0, 40, ConfigUnitType.Pixels)]
    [RotationConfig(CombatType.PvE, Name = "Example pixel config")]
    public float PixelExample { get; set; } = 10;

    [Range(0, 40, ConfigUnitType.Seconds)]
    [RotationConfig(CombatType.PvE, Name = "Example time config")]
    public float TimeExample { get; set; } = 10;

    [Range(3, 25, ConfigUnitType.Yalms)]
    [RotationConfig(CombatType.PvE, Name = "Example distance/yalm config")]
    public float DistanceExample { get; set; } = 5;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Example percentage config")]
    public float PercentageExample { get; set; } = 0.3f;

    [Range(0, 10000, ConfigUnitType.None, 100)]
    [RotationConfig(CombatType.PvE, Name = "Example float config")]
    public float FloatValueExample { get; set; } = 1000;

    [RotationConfig(CombatType.PvE, Name = "Example multi option config")]
    public Examplemultioptionconfig Examplemultioptionconfigset { get; set; } = Examplemultioptionconfig.Option1;
    public enum Examplemultioptionconfig : byte
    {
        [Description("Example option 1")]
        Option1,

        [Description("Example option 2")]
        Option2,

        [Description("Example option 3")]
        Option3,
    }
    #endregion

    #region Countdown Logic
    protected override IAction? CountDownAction(float remainTime)
    {

        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    [RotationDesc()]
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool InterruptAbility(IAction nextGCD, out IAction? act)
    {

        return base.InterruptAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool DispelAbility(IAction nextGCD, out IAction? act)
    {

        return base.DispelAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool AntiKnockbackAbility(IAction nextGCD, out IAction? act)
    {

        return base.AntiKnockbackAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool SpeedAbility(IAction nextGCD, out IAction? act)
    {

        return base.SpeedAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool ProvokeAbility(IAction nextGCD, out IAction? act)
    {

        return base.ProvokeAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {

        return base.MoveForwardAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {

        return base.MoveBackAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {

        return base.HealAreaAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {

        return base.HealSingleAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {

        return base.DefenseAreaAbility(nextGCD, out act);
    }

    [RotationDesc()]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {

        return base.DefenseSingleAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {

        return base.GeneralAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic

    [RotationDesc()]
    protected override bool EmergencyGCD(out IAction? act)
    {

        return base.EmergencyGCD(out act);
    }

    [RotationDesc()]
    protected override bool MyInterruptGCD(out IAction? act)
    {

        return base.MyInterruptGCD(out act);
    }

    [RotationDesc()]
    protected override bool DispelGCD(out IAction? act)
    {

        return base.DispelGCD(out act);
    }

    [RotationDesc()]
    protected override bool RaiseGCD(out IAction? act)
    {

        return base.RaiseGCD(out act);
    }

    [RotationDesc()]
    protected override bool MoveForwardGCD(out IAction? act)
    {

        return base.MoveForwardGCD(out act);
    }

    [RotationDesc()]
    protected override bool HealAreaGCD(out IAction? act)
    {

        return base.HealAreaGCD(out act);
    }

    [RotationDesc()]
    protected override bool HealSingleGCD(out IAction? act)
    {

        return base.HealSingleGCD(out act);
    }

    [RotationDesc()]
    protected override bool DefenseAreaGCD(out IAction? act)
    {

        return base.DefenseAreaGCD(out act);
    }

    [RotationDesc()]
    protected override bool DefenseSingleGCD(out IAction? act)
    {

        return base.DefenseSingleGCD(out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods

    #endregion
}