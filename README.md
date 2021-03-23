# SerpentsHand

A plugin that adds a new class to your server named "Serpent's Hand". This class works with the SCPs to eliminate all other beings. They have a chance to spawn instead of a squad of Chaos Insurgency.

# Installation

**[EXILED](https://github.com/galaxy119/EXILED) must be installed for this to work.**

**If you have [AdminTools](https://github.com/galaxy119/AdminTools/tree/master/AdminTools) installed, make sure you set `admin_god_tuts: false` in your EXILED config.**

Place the "SerpentsHand.dll" file in your EXILED/Plugins folder.

# Features
* Uses the tutorial model for this class
* Class has a configrable percent chance to spawn instead of chaos
* A custom spawn location
* Commands to spawn individual members and a squad manually
* Announcements for a squad of Serpent's Hand spawning, as well as one for chaos spawning to let the players know which one spawned
* Custom API for other plugins to interact with

# Configs
| Config        | Value Type | Default Value | Description |
| :-------------: | :---------: | :------: | :--------- |
| `is_enabled` | bool | true | Is the plugin enabled.
| `debug` | bool | false | Should debug messages be printed in a console.

## SerepentsHandModifiers
Configs for Serpents Hand player.
| Config        | Value Type | Default Value | Description |
| :-------------: | :---------: | :------: | :--------- |
| `role_name` | string | "Serpent's Hand" | Determines role name seen in game.
| `role_color` | string | '' | Determines color role name seen in game. (leave empty for default Tutorial green)
| `health` | float | 120 | The amount of health Serpents Hand has.
| `spawn_items` | List | GunProject90, KeycardChaosInsurgency, GrenadeFlash, Radio, Medkit | The items Serpents Hand spawn with. (supports [CustomItems](https://github.com/Exiled-Team/CustomItems))
| `spawn_ammo` | Dictionary | Nato556: 250, Nato762: 250, Nato9: 250 | The ammo Serpents Hand spawn with.
| `friendly_fire` | bool | false | Determines if friendly fire between Serpents Hand and SCPs is enabled.
| `teleport_to106` | bool | true | Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.
| `end_round_friendly_fire` | bool | false | Determines if Serpents Hand should be able to hurt SCPs after the round ends.
| `scps_win_with_chaos` | bool | true | Set this to false if Chaos and SCPs CANNOT win together on your server.

# Commands
All Serpents Hand commands begins with `sh` prefix.
| Command | Prefix | Required permission | Description | Example |
| :-------------: | :---------: | :---------: | :---------: | :---------:
| **list** | l | `sh.list` | Shows the list of players that are currently Serpents Hand. | `sh l`
| **spawn** | s | `sh.spawn` | Makes the player a Serpents Hand. (it uses IDs / nicknames as argument, if no argument is given, it will make the Command Sender a Serpents Hand) | `sh s 2` `sh s`
| **spawnteam** | st | `sh.spawnteam` | Spawns Serpents Hand team with given number of players (if no argument is given it will try to spawn a squad with max number provided in a config) **Keep in mind this command won't work if there is not enough Spectators** | `sh st`
