using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons.DalamudServices;
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

    private static readonly IReadOnlyList<(ActionID Id, string Name)> SingleTargetRequiredActions = new List<(ActionID Id, string Name)>
    {
        (ActionID.MakeSpell(11385), "Water Cannon"),
        (ActionID.MakeSpell(11433), "Sonic Boom"),
        (ActionID.MakeSpell(11430), "Moon Flute"),
    };

    private static readonly IReadOnlyList<(ActionID Id, string Name)> AoeRequiredActions = new List<(ActionID Id, string Name)>
    {
        (ActionID.MakeSpell(11385), "Water Cannon"),
        (ActionID.MakeSpell(11404), "Flamethrower"),
        (ActionID.MakeSpell(11418), "Surpanakha"),
    };

    private const float DefaultSpellRange = 25f;
    private const float DefaultAoeRadius = 8f;

    private IReadOnlyList<string> _missingSpells = Array.Empty<string>();
    private string _missingSpellStatus = "Missing spells: none";

    private IReadOnlyList<(ActionID Id, string Name)> GetRequiredActionsForProfile()
    {
        return Profile switch
        {
            BluProfile.AoE_Basic => AoeRequiredActions,
            _ => SingleTargetRequiredActions,
        };
    }

    private bool HasSpell(ActionID id)
    {
        return ActionHelper.TryGetAction(id, out var action) && action.IsUnlock;
    }

    protected override IBaseAction[] ActiveActions => GetRequiredActionsForProfile()
        .Select(req => ActionHelper.TryGetAction(req.Id, out var action) ? action : null)
        .OfType<IBaseAction>()
        .ToArray();

    private static bool IsValidCombatTarget(out IBattleChara? target)
    {
        target = Svc.Targets.Target as IBattleChara;

        if (target is null || !target.IsTargetable || target.IsDead)
        {
            return false;
        }

        if (!Svc.Condition[ConditionFlag.InCombat])
        {
            return false;
        }

        var player = Svc.ClientState.LocalPlayer;
        if (player is null)
        {
            return false;
        }

        return Vector3.Distance(player.Position, target.Position) <= DefaultSpellRange;
    }

    private static int CountEnemiesInRange(IBattleChara center, float radius)
    {
        return Svc.Objects.Count(obj => obj is IBattleChara enemy
            && enemy.ObjectKind == ObjectKind.BattleNpc
            && enemy is IBattleNpc battleNpc
            && battleNpc.BattleNpcSubKind == BattleNpcSubKind.Enemy
            && enemy.IsTargetable
            && !enemy.IsDead
            && Vector3.Distance(center.Position, enemy.Position) <= radius);
    }

    private static bool TryGetReadyAction(ActionID actionId, out IAction? action)
    {
        if (ActionHelper.TryGetAction(actionId, out var candidate)
            && candidate.IsUnlock
            && candidate.IsReady)
        {
            action = candidate;
            return true;
        }

        action = null;
        return false;
    }

    private static bool TryGetReadySpell(ActionID actionId, IBattleChara target, out IAction? action)
    {
        if (!TryGetReadyAction(actionId, out var candidate))
        {
            action = null;
            return false;
        }

        var player = Svc.ClientState.LocalPlayer;
        if (player is null)
        {
            action = null;
            return false;
        }

        if (Vector3.Distance(player.Position, target.Position) > DefaultSpellRange)
        {
            action = null;
            return false;
        }

        action = candidate;
        return true;
    }

    private static bool CanWeaveNow()
    {
        var player = Svc.ClientState.LocalPlayer;
        return player is not null && !player.IsCasting;
    }

    private void RefreshMissingSpells()
    {
        var required = GetRequiredActionsForProfile();
        _missingSpells = required
            .Where(req => !HasSpell(req.Id))
            .Select(req => req.Name)
            .ToList();

        _missingSpellStatus = _missingSpells.Count > 0
            ? $"Missing spells: {string.Join(", ", _missingSpells)}"
            : "Missing spells: none";
    }

    private bool ShouldStopForMissingSpells(out IAction? act)
    {
        RefreshMissingSpells();

        if (_missingSpells.Count == 0)
        {
            act = null;
            return false;
        }

        if (StopIfMissingSpells)
        {
            act = null;
            return true;
        }

        if (ActionHelper.TryGetAction(ActionID.MakeSpell(11385), out var waterCannon)
            && waterCannon.IsUnlock
            && waterCannon.IsReady)
        {
            act = waterCannon;
            return true;
        }

        act = null;
        return true;
    }

    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        if (ShouldStopForMissingSpells(out act))
        {
            return act != null;
        }

        if (!IsValidCombatTarget(out _))
        {
            act = null;
            return false;
        }

        if (UseOffensiveOgcds && CanWeaveNow())
        {
            if (TryGetReadyAction(ActionID.MakeSpell(11430), out act))
            {
                return true;
            }
        }

        if (UseDefensiveOgcds && CanWeaveNow())
        {
            if (TryGetReadyAction(ActionID.MakeSpell(11390), out act))
            {
                return true;
            }
        }

        return base.GeneralAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        if (!IsValidCombatTarget(out var target))
        {
            act = null;
            return false;
        }

        if (ShouldStopForMissingSpells(out act))
        {
            return act != null;
        }

        var aoeTargetCount = CountEnemiesInRange(target, DefaultAoeRadius);

        if (Profile == BluProfile.AoE_Basic && aoeTargetCount >= AoeTargetThreshold)
        {
            if (TryGetReadySpell(ActionID.MakeSpell(11404), target, out act))
            {
                return true;
            }

            if (TryGetReadySpell(ActionID.MakeSpell(11418), target, out act))
            {
                return true;
            }
        }

        if (TryGetReadySpell(ActionID.MakeSpell(11433), target, out act))
        {
            return true;
        }

        if (TryGetReadySpell(ActionID.MakeSpell(11385), target, out act))
        {
            return true;
        }

        act = null;
        return false;
    }
}
