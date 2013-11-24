using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Config
{
    public const float CONST_GRAVITY = 9.8f;
    public const float CONST_FACTOR = 5.0f;
    public const int DEFAULT_PORT = 25256;
    public const int POOL_LAYER = 31;

    public enum GameMode
    {
        SURVIVAL,
        ARCADE
    }

    public enum PowerType
    {
        AS_NEIL_TM,
        BACK_TO_SCHOOL,
        BOMB_SQUAD,
        BRING_A_SW_TO_A_GF,
        FIRE_UP,
        IMPENETRABLE_TRINKET,
        KICK_IT_LIKE_U_MEAN_IT,
        RANDOM_TELEPORTER,
        SPEED_UP,
        THE_HOME_RUNNER,
        VENGEFUL_PHENIX
    }


}

public struct GameIntel
{
    public float game_duration;
    public Config.GameMode game_mode;
    public Config.PowerType[] power_ups;
    public int nb_players;
    public int nb_cpus;
    public bool auth_reco;
    public bool disable_persp_change;

    public GameIntel(float game_duration, int game_mode, bool[] power_ups, int nb_players, int nb_cpus, bool authorize_reconnection, bool disable_perspective_change)
    {
        this.game_duration = game_duration;
        this.game_mode = (Config.GameMode)game_mode;

        List<Config.PowerType> powers = new List<Config.PowerType>();

        for (int i = 0, len = power_ups.Length; i < len; i++){
            if (power_ups[i])
                powers.Add((Config.PowerType)i);
        }

        this.power_ups = powers.ToArray();
        this.nb_players = nb_players;
        this.nb_cpus = nb_cpus;
        this.auth_reco = authorize_reconnection;
        this.disable_persp_change = disable_perspective_change; 
    }
}
