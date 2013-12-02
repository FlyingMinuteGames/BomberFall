/**
 * MainMenuScript
 *  --> Script Handling all menu components including
 *      - every player options
 *      - game launches
 *      - game settings
 *      - game lobby
 *      - connection to game
 *      - game hosting
 *      
 * Author: Jean-Vincent Lamberti
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MainMenuScript : MonoBehaviour {

    public GUISkin skin;
    public Texture logo;
    private GameObject networkManager; // Prefab

    private GameObject instantiatedMaster; //Prefab instancié
    private NetworkMgr scriptStartNet;

    private string serverIP = "127.0.0.1";
    private int serverPort = 25000;




    private MenuConfig.MainMenuSelected menu;
    private MenuConfig.SubMenuSelected submenu;
    private MenuConfig.SubMenuSelected prev_submenu;
    private int m_ratio = 0;
    private int m_resolution = 0;
    private int quality = 0;
    private float m_music_volume = 10.0f;
    private float m_sound_effects_volume = 10.0f;
    private bool m_fullscreen = false;
    private bool isHost;
    private int m_antialiasing;
    private int m_vsync;

    private float m_game_duration = 30f;
    private int m_gameplay_mode = 0;
    private int m_map_index = 0;
    private int m_nb_players = 2;
    private int m_nb_CPUs = 0;
    private bool m_auth_reco = false;
    private bool m_disable_persp_change = false;
    private string m_server_password = "";
    private string m_message = "";
    private string m_sender = "Player 1";

    private PopUpScript popup;

   

    private List<MenuConfig.ChatMessage> m_chat_messages;

    private List<MenuConfig.ConnectedPlayer> m_connected_players;


    private Vector2 m_keybindings_scrollPosition = Vector2.zero;
    private Vector2 m_chat_scrollPosition = Vector2.zero;

    private GUIContent[] ratio_combobox;

    private GUIContent[] _4_3_combobox;
    private GUIContent[] _16_10_combobox;
    private GUIContent[] _16_9_combobox;

    private GUIContent[] gameplay_mode_combobox;
    private GUIContent[] maps_combobox;
    private GUIContent[] nb_players_combobox;
    private GUIContent[] nb_CPUs_combobox;

    
    private GUIContent[] m_aa;
    private GUIContent[] m_quality;
    private GUIContent[] m_vsyncContent;


    private ComboBox comboBoxControl = new ComboBox();
    private ComboBox comboBoxResolution = new ComboBox();
    private ComboBox comboBoxQuality = new ComboBox();
    private ComboBox comboBoxGamemode = new ComboBox();
    private ComboBox comboBoxMaps = new ComboBox();
    private ComboBox comboboxNbPlayers = new ComboBox();
    private ComboBox comboboxNbCPUs = new ComboBox();
    private ComboBox comboboxAA = new ComboBox();
    private ComboBox comboboxVsync = new ComboBox();


    private bool hasMapFiles;
    private GameMgr gameMgr;



    void Start()
    {/*
    }

	//Initialization block
	void Awake() {*/



        Screen.showCursor = true;
        Screen.lockCursor = false;

        popup = gameObject.transform.FindChild("PopupContainer").gameObject.GetComponent<PopUpScript>();

        ////Initialize Option Values
        MenuConfig.m_keybindings = new String[MenuConfig.m_keybindings_labels.Length];

        hasMapFiles = MenuUtils.LoadMapsList();

        ratio_combobox = new GUIContent[3];
        _4_3_combobox = new GUIContent[MenuConfig.resolution_4_3.Length];
        _16_10_combobox = new GUIContent[MenuConfig.resolution_16_10.Length];
        _16_9_combobox = new GUIContent[MenuConfig.resolution_16_9.Length];
        m_quality = new GUIContent[MenuConfig.quality_string.Length];
        m_aa = new GUIContent[MenuConfig.aa_string.Length];
        m_vsyncContent = new GUIContent[MenuConfig.vsync_string.Length];


        gameplay_mode_combobox = new GUIContent[MenuConfig.gameplay_mode_string.Length];
        maps_combobox = new GUIContent[MenuConfig.maps_string.Length];

        m_chat_messages = new List<MenuConfig.ChatMessage>();
        m_connected_players = new List<MenuConfig.ConnectedPlayer>();


        menu = MenuConfig.MainMenuSelected.NO_SELECTED;
        submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
        for (int i = 0; i < MenuConfig.ratio_string.Length; i++)
            ratio_combobox[i] = new GUIContent(MenuConfig.ratio_string[i]);

        for (int i = 0; i < MenuConfig.resolution_4_3.Length; i++)
            _4_3_combobox[i] = new GUIContent(MenuConfig.resolution_4_3[i]);
        for (int i = 0; i < MenuConfig.resolution_16_10.Length; i++)
            _16_10_combobox[i] = new GUIContent(MenuConfig.resolution_16_10[i]);
        for (int i = 0; i < MenuConfig.resolution_16_9.Length; i++)
            _16_9_combobox[i] = new GUIContent(MenuConfig.resolution_16_9[i]);

        for (int i = 0; i < MenuConfig.quality_string.Length; i++)
            m_quality[i] = new GUIContent(MenuConfig.quality_string[i]);
        for (int i = 0; i < MenuConfig.aa_string.Length; i++)
            m_aa[i] = new GUIContent(MenuConfig.aa_string[i]);
        for (int i = 0; i < MenuConfig.vsync_string.Length; i++)
            m_vsyncContent[i] = new GUIContent(MenuConfig.vsync_string[i]);




        for (int i = 0; i < MenuConfig.gameplay_mode_string.Length; i++)
            gameplay_mode_combobox[i] = new GUIContent(MenuConfig.gameplay_mode_string[i]);
        for (int i = 0; i < MenuConfig.maps_string.Length; i++)
            maps_combobox[i] = new GUIContent(MenuConfig.maps_string[i]);


        nb_players_combobox = MenuUtils.SetComboboxRange(2, 4);
        nb_CPUs_combobox = MenuUtils.SetComboboxRange(0, 3);




        skin.customStyles[0].hover.background = skin.customStyles[0].onHover.background = new Texture2D(2, 2);
        
        //Creates player prefs setted to default values if they don't exist
        InitializePlayerPrefs();

        //Sets Key Bindings accordingly to playerprefs values
        LoadFromPlayerPrefs("keybindings");

        //Sets Video Options accordingly to playerprefs values
        LoadFromPlayerPrefs("video");
        comboBoxControl.SetSelectedItemIndex(m_ratio);
        comboBoxQuality.SetSelectedItemIndex(quality);
        comboBoxResolution.SetSelectedItemIndex(m_resolution);
        comboboxAA.SetSelectedItemIndex(m_antialiasing);
        comboboxVsync.SetSelectedItemIndex(m_vsync);


        //LOAD FROM PLAYER PREFS
        LoadFromPlayerPrefs("host_params");

        comboBoxGamemode.SetSelectedItemIndex(m_gameplay_mode);
        comboBoxMaps.SetSelectedItemIndex(m_map_index);
        comboboxNbPlayers.SetSelectedItemIndex(m_nb_players);
        comboboxNbCPUs.SetSelectedItemIndex(m_nb_CPUs);
        Debug.Log("MainMenu init");
        gameMgr = GameMgr.Instance;

        active = true;
	}

    public void Reset()
    {
        menu = MenuConfig.MainMenuSelected.NO_SELECTED;
        submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
    }

    /*
     * InitializePlayerPrefs()
     *  Creates the player prefs regarding options and preferences
     * */
    void InitializePlayerPrefs()
    {

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);

        if (!PlayerPrefs.HasKey("SoundVolume"))
            PlayerPrefs.SetFloat("SoundVolume", 0.8f);

        if (!PlayerPrefs.HasKey("AspectRatio"))
            PlayerPrefs.SetInt("AspectRatio", 2);

        if (!PlayerPrefs.HasKey("Resolution"))
            PlayerPrefs.SetInt("Resolution", 0);

        if (!PlayerPrefs.HasKey("QualityLevel"))
            PlayerPrefs.SetInt("QualityLevel", 2);

        if (!PlayerPrefs.HasKey("Fullscreen"))
            PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);

        if (!PlayerPrefs.HasKey("MenuKey"))
                PlayerPrefs.SetInt("MenuKey", (int)KeyCode.Escape);

        if (!PlayerPrefs.HasKey("JumpKey"))
            PlayerPrefs.SetInt("JumpKey", (int)KeyCode.Space);

        if (!PlayerPrefs.HasKey("ForwardKey"))
            PlayerPrefs.SetInt("ForwardKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[0]));

        if (!PlayerPrefs.HasKey("BackwardKey"))
            PlayerPrefs.SetInt("BackwardKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[1]));

        if (!PlayerPrefs.HasKey("LeftKey"))
            PlayerPrefs.SetInt("LeftKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[2]));

        if (!PlayerPrefs.HasKey("RightKey"))
            PlayerPrefs.SetInt("RightKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[3]));
       
        if (!PlayerPrefs.HasKey("DefensiveItemKey"))
            PlayerPrefs.SetInt("DefensiveItemKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[4]));


        if (!PlayerPrefs.HasKey("OffensiveItemKey"))
            PlayerPrefs.SetInt("OffensiveItemKey", (int)MenuUtils.GetKeyCode(MenuConfig.m_keybindings_default[5]));

        //if (!PlayerPrefs.HasKey("antmPower"))
            PlayerPrefs.SetInt("antmPower", 0);
        
        //if (!PlayerPrefs.HasKey("btsPower"))
            PlayerPrefs.SetInt("btsPower", 0);
        
        //if (!PlayerPrefs.HasKey("bsPower"))
            PlayerPrefs.SetInt("bsPower", 0);
        
        if (!PlayerPrefs.HasKey("bastabfPower"))
            PlayerPrefs.SetInt("bastabfPower", 1);
        
        if (!PlayerPrefs.HasKey("fuPower"))
            PlayerPrefs.SetInt("fuPower", 1);
        
        //if (!PlayerPrefs.HasKey("itPower"))
            PlayerPrefs.SetInt("itPower", 0);
        
        //if (!PlayerPrefs.HasKey("kilymiPower"))
            PlayerPrefs.SetInt("kilymiPower", 0);
        
        //if (!PlayerPrefs.HasKey("rtPower"))
            PlayerPrefs.SetInt("rtPower", 0);
        
        //if (!PlayerPrefs.HasKey("suPower"))
            PlayerPrefs.SetInt("suPower", 0);
        
        //if (!PlayerPrefs.HasKey("thrPower"))
            PlayerPrefs.SetInt("thrPower", 0);
        
        //if (!PlayerPrefs.HasKey("vpPower"))
            PlayerPrefs.SetInt("vpPower", 0);

        if (!PlayerPrefs.HasKey("gameDuration"))
            PlayerPrefs.SetFloat("gameDuration", 30f);

        if (!PlayerPrefs.HasKey("gameplayMode"))
            PlayerPrefs.SetInt("gameplayMode", 0);

        if (!PlayerPrefs.HasKey("nbPlayers"))
            PlayerPrefs.SetInt("nbPlayers", 2);

        //if (!PlayerPrefs.HasKey("nbCPUs"))
            PlayerPrefs.SetInt("nbCPUs", 0);

        if (!PlayerPrefs.HasKey("allowReconnection"))
            PlayerPrefs.SetInt("allowReconnection", 0);

        if (!PlayerPrefs.HasKey("disablePerspectiveChange"))
            PlayerPrefs.SetInt("disablePerspectiveChange", 0);

        if (!PlayerPrefs.HasKey("serverPassword"))
            PlayerPrefs.SetString("serverPassword", "");

        if (!PlayerPrefs.HasKey("serverIP"))
            PlayerPrefs.SetString("serverIP", "127.0.0.1");

        if (!PlayerPrefs.HasKey("AntiAliasing"))
            PlayerPrefs.SetInt("AntiAliasing", 0);

        if (!PlayerPrefs.HasKey("VSync"))
            PlayerPrefs.SetInt("VSync", 0);

        PlayerPrefs.Save();
    }


    /**
     * LoadResolution()
     *  --> loads and set the resolutions to the parameters given by the user in the options menu
     * */
    void LoadResolution()
    {
        String resSt;
        if (m_ratio == 0)
            resSt = MenuConfig.resolution_4_3[m_resolution];
        else if (m_ratio == 1)
            resSt = MenuConfig.resolution_16_10[m_resolution];
        else
            resSt = MenuConfig.resolution_16_9[m_resolution];


        String[] resTab = resSt.Split('x');

        Screen.SetResolution(int.Parse(resTab[0]), int.Parse(resTab[1]), m_fullscreen);

    }

    /**
     * LoadFromPlayerPrefs(String st)
     *  -> assign corresponding values to UI elements  from PlayerPrefs matching st selector
     * */
    void LoadFromPlayerPrefs(String st = "")
    {
        if (st.Equals("video"))
        {
            m_fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
            m_ratio = PlayerPrefs.GetInt("AspectRatio");
            quality = PlayerPrefs.GetInt("QualityLevel");
            m_resolution = PlayerPrefs.GetInt("Resolution");
            QualitySettings.SetQualityLevel(quality, true);
            m_sound_effects_volume = PlayerPrefs.GetFloat("SoundVolume") * 10;
            m_music_volume = PlayerPrefs.GetFloat("MusicVolume") * 10;
            m_vsync = PlayerPrefs.GetInt("VSync");
            m_antialiasing = PlayerPrefs.GetInt("AntiAliasing");

        }
        else if (st.Equals("keybindings"))
        {
           MenuConfig.m_keybindings[0] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("ForwardKey"));
           MenuConfig.m_keybindings[1] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("BackwardKey"));
           MenuConfig.m_keybindings[2] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("LeftKey"));
           MenuConfig.m_keybindings[3] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("RightKey"));
           MenuConfig.m_keybindings[4] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("OffensiveItemKey"));

           //MenuConfig.m_keybindings[5] = MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("DefensiveItemKey"));
        }
        else if (st.Equals("host_params"))
        {
            MenuConfig.power_ups_settings[0] = (1 == PlayerPrefs.GetInt("antmPower") ? true : false);
            MenuConfig.power_ups_settings[1] = (1 == PlayerPrefs.GetInt("btsPower") ? true : false);
            MenuConfig.power_ups_settings[2] = (1 == PlayerPrefs.GetInt("bsPower") ? true : false);
            MenuConfig.power_ups_settings[3] = (1 == PlayerPrefs.GetInt("bastabfPower") ? true : false);
            MenuConfig.power_ups_settings[4] = (1 == PlayerPrefs.GetInt("fuPower") ? true : false);
            MenuConfig.power_ups_settings[5] = (1 == PlayerPrefs.GetInt("itPower") ? true : false);
            MenuConfig.power_ups_settings[6] = (1 == PlayerPrefs.GetInt("kilymiPower") ? true : false);
            MenuConfig.power_ups_settings[7] = (1 == PlayerPrefs.GetInt("rtPower") ? true : false);
            MenuConfig.power_ups_settings[8] = (1 == PlayerPrefs.GetInt("suPower") ? true : false);
            MenuConfig.power_ups_settings[9] = (1 == PlayerPrefs.GetInt("thrPower") ? true : false);
            MenuConfig.power_ups_settings[10] = (1 == PlayerPrefs.GetInt("vpPower") ? true : false);
            m_game_duration = PlayerPrefs.GetFloat("gameDuration");
            m_gameplay_mode = PlayerPrefs.GetInt("gameplayMode");
            m_nb_players = PlayerPrefs.GetInt("nbPlayers");
            m_nb_CPUs = PlayerPrefs.GetInt("nbCPUs");
            m_auth_reco = (1 == PlayerPrefs.GetInt("allowReconnection") ? true : false);
            m_disable_persp_change = (1 == PlayerPrefs.GetInt("disablePerspectiveChange") ? true : false);
            m_server_password = PlayerPrefs.GetString("serverPassword");

            hasMapFiles = MenuUtils.LoadMapsList();

        }
        else if (st.Equals("join_params")){
            serverIP = PlayerPrefs.GetString("serverIP");
        }
    }

    /**
     * SetPlayerPrefs(String st)
     *  --> sets the player prefs to match ui component values matching st selector
     * */
    void SetPlayerPrefs(String st = "")
    {
        if (st.Equals("video"))
        {
            bool _ReloadNeeded = false, vaaSetted = false;

            int _ratio = PlayerPrefs.GetInt("AspectRatio");
            int _resolution = PlayerPrefs.GetInt("Resolution");
            bool _fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            int _quality = PlayerPrefs.GetInt("QualityLevel");
            int _aa = PlayerPrefs.GetInt("AntiAliasing");
            int _vs = PlayerPrefs.GetInt("VSync");

            if (_ratio != m_ratio || _resolution != m_resolution)
                _ReloadNeeded = true;

            PlayerPrefs.SetInt("Fullscreen", m_fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("AspectRatio", m_ratio);
            PlayerPrefs.SetInt("Resolution", m_resolution);
            PlayerPrefs.SetInt("QualityLevel", quality);
            PlayerPrefs.SetInt("AntiAliasing", m_antialiasing);
            PlayerPrefs.SetInt("VSync", m_vsync);

            PlayerPrefs.SetFloat("SoundVolume", m_sound_effects_volume / 10);
            PlayerPrefs.SetFloat("MusicVolume", m_music_volume / 10);


            if (_fullscreen != m_fullscreen)
                Screen.fullScreen = m_fullscreen;

            if (_ReloadNeeded)
                LoadResolution();

            if (_quality != quality)
            {
                QualitySettings.SetQualityLevel(quality);
                QualitySettings.antiAliasing = m_antialiasing;
                QualitySettings.vSyncCount = m_vsync;
                vaaSetted = true;
            }

            if (_aa != m_antialiasing && !vaaSetted)
            {
                switch(m_antialiasing){
                    case 0: _aa = 0; break;
                    case 1: _aa = 2; break;
                    case 2: _aa = 4; break;
                    case 3: _aa = 8; break;
                }
                QualitySettings.antiAliasing = _aa;
            }

            if (_vs != m_vsync && !vaaSetted)
                QualitySettings.vSyncCount = m_vsync;

        }
        else if (st.Equals("keybindings"))
        {
            KeyCode kb;
            kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[0]);
            if (kb != KeyCode.Dollar)
                PlayerPrefs.SetInt("ForwardKey", (int)kb);
            kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[1]);
            if (kb != KeyCode.Dollar)
                PlayerPrefs.SetInt("BackwardKey", (int)kb);
            kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[2]);
            if (kb != KeyCode.Dollar)
                PlayerPrefs.SetInt("LeftKey", (int)kb);
            kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[3]);
            if (kb != KeyCode.Dollar)
                PlayerPrefs.SetInt("RightKey", (int)kb);
            kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[4]);
            if (kb != KeyCode.Dollar)
                PlayerPrefs.SetInt("OffensiveItemKey", (int)kb);
            //kb = MenuUtils.GetKeyCode(MenuConfig.m_keybindings[5]);
            //if (kb != KeyCode.Dollar)
            //    PlayerPrefs.SetInt("DefensiveItemKey", (int)kb);

        }
        else if (st.Equals("host_params"))
        {
            PlayerPrefs.SetInt("antmPower", MenuConfig.power_ups_settings[0] ? 1 : 0);
            PlayerPrefs.SetInt("btsPower", MenuConfig.power_ups_settings[1] ? 1 : 0);
            PlayerPrefs.SetInt("bsPower", MenuConfig.power_ups_settings[2] ? 1 : 0);
            PlayerPrefs.SetInt("bastabfPower", MenuConfig.power_ups_settings[3] ? 1 : 0);
            PlayerPrefs.SetInt("fuPower", MenuConfig.power_ups_settings[4] ? 1 : 0);
            PlayerPrefs.SetInt("itPower", MenuConfig.power_ups_settings[5] ? 1 : 0);
            PlayerPrefs.SetInt("kilymiPower", MenuConfig.power_ups_settings[6] ? 1 : 0);
            PlayerPrefs.SetInt("rtPower", MenuConfig.power_ups_settings[7] ? 1 : 0);
            PlayerPrefs.SetInt("suPower", MenuConfig.power_ups_settings[8] ? 1 : 0);
            PlayerPrefs.SetInt("thrPower", MenuConfig.power_ups_settings[9] ? 1 : 0);
            PlayerPrefs.SetInt("vpPower", MenuConfig.power_ups_settings[10] ? 1 : 0);


            PlayerPrefs.SetFloat("gameDuration", m_game_duration);
            PlayerPrefs.SetInt("gameplayMode", m_gameplay_mode);
            PlayerPrefs.SetInt("nbPlayers", m_nb_players);
            PlayerPrefs.SetInt("nbCPUs", m_nb_CPUs);
            PlayerPrefs.SetInt("allowReconnection", m_auth_reco ? 1 : 0);
            PlayerPrefs.SetInt("disablePerspectiveChange", m_disable_persp_change ? 1 : 0);
            PlayerPrefs.SetString("serverPassword", m_server_password);

        }
        else if (st.Equals("join_params")){
            PlayerPrefs.SetString("serverIP", serverIP);
        }


        PlayerPrefs.Save();
    }

    /**
     *  Detects keydown and should test for intended key, but it doesn't
     * */
    bool GUIKeyDown(KeyCode key)
    {
        if (Event.current.type == EventType.KeyDown)
            return true;
        return false;
    }
    public bool active = false;
    void OnGUI()
    {
        if (!active)
            return;

        GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(20, 50, 600 * 0.4f, 189 * 0.4f)), logo, ScaleMode.ScaleToFit);
        GUI.Box(MenuUtils.ResizeGUI(new Rect(10, 530, 780, 50)), "", skin.box);

        GUI.Label(MenuUtils.ResizeGUI(new Rect(20, 535, 700, 40)), "BOMBERFALL - Student Project made in 50 days by Cyril Basset and Jean-Vincent Lamberti using the UnityEngine", skin.label);
        GUI.Label(MenuUtils.ResizeGUI(new Rect(20, 550, 800, 40)), "Music by PocketMaster. All musics, sounds and fonts are under Creative Commons Licences and free for non-commercial use", skin.label);

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(20, 200, 100, 30)), "PLAY", skin.button))
        {
            if (menu != MenuConfig.MainMenuSelected.PLAY_SELECTED)
            {
                menu = MenuConfig.MainMenuSelected.PLAY_SELECTED;
                submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
            else
            {
                menu = MenuConfig.MainMenuSelected.NO_SELECTED;
                submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }

        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(20, 240, 100, 30)), "OPTIONS", skin.button))
        {

            if (menu != MenuConfig.MainMenuSelected.OPTION_SELECTED)
                menu = MenuConfig.MainMenuSelected.OPTION_SELECTED;
            else
            {
                menu = MenuConfig.MainMenuSelected.NO_SELECTED;
                submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(20, 280, 100, 30)), "QUIT", skin.button))
        {
            Application.Quit();
        }


        if (menu == MenuConfig.MainMenuSelected.OPTION_SELECTED)
        {

            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(140, 240, 100, 30)), "PRESENTATION", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.PRESENTATION_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.PRESENTATION_SELECTED;
                else submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(140, 280, 100, 30)), "CONTROLS", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.CONTROLS_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.CONTROLS_SELECTED;
                else submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
        }

        if (menu == MenuConfig.MainMenuSelected.PLAY_SELECTED)
        {

            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(140, 200, 100, 30)), "HOST", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.HOST_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.HOST_SELECTED;
                else submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(140, 240, 100, 30)), "JOIN", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.JOIN_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.JOIN_SELECTED;
                else submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
        }


        if (submenu == MenuConfig.SubMenuSelected.PRESENTATION_SELECTED)
        {

            GUI.Box(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)), "PRESENTATION SETTINGS", skin.box);
            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(280, 120, 500, 600)));


            GUI.Label(MenuUtils.ResizeGUI(new Rect(240, 55, 100, 40)), "Aspect Ratio :", skin.label);

            int selGrid;
            if ((selGrid = comboBoxQuality.List(MenuUtils.ResizeGUI(new Rect(70, 225, 150, 20)), m_quality[quality].text, m_quality, skin.button, skin.box, skin.customStyles[0])) != quality)
            {
                quality = selGrid;
            }


            GUI.Label(MenuUtils.ResizeGUI(new Rect(240, 255, 100, 40)), "V-Sync :", skin.label);

            if ((selGrid = comboboxVsync.List(MenuUtils.ResizeGUI(new Rect(325, 255, 150, 20)), m_vsyncContent[m_vsync].text, m_vsyncContent, skin.button, skin.box, skin.customStyles[0])) != m_vsync)
            {
                m_vsync = selGrid;
            }




            GUI.Label(MenuUtils.ResizeGUI(new Rect(240, 225, 100, 40)), "Anti Aliasing :", skin.label);

            if ((selGrid = comboboxAA.List(MenuUtils.ResizeGUI(new Rect(325, 225, 150, 20)), m_aa[m_antialiasing].text, m_aa, skin.button, skin.box, skin.customStyles[0])) != m_antialiasing)
            {
                m_antialiasing = selGrid;
            }


            GUI.Label(MenuUtils.ResizeGUI(new Rect(240, 145, 100, 40)), "Resolution :", skin.label);


            switch (m_ratio)
            {
                case 0:
                    if ((selGrid = comboBoxResolution.List(MenuUtils.ResizeGUI(new Rect(330, 145, 100, 20)), _4_3_combobox[m_resolution].text, _4_3_combobox, skin.button, skin.box, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
                case 1:
                    if ((selGrid = comboBoxResolution.List(MenuUtils.ResizeGUI(new Rect(330, 145, 100, 20)), _16_10_combobox[m_resolution].text, _16_10_combobox, skin.button, skin.box, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
                case 2:
                    if ((selGrid = comboBoxResolution.List(MenuUtils.ResizeGUI(new Rect(330, 145, 100, 20)), _16_9_combobox[m_resolution].text, _16_9_combobox, skin.button, skin.box, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
            }

            if ((selGrid = comboBoxControl.List(MenuUtils.ResizeGUI(new Rect(330, 55, 100, 20)), ratio_combobox[m_ratio].text, ratio_combobox, skin.button, skin.box, skin.customStyles[0])) != m_ratio)
            {
                m_ratio = selGrid;
            }


            GUI.Label(MenuUtils.ResizeGUI(new Rect(0, 30, 200, 40)), "Music Volume", skin.label);
            m_music_volume = GUI.HorizontalSlider(MenuUtils.ResizeGUI(new Rect(0, 60, 180, 20)), m_music_volume, 0, 10.0f, skin.horizontalSlider, skin.horizontalSliderThumb);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(195, 55, 40, 40)), Math.Round(m_music_volume * 10, 0).ToString() + "%", skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(0, 120, 300, 40)), "Sound Effects Volume", skin.label);
            m_sound_effects_volume = GUI.HorizontalSlider(MenuUtils.ResizeGUI(new Rect(0, 150, 180, 20)), m_sound_effects_volume, 0, 10.0f, skin.horizontalSlider, skin.horizontalSliderThumb);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(195, 145, 60, 40)), Math.Round(m_sound_effects_volume * 10, 0).ToString() + "%", skin.label);

            GUI.Label(MenuUtils.ResizeGUI(new Rect(0, 195, 85, 20)), "Fullscreen :", skin.label);

            m_fullscreen = GUI.Toggle(MenuUtils.ResizeGUI(new Rect(70, 195, 100, 20)), m_fullscreen, m_fullscreen ? "True" : "False", skin.toggle);


            GUI.Label(MenuUtils.ResizeGUI(new Rect(0, 225, 100, 40)), "Quality :", skin.label);


            GUI.EndGroup();

            SetPlayerPrefs("video");

        }

        if (submenu == MenuConfig.SubMenuSelected.CONTROLS_SELECTED)
        {

            LoadFromPlayerPrefs("controls");

            GUI.Box(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)), "CONTROLS SETTINGS", skin.box);
            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)));
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 30, 70, 40)), "Keybindings :", skin.label);
            //Key Bindings
            GUI.Box(MenuUtils.ResizeGUI(new Rect(80, 30, 400, 250)), "", skin.box);
            m_keybindings_scrollPosition = GUI.BeginScrollView(MenuUtils.ResizeGUI(new Rect(80, 30, 400, 250)), m_keybindings_scrollPosition, MenuUtils.ResizeGUI(new Rect(0, 0, 200, 25 * (MenuConfig.m_keybindings_labels.Length + 1))));
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 0, 400, 40)), "You can click and type any letter from A to Z to assign it", skin.label);

            int i;
            for (i = 0; i < MenuConfig.m_keybindings_labels.Length; i++)
            {
                GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 25 * (i + 1), 90, 40)), MenuConfig.m_keybindings_labels[i] + " :", skin.label);
                MenuConfig.m_keybindings[i] = GUI.TextField(MenuUtils.ResizeGUI(new Rect(130, 25 * (i + 1), 20, 20)), MenuConfig.m_keybindings[i], 1, skin.textField);
            }
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 25 * (i + 1), 80, 40)), "Jump :", skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(130, 25 * (i + 1), 80, 40)), "Space", skin.label);
            i++;
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 25 * (i + 1), 80, 40)), "Pause :", skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(130, 25 * (i + 1), 150, 40)), "Esc.", skin.label);



            GUI.EndScrollView();
            //End Key Bindings

            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(420, 290, 60, 20)), "Apply", skin.button))
            {
                SetPlayerPrefs("keybindings");
                LoadFromPlayerPrefs("keybindings");
            }
            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(350, 290, 60, 20)), "Default", skin.button))
            {
                MenuConfig.m_keybindings = MenuConfig.m_keybindings_default;
                SetPlayerPrefs("keybindings");
                LoadFromPlayerPrefs("keybindings");
            }

            GUI.EndGroup();
        }

        if (submenu == MenuConfig.SubMenuSelected.HOST_SELECTED)
        {

            LoadFromPlayerPrefs("host_params");

            GUI.Box(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)), "HOST SETTINGS", skin.box);
            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)));
            int selGrid, selGrid2, nbPlayers, nbCPUs;

            GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 85, 30, 40)), "Map", skin.label);

            if (hasMapFiles)
            {
                if ((selGrid2 = comboBoxMaps.List(MenuUtils.ResizeGUI(new Rect(90, 85, 150, 20)), maps_combobox[m_map_index].text, maps_combobox, skin.button, skin.box, skin.customStyles[0])) != m_map_index)
                {
                    m_map_index = selGrid2;
                }
            }
            else
                GUI.Label(MenuUtils.ResizeGUI(new Rect(90, 85, 150, 100)), "No maps found. Please move you map files to the Maps directory located at the root of the game directory.", skin.label);


            GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 30, 100, 40)), "Gameplay Mode", skin.label);

            if ((selGrid = comboBoxGamemode.List(MenuUtils.ResizeGUI(new Rect(150, 30, 70, 20)), gameplay_mode_combobox[m_gameplay_mode].text, gameplay_mode_combobox, skin.button, skin.box, skin.customStyles[0])) != m_gameplay_mode)
            {
                m_gameplay_mode = selGrid;
            }

            if (1 == selGrid)
            {
                GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 30, 100, 40)), "Game Duration", skin.label);
                m_game_duration = GUI.HorizontalSlider(MenuUtils.ResizeGUI(new Rect(350, 35, 85, 20)), m_game_duration, 30f, 300f, skin.horizontalSlider, skin.horizontalSliderThumb);
                GUI.Label(MenuUtils.ResizeGUI(new Rect(450, 30, 140, 40)), Math.Round(m_game_duration) + " sec.", skin.label);
            }


            GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 50, 50, 40)), "Players", skin.label);

            if ((nbPlayers = comboboxNbPlayers.List(MenuUtils.ResizeGUI(new Rect(310, 50, 25, 20)), nb_players_combobox[m_nb_players].text, nb_players_combobox, skin.button, skin.box, skin.customStyles[0])) != m_nb_players)
            {
               /* nb_CPUs_combobox = MenuUtils.SetComboboxRange( 0, 3 - nbPlayers);*/
                m_nb_players = nbPlayers;
            }

            //GUI.Label(MenuUtils.ResizeGUI(new Rect(345, 50, 50, 40)), "CPUs", skin.label);

            //if ((nbCPUs = comboboxNbCPUs.List(MenuUtils.ResizeGUI(new Rect(405, 50, 25, 20)), nb_CPUs_combobox[m_nb_CPUs].text, nb_CPUs_combobox, skin.button, skin.box, skin.customStyles[0])) != m_nb_CPUs)
            //{
            //    nb_players_combobox = MenuUtils.SetComboboxRange(1, 4 - nbCPUs);
            //    m_nb_CPUs = nbCPUs;
            //}

            GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 140, 200, 40)), "Active power-ups :", skin.label);

            for (int index = 0,j=0, len = MenuConfig.power_ups_settings.Length; index < len; index++)
            {
                if (index != 4 && index != 8 && index != 3)
                    continue;

                MenuConfig.power_ups_settings[index] = GUI.Toggle(MenuUtils.ResizeGUI(new Rect(260, 160 + (j * 20), 250, 20)), MenuConfig.power_ups_settings[index], MenuConfig.power_ups_string[index], skin.toggle);
                j++;
            }
            
            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(260, 300, 190, 80)), "Create Game", skin.button))
            {
                Debug.Log("Create game " + m_nb_players);
                gameMgr.gameIntel = new GameIntel(m_game_duration, m_gameplay_mode, MenuConfig.power_ups_settings, m_nb_players + 2, m_nb_CPUs, m_auth_reco, m_disable_persp_change, MenuConfig.maps_string[m_map_index]);
                gameMgr.maps = Maps.LoadMapsFromFile(MenuConfig.maps_string[m_map_index]);
                gameMgr.StartServer();
                gameMgr.StartClient("127.0.0.1");
                isHost = true;
                submenu = MenuConfig.SubMenuSelected.LOBBY_SELECTED;
            }



            //GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 260, 250, 20)), "Additionnal Settings :", skin.label);

            //GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 290, 150, 20)), "Server password", skin.label);

            //m_server_password = GUI.PasswordField(MenuUtils.ResizeGUI(new Rect(140, 290, 100, 20)), m_server_password, '*', skin.textField);


            //m_disable_persp_change = GUI.Toggle(MenuUtils.ResizeGUI(new Rect(50, 320, 190, 20)), m_disable_persp_change, "Disable perspective's change", skin.toggle);
            //m_auth_reco = GUI.Toggle(MenuUtils.ResizeGUI(new Rect(50, 340, 190, 20)), m_auth_reco, "Allow players to reconnect", skin.toggle);




            GUI.EndGroup();

            SetPlayerPrefs("host_params");
        }

        if (submenu == MenuConfig.SubMenuSelected.JOIN_SELECTED)
        {

            LoadFromPlayerPrefs("join_params");

            GUI.Box(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)), "JOIN A GAME", skin.box);
            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)));


            GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 70, 150, 20)), "Server IP", skin.label);

            serverIP = GUI.TextField(MenuUtils.ResizeGUI(new Rect(140, 70, 100, 20)), serverIP, skin.textField);


            //GUI.Label(MenuUtils.ResizeGUI(new Rect(50, 110, 150, 20)), "Server password", skin.label);

            //m_server_password = GUI.PasswordField(MenuUtils.ResizeGUI(new Rect(140, 110, 100, 20)), m_server_password, '*', skin.textField);

            if (GUI.Button(new Rect(140, 240, 190, 80), "Join game", skin.button))
            {

                if (gameMgr.StartClient(serverIP))
                {
                    isHost = false;
                    m_connected_players.Add(new MenuConfig.ConnectedPlayer("Player " + (m_connected_players.Count + 1), false));
                    submenu = MenuConfig.SubMenuSelected.LOBBY_SELECTED;
                }


            }


            GUI.EndGroup();

            SetPlayerPrefs("join_params");
        }

        if (submenu == MenuConfig.SubMenuSelected.LOBBY_SELECTED)
        {


            GUI.Box(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)), "GAME LOBBY", skin.box);
            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(260, 120, 500, 400)));


            //HOST SETTINGS OVERVIEW
            //GUI.Label(MenuUtils.ResizeGUI(new Rect(220, 50, 180, 40)), "Host settings overview :", skin.label);

            GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(120, 60, 300, 400)));
            GUI.Box(MenuUtils.ResizeGUI(new Rect(0, 0, 200, 200)), "SETTINGS OVERVIEW", skin.box);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 30, 150, 20)), "Game Mode :  " + (gameMgr.gameIntel.game_mode == Config.GameMode.ARCADE ? "Arcade" : "Survival"), skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 50, 150, 20)), "Map Name :  " + gameMgr.gameIntel.map_name, skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 70, 150, 20)), "Players :  " + gameMgr.gameIntel.nb_players, skin.label);
            if (gameMgr.gameIntel.game_mode == Config.GameMode.ARCADE)
                GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 90, 150, 20)), "Game Duration :  " + gameMgr.gameIntel.nb_players, skin.label);

            //GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 60, 150, 20)), "CPUs :  " + gameMgr.gameIntel.nb_cpus, skin.label);
            //GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 70, 150, 20)), "Perspective change :  " + (gameMgr.gameIntel.disable_persp_change ? "Deactivated" : "Activated"), skin.label);
            //GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 80, 150, 20)), "Reconnection :  " + (gameMgr.gameIntel.auth_reco ? "Authorized" : "Not Authorized"), skin.label);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 100, 150, 200)), "Active power-ups :  " + gameMgr.gameIntel.powers_str + "Bomb Up", skin.label);

            GUI.EndGroup();

            //CHAT SECTION
            GUI.Box(MenuUtils.ResizeGUI(new Rect(80, 300, 250, 65)), "", skin.box);
            m_chat_scrollPosition = GUI.BeginScrollView(MenuUtils.ResizeGUI(new Rect(80, 300, 250, 65)), m_chat_scrollPosition, MenuUtils.ResizeGUI(new Rect(0, 0, 200, 10 * (m_chat_messages.Count + 1))));


            for (int i = 0; i < m_chat_messages.Count; i++)
            {
                GUI.Label(MenuUtils.ResizeGUI(new Rect(10, 10 * i, 380, 65)), m_chat_messages[i].sender + " : " + m_chat_messages[i].message, skin.label);
            }



            GUI.EndScrollView();


            GUI.Label(MenuUtils.ResizeGUI(new Rect(40, 368, 80, 20)), "Message", skin.label);

            m_message = GUI.TextField(MenuUtils.ResizeGUI(new Rect(80, 365, 250, 20)), m_message, 150); //Allows Multi line input if using custom skin


            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(335, 365, 50, 20)), "Send", skin.button) || GUIKeyDown(KeyCode.Return))
            {
                if (m_message.Length > 0)
                {
                    if (gameMgr.s != null)
                    {
                        gameMgr.s.SendPacketBroadCast(PacketBuilder.BuildSendMessage("Server", m_message));
                        AddMessage("Server", m_message);
                    }
                    else if (gameMgr.c != null)
                    {
                        gameMgr.c.SendPacket(PacketBuilder.BuildSendMessage("Player", m_message));
                    }
                    m_message = "";
                }

            }



            if (isHost)
            {


                if (GUI.Button(MenuUtils.ResizeGUI(new Rect(400, 350, 80, 30)), "Start", skin.button))
                {
                    if (gameMgr.s.client_count > 1)
                    {
                        gameMgr.StartGame();
                        active = false;
                    }
                }
            }

            GUI.EndGroup();

        }

        if(prev_submenu != submenu && prev_submenu == MenuConfig.SubMenuSelected.LOBBY_SELECTED)
            gameMgr.Reset();

        prev_submenu = submenu;

    }



    public void AddMessage(string name, string message)
    {
        m_chat_messages.Add(new MenuConfig.ChatMessage(name, message));
        m_chat_scrollPosition = new Vector2(0, Mathf.Infinity);
    }
}
