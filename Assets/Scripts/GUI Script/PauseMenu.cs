using UnityEngine;
using System.Collections;
using System;

public class PauseMenu : MonoBehaviour {


    public GUISkin skin;
    public Texture logo;
    public Texture background;
    private  MenuConfig.MainMenuSelected menu;
    private MenuConfig.SubMenuSelected submenu;
    private int m_ratio = 0;
    private int m_resolution = 0;
    private int quality = 0;
    private float m_fov = 90.0f;
    private float m_music_volume = 10.0f;
    private float m_sound_effects_volume = 10.0f;
    private bool m_fullscreen = false;
    private int m_antialiasing;
    private int m_vsync;
    private HUD hud;
   
    private Vector2 m_keybindings_scrollPosition = Vector2.zero;

    private GUIContent[] ratio_combobox;

    private GUIContent[] _4_3_combobox;
    private GUIContent[] _16_10_combobox;
    private GUIContent[] _16_9_combobox;



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


    public bool active = false;

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
        if (st.Equals("video") || st.Equals(""))
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
                switch (m_antialiasing)
                {
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
        PlayerPrefs.Save();
    }

    void Awake()
    {
        Screen.showCursor = true;
        Screen.lockCursor = false;
        
        MenuConfig.m_keybindings = new String[MenuConfig.m_keybindings_labels.Length];

        ratio_combobox = new GUIContent[3];
        _4_3_combobox = new GUIContent[MenuConfig.resolution_4_3.Length];
        _16_10_combobox = new GUIContent[MenuConfig.resolution_16_10.Length];
        _16_9_combobox = new GUIContent[MenuConfig.resolution_16_9.Length];
        m_quality = new GUIContent[MenuConfig.quality_string.Length];
        m_aa = new GUIContent[MenuConfig.aa_string.Length];
        m_vsyncContent = new GUIContent[MenuConfig.vsync_string.Length];

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

        LoadFromPlayerPrefs("keybindings");
        LoadFromPlayerPrefs("video");

        comboBoxControl.SetSelectedItemIndex(m_ratio);
        comboBoxQuality.SetSelectedItemIndex(quality);
        comboBoxResolution.SetSelectedItemIndex(m_resolution);
        comboboxAA.SetSelectedItemIndex(m_antialiasing);
        comboboxVsync.SetSelectedItemIndex(m_vsync);

         hud = GameObject.Find("HUD").GetComponent<HUD>();


    }

    void OnGUI()
    {
        if (!active)
            return;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);

        GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(30, 50, 600 * 0.38f, 189 * 0.38f)), logo, ScaleMode.ScaleToFit);

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 200, 100, 30)), "RESUME", skin.button))
        {
            active = false;
        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 240, 100, 30)), "OPTIONS", skin.button))
        {

            if (menu != MenuConfig.MainMenuSelected.OPTION_SELECTED)
                menu = MenuConfig.MainMenuSelected.OPTION_SELECTED;
            else
            {
                menu = MenuConfig.MainMenuSelected.NO_SELECTED;
                submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 280, 100, 30)), "QUIT TO MENU", skin.button))
        {
            GameMgr.Instance.QuitGame();
            active = false;
        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 320, 100, 30)), "QUIT TO DESKTOP", skin.button))
        {
            Application.Quit();
        }



        if (menu == MenuConfig.MainMenuSelected.OPTION_SELECTED)
        {

            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(150, 240, 100, 30)), "PRESENTATION", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.PRESENTATION_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.PRESENTATION_SELECTED;
                else submenu = MenuConfig.SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(150, 280, 100, 30)), "CONTROLS", skin.button))
            {
                if (submenu != MenuConfig.SubMenuSelected.CONTROLS_SELECTED)
                    submenu = MenuConfig.SubMenuSelected.CONTROLS_SELECTED;
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

    }

    public void SwitchState()
    {
        active = !active;
        if (active)
        {
            LoadFromPlayerPrefs();
            hud.Deactivate();
        }
        else
            hud.Activate();

    }

    public void Update()
    {
        if (!GameMgr.Instance.game_started)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.SwitchState();
        }
    
    }

}
