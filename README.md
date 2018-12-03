# SerpentsHand

A plugin that adds a new class to your server named "Serpent's Hand".

# Installation

**[Smod2](https://github.com/Grover-c13/Smod2) must be installed for this to work.**

Place the "PlayerXP.dll" file in your sm_plugins folder.

# Features
* Uses the tutorial model for this class
* Class has a 50% chance (can be changed in configs) to spawn instead of chaos
* A custom spawn location
* Commands to spawn individual members and a squad manually
* Announcements for a squad of Serpent's Hand spawning, as well as one for chaos spawning to let the players know which one spawned
* Unique ways for Serpent's Hand to work with SCPs, including utilizing 106's ability to walk through doors to their advantage
* Disables Chaos winning with SCPs (can be changed in configs)

# Configs
| Config        | Value Type | Default Value | Description |
| :-------------: | :---------: | :------: | :--------- |
| sh_spawn_chance | 1-100 | 50 | The percent chance for a squad of Serpent's Hand to spawn instead of chaos. |
| sh_entry_announcement | String | serpents hand entered | The announcement to be played when Serpent's Hand are spawned, sentences must be written exactly to work with CASSIE's available phrases (Ex. serpents hand . number two hundred and thirty two), all phrases can be found [here](https://github.com/Cyanox62/CustomAnnouncements/wiki/CASSIE-Phrases). |
| sh_ci_entry_announcement | String | | The annoumcement to be played when Chaos spawn instead of Serpent's Hand, same rules as the other announcement config apply |
| sh_spawn_items | List | 20,26,12,14,10 | The item IDs that Serpent's Hand members should spawn with. A full list of item IDs can be found [here](https://github.com/Cyanox62/SerpentsHand/wiki/Item-IDs). |
| sh_friendly_fire | Boolean | False | Should SCPs and Serpent's Hand be able to hurt eachother. This includes 106's pocket dimension, with this disabled, Serpent's Hand members will never die no matter which exit they take in the Pocket Dimension. **Note: Having AdminToolbox installed will prevent Serpent's Hand members from hurting eachother regardless of this config option.** |
| sh_teleport_to_106 | Boolean | True | When a Serpent's hand member escapes the Pocket Dimension, should they teleport to 106 instead of spawning at his chamber. |
| sh_ci_win_with_scp | Boolean | False | Should the round end with SCPs and Chaos left. |
| sh_health | Number | 120 | How much health Serpent's Hand members will have. |

# Commands
| Command        | Value Type | Description |
| :-------------: | :---------: | :--------- |
| SPAWNSH | Player Name / SteamID64 | Spawns the specified player as Serpent's Hand. |
| SPAWNSHSQUAD | Squad Size | Spawns a Squad of Serpent's Hand, if no size is specified it will default to 5. This will trigger the squad spawn announcement. |
