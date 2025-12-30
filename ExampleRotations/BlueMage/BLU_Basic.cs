using Dalamud.Interface.Colors;
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

    #region Tracking Properties
    public override void DisplayStatus()
    {
        var missingSpells = GetMissingSpells();
        var gatingActive = StopIfMissingSpells && missingSpells.Count > 0;

        ImGui.TextColored(ImGuiColors.DalamudViolet, "Blue Mage Rotation Tracking:");
        ImGui.Text($"Profile: {Profile}");
        ImGui.Text($"Use offensive oGCDs: {UseOffensiveOgcds}");
        ImGui.Text($"Use defensive oGCDs: {UseDefensiveOgcds}");
        ImGui.Text($"Stop if missing spells: {StopIfMissingSpells}");
        ImGui.Text($"AoE target threshold: {AoeTargetThreshold}");

        if (missingSpells.Count > 0)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, $"Missing spells: {string.Join(", ", missingSpells)}");
        }
        else
        {
            ImGui.TextColored(ImGuiColors.ParsedGreen, "Missing spells: none");
        }

        ImGui.TextColored(gatingActive ? ImGuiColors.DalamudRed : ImGuiColors.ParsedGreen,
            $"Gating active: {gatingActive}");

        ImGui.TextColored(ImGuiColors.DalamudViolet, "Base Tracking:");
        base.DisplayStatus();
    }
    #endregion

    private IReadOnlyList<string> GetMissingSpells()
    {
        // Placeholder for future spell checks. This keeps the tracking UI consistent while
        // avoiding hard dependencies on specific spell data in the example rotation.
        return Array.Empty<string>();
    }
}
