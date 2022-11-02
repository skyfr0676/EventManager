[![Discord](https://img.shields.io/discord/940677414474903612?color=red&label=Discord&logo=discord&logoColor=white&style=flat)](https://discord.gg/exGDTaZweY) [![DownLoads](https://img.shields.io/github/downloads/skyyt15/EventManager/total?color=red&label=DownLoads&logo=github&style=flat)](https://github.com/skyyt15/EventManager/releases) [![Watchers](https://img.shields.io/github/watchers/skyyt15/EventManager?logo=github&logoColor=red&style=flat)](https://github.com/skyyt15/EventManager/watchers) [![Last Release](https://img.shields.io/github/release-date/skyyt15/EventManager?color=red&style=flat)](https://github.com/skyyt15/EventManager/releases)
# EventManager
EventManager is an [Scp: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/) plugin created to add event system for server to add some... [fun](https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley) in the game.

## index
>- <a href="readme.md/installation">Installation</a>
>- <a href="readme.md/config">Configuration</a>

# installation

you need the **latest** [EXILED](https://github.com/Exiled-team/EXILED/releases) version installed in to your server.

put on ['EventManager.dll'](https://github.com/skyyt15/EventManager/releases) file in 'EXILED\Plugins\' Path

# config

in 'EXILED\Config\', you probably found **'{your port}-config.yml'**. in to the config, you found 11 config for 'EventManager'.
Let's explain it in a table!


| Config Name | Description | Type
| :-------------: | :---------: | :---------:
| gun_game_doorunauthorized | called when the player want to open hcz armory, checkpoint with the gungame was started. | **string**
| gun_game_elevunauthorized | called when the player want to use elevator with the gungame was started. | **string**
| gun_game_winner_msg_for_lost | called when the gun game was finished and this message displayed to all the losers | **string**
| gun_game_winner_msg_for_winner | called when the gun game was finished and this message displayed to the winner | **string**
| gun_game_winner_duration | used for the *gun_game_winner_msg_for_lost* and *gun_game_winner_msg_for_winner* message duration | **int**
| gun_game_time_before_respawning | it's the time between the respawn | **float**
| gun_game_dead_msg | called when the player was dead | **string**
| gun_game_late_join_msg | called when the player join after the gungame was started | **string**
| gun_game_next_level | called when the player win a level | **string**
| gun_game_lost_level | called when the player kill him | **string**
| gun_game_spawn_protect_time | used for the spawn protect time (0 = none) | **float**
