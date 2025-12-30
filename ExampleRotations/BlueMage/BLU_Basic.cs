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
        (new ActionID(ActionType.Spell, 11385), "Water Cannon"),
        (new ActionID(ActionType.Spell, 11433), "Sonic Boom"),
        (new ActionID(ActionType.Spell, 11430), "Moon Flute"),
    };

    private static readonly IReadOnlyList<(ActionID Id, string Name)> AoeRequiredActions = new List<(ActionID Id, string Name)>
    {
        (new ActionID(ActionType.Spell, 11385), "Water Cannon"),
        (new ActionID(ActionType.Spell, 11404), "Flamethrower"),
        (new ActionID(ActionType.Spell, 11418), "Surpanakha"),
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

    private bool HasSpell(ActionID id) => ActionManagerEx.TryGetAction(id, out var action) && action.IsUnlock;

    protected override IBaseAction[] ActiveActions => GetRequiredActionsForProfile()
        .Select(req => ActionManagerEx.TryGetAction(req.Id, out var action) ? action : null)
        .OfType<IBaseAction>()
        .ToArray();

    private static bool IsValidCombatTarget(out IBattleChara? target)
    {
        var player = Svc.ClientState.LocalPlayer;
        if (player is null || !Svc.Condition[ConditionFlag.InCombat])
        {
            target = null;
            return false;
        }

        target = Svc.Objects
            .OfType<IBattleChara>()
            .Where(enemy => enemy.ObjectKind == ObjectKind.BattleNpc
                && enemy.IsTargetable
                && !enemy.IsDead)
            .OrderBy(enemy => Vector3.Distance(player.Position, enemy.Position))
            .FirstOrDefault();

        return target is not null && Vector3.Distance(player.Position, target.Position) <= DefaultSpellRange;
    }

    private static int CountEnemiesInRange(IBattleChara center, float radius) => Svc.Objects.Count(obj => obj is IBattleChara enemy
        && enemy.ObjectKind == ObjectKind.BattleNpc
        && enemy.IsTargetable
        && !enemy.IsDead
        && Vector3.Distance(center.Position, enemy.Position) <= radius);

    private static bool TryGetReadyAction(ActionID actionId, out IAction? action)
    {
        if (ActionManagerEx.TryGetAction(actionId, out var candidate)
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

        if (ActionManagerEx.TryGetAction(new ActionID(ActionType.Spell, 11385), out var waterCannon)
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
            if (TryGetReadyAction(new ActionID(ActionType.Spell, 11430), out act))
            {
                return true;
            }
        }

        if (UseDefensiveOgcds && CanWeaveNow())
        {
            if (TryGetReadyAction(new ActionID(ActionType.Spell, 11390), out act))
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
            if (TryGetReadySpell(new ActionID(ActionType.Spell, 11404), target, out act))
            {
                return true;
            }

            if (TryGetReadySpell(new ActionID(ActionType.Spell, 11418), target, out act))
            {
                return true;
            }
        }

        if (TryGetReadySpell(new ActionID(ActionType.Spell, 11433), target, out act))
        {
            return true;
        }

        if (TryGetReadySpell(new ActionID(ActionType.Spell, 11385), target, out act))
        {
            return true;
        }

        act = null;
        return false;
    }
}
