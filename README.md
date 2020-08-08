# SerpentsHand

A plugin that adds a new class to your server named "Serpent's Hand". This class works with the SCPs to eliminate all other beings. They have a chance to spawn instead of a squad of Chaos Insurgency.

# Installation

**[EXILED](https://github.com/galaxy119/EXILED) must be installed for this to work.**

Place the `SerpentsHand.dll` file in your `EXILED/Plugins` folder.

# Features
* Uses the tutorial model for this class
* Class has a configurable to spawn instead of chaos
* A custom spawn location
* Commands to spawn individual members and a squad manually
* Announcements for a squad of Serpent's Hand spawning, as well as one for chaos spawning to let the players know which one spawned

# Developer API Reference
See [this page](https://github.com/Cyanox62/SerpentsHand/wiki/API) for API usage.

# Commands
|     Command    | Value Type | Description |
| :-------------: | :---------: | :--------- |
| SPAWNSH | Player Name / SteamID64 | Spawns the specified player as Serpent's Hand. |
| SPAWNSHSQUAD | Squad Size | Spawns a Squad of Serpent's Hand, if no size is specified it will default to 5. This will trigger the squad spawn announcement. |
