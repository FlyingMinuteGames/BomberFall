
using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{

    private GameObject m_displayer;
    private GameObject m_timeDisplayer;
    private TimerScript m_timerscript;
    public Texture[] player_textures;
    public TextMesh[] player_names;
    public TextMesh[] player_scores;
    public TextMesh textMeshPrefab;

    private Color[] colors = { Color.white, Color.red, new Color(0f, 0.2f, 0.8f), new Color(1f,0.5f,0f)};
    private GameIntel gameIntel;
    private bool active = false;

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
        //if (Event.current.type == EventType.Repaint)
        //{
        //    if (!active)
        //        return;

        //    float j = 0f;
        //    for (int i = 0, len = gameIntel.nb_players + gameIntel.nb_cpus; i < len; i++)
        //    {
        //        if (i == 2)
        //            j += 0.15f;
        //        Rect tmp = screenRect(0.15f*(i+1)+j, 0.03f, 0.05f*0.6f, 0.06f*0.6f);
        //        Debug.Log("RECT : "+ tmp);
        //        GUI.DrawTexture(tmp, player_textures[i]);


        //        //GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(175 + (80 * i) + j, 10, 50 * 0.4f, 60 * 0.4f)), player_textures[i], ScaleMode.ScaleAndCrop);

        //    }
        //}
    }

    public static Rect screenRect(float tx, float ty, float tw, float th)
    {
        float x1 = tx * Screen.width;
        float y1 = ty * Screen.height;

        float sw = tw * Screen.width;
        float sh = th * Screen.height;

        return new Rect(x1, y1, sw, sh);
    }
}
