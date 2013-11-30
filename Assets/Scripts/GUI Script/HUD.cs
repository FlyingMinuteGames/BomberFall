
using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{

    private GameObject m_displayer;
    private GameObject m_timeDisplayer;
    private TimerScript m_timerscript;
    public Texture[] player_textures;
    public Texture[] power_illustrations;
    public TextMesh[] player_names;
    public TextMesh[] player_scores;
    public TextMesh textMeshPrefab;
    public Texture power_up_background;
    public GUISkin skin;

    private Color[] colors = { Color.white, Color.red, new Color(0f, 0.2f, 0.8f), new Color(1f,0.5f,0f)};
    private GameIntel gameIntel;
    private bool active = false;
    private bool hasOffensivePower = false;
    private bool hasDefensivePower = false;
    private Config.PowerType offensivePower;
    private Config.PowerType defensivePower;
    private string offensiveStr;
    private string defensiveStr;

    void Start()
    {
        Debug.Log("In start of HUD");
        m_displayer = transform.FindChild("Displayer").gameObject;
        m_timeDisplayer = m_displayer.transform.FindChild("Timer").gameObject;
        m_timerscript = m_timeDisplayer.GetComponent<TimerScript>();
        colors[3] += Color.red;
        Deactivate();
    }

    public void Init()
    {
        active = true;
        gameIntel = GameMgr.Instance.gameIntel;

        int len = gameIntel.nb_players + gameIntel.nb_cpus;
        player_names = new TextMesh[4];
        player_scores = new TextMesh[4];
        float j = 0;
        for (int i = 0; i < len; i++)
        {
            if (i == 2)
                j += 4f;

            player_names[i] = (TextMesh)Instantiate(textMeshPrefab, new Vector3(textMeshPrefab.transform.position.x + (3f * i) + j, textMeshPrefab.transform.position.y, textMeshPrefab.transform.position.z), Quaternion.identity);
            player_names[i].text = "Player " + (i + 1);
            player_names[i].renderer.material.color = colors[i];

            player_scores[i] = (TextMesh)Instantiate(textMeshPrefab, new Vector3(textMeshPrefab.transform.position.x + (3f * i) + j, textMeshPrefab.transform.position.y - 0.4f, textMeshPrefab.transform.position.z), Quaternion.identity);
            player_scores[i].text = gameIntel.game_mode == Config.GameMode.ARCADE ? "0" : "";


        }

        offensiveStr = "Offensive (" + MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("OffensiveItemKey")) + ")";
        defensiveStr = "Defensive (" + MenuUtils.GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("DefensiveItemKey")) + ")";
        BindOffensivePower(Config.PowerType.BRING_A_SW_TO_A_GF);
        BindDefensivePower(Config.PowerType.IMPENETRABLE_TRINKET);



        if (gameIntel.game_mode == Config.GameMode.ARCADE)
            m_timerscript.Init();
        m_displayer.SetActive(true);
    }

    void Deactivate()
    {
        m_displayer.SetActive(false);
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (!active)
                return;
            GUI.Label(MenuUtils.ResizeGUI(new Rect(700, 380, 50, 50)), offensiveStr, skin.customStyles[1]);
            GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(700, 400, 50, 50), false, true), power_up_background, ScaleMode.StretchToFill);
            if (hasOffensivePower)
                GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(703, 403, 44, 44), false, true), power_illustrations[(int)offensivePower], ScaleMode.ScaleToFit);
            GUI.Label(MenuUtils.ResizeGUI(new Rect(700, 490, 50, 50)), defensiveStr, skin.customStyles[1]);
            GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(700, 510, 50, 50), false, true), power_up_background, ScaleMode.StretchToFill);
            if (hasDefensivePower)
                GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(703, 513, 44, 44), false, true), power_illustrations[(int)defensivePower], ScaleMode.ScaleToFit);
        }
    }

    public void BindOffensivePower(Config.PowerType powertype){
        hasOffensivePower = true;
        offensivePower = powertype;
    }

    public void unBindOffensivePower()
    {
        hasOffensivePower = false;
    }

    public void BindDefensivePower(Config.PowerType powertype)
    {
        hasDefensivePower = true;
        defensivePower = powertype;
    }

    public void unBindDefensivePower()
    {
        hasDefensivePower = false;
    }

}
