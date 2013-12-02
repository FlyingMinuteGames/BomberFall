using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class MenuConfig{

    public static String[] m_keybindings_labels = {
                                               "Go Forward/Jump", 
                                               "Go Backward", 
                                               "Go Left", 
                                               "Go Right", 
                                               "Offensive Item"/*, 
                                               "Defensive Item"*/
                                          };

    public static String[] m_keybindings_default = {
                                               "Z",
                                               "S",
                                               "Q",
                                               "D",
                                               "E"/*,
                                               "A"
*/                                          };

    public static String[] m_keybindings = new String[m_keybindings_default.Length];

    public static String[] ratio_string = { "4/3", "16/10", "16/9" };
    public static String[] resolution_4_3 = { "640x480", "800x600", "1024x768", "1280x960", "1440x1080" };
    public static String[] resolution_16_10 = { "1280x800", "1440x900", "1680x1050", "1920x1200", "2560x1600" };
    public static String[] resolution_16_9 = { "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440" };
    public static String[] quality_string = { "Not that great", "Meh", "Better", "Can't go higher" };
    public static String[] gameplay_mode_string = { "Survival", "Arcade" };
    public static String[] aa_string = { "Disabled", "2x Multi Sampling", "4x Multi Sampling", "8x Multi Sampling" };
    public static String[] vsync_string = { "Disabled", "Enabled"};

   
    public static FileInfo[] mapFiles;

    public static String[] maps_string = { ""};

    public static String[] power_ups_string = { "As Neil Taught Me", "Back to School", "Bomb Squad", "Bring a Sword to a BombFight", "Fire Up", "Impenetrable Trinket", "Kick it Like you Mean it!", "Randomizatron teleporter", "Speed Up", "The Home Runner", "Vengeful Phenix" };
    public static bool[] power_ups_settings = new bool[power_ups_string.Length];
    
    public enum MainMenuSelected
    {
        NO_SELECTED,
        OPTION_SELECTED,
        PLAY_SELECTED,
    }
    public enum SubMenuSelected
    {
        NO_SELECTED,
        PRESENTATION_SELECTED,
        CONTROLS_SELECTED,
        HOST_SELECTED, 
        JOIN_SELECTED,
        LOBBY_SELECTED
    }

    public class ChatMessage
    {

        private string _message;

        public string message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _sender;

        public string sender
        {
            get { return _sender; }
            set { _sender = value; }
        }


        public ChatMessage(string sender, string message)
        {
            
            this.sender = sender;
            this.message = message;
        }
    }

    public class ConnectedPlayer
    {

        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _ready;

        public bool ready
        {
            get { return _ready; }
            set { _ready = value; }
        }


        public ConnectedPlayer(string name, bool ready)
        {

            this.name = name;
            this.ready = ready;
        }
    }

}