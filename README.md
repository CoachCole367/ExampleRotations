# Example Rotations for RSR

This is a forkable template for creating 3rd party rotations for RSR.

## Installation
Load URL for latest example rotation DLL:
```
https://github.com/FFXIV-CombatReborn/ExampleRotations/releases/latest/download/ExampleRotations.dll
```

## Blue Mage profiles
Two basic Blue Mage profiles are provided:
- **Single Target - Basic**: Focuses on single-target encounters.
  - Required spells: Water Cannon, Sonic Boom, Moon Flute.
- **AoE - Basic**: Prioritizes AoE actions when enemies meet the configured threshold.
  - Required spells: Water Cannon, Flamethrower, Surpanakha.

If the rotation is missing any required spells, the UI shows the missing list. With **Stop if missing spells** enabled (default), the rotation halts until the spells are learned; when disabled, it falls back to casting Water Cannon whenever possible.
