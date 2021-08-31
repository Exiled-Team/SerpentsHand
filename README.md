# SerpentsHand

A plugin that adds a new class to your server named "Serpent's Hand". This class works with the SCPs to eliminate all other beings. They have a chance to spawn instead of a squad of Chaos Insurgency.

# Installation

**[EXILED](https://github.com/galaxy119/EXILED) must be installed for this to work.**

**If you have [AdminTools](https://github.com/galaxy119/AdminTools/tree/master/AdminTools) installed, make sure you set `admin_god_tuts: false` in your EXILED config.**

Place the "SerpentsHand.dll" file in your EXILED/Plugins folder.

# Features
* Uses the Tutorial model for this class
* Class has a configrable percent chance to spawn instead of chaos
* A configurable spawn location
* Commands to spawn individual members and a squad manually
* Announcements for a squad of Serpent's Hand spawning, as well as one for chaos spawning to let the players know which one spawned
* Custom API for other plugins to interact with
* Compatible with [RespawnTimer](https://github.com/Michal78900/RespawnTimer)

# Commands
All Serpents Hand commands begins with `sh` prefix.
| Command | Prefix | Required permission | Description | Example |
| :-------------: | :---------: | :---------: | :---------: | :---------:
| **list** | l | `sh.list` | Shows the list of players that are currently Serpents Hand. | `sh l`
| **spawn** | s | `sh.spawn` | Makes the player a Serpents Hand. (it uses IDs / nicknames as argument, if no argument is given, it will make the Command Sender a Serpents Hand) | `sh s 2` `sh s`
| **spawnteam** | st | `sh.spawnteam` | Spawns Serpents Hand team with given number of players (if no argument is given it will try to spawn a squad with max number provided in a config) **Keep in mind this command won't work if there is not enough Spectators** | `sh st`
