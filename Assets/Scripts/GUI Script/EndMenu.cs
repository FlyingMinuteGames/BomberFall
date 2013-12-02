using UnityEngine;
using System.Collections;

public class EndMenu : MonoBehaviour
{

    public GUISkin skin;
    public Texture logo;
    public Texture background;
    private Config.GameMode gamemode;

    public bool m_active = false;
    private int[] scores;

    void Awake()
    {
        Screen.showCursor = true;
        Screen.lockCursor = false;
    }

    void OnGUI()
    {
        if (!m_active)
            return;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);

        GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 130, 600 * 0.50f, 40)), "GAME OVER", skin.customStyles[2]);
        GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(250, 25, 600 * 0.50f, 189 * 0.50f)), logo, ScaleMode.ScaleToFit);

        int max = scores[0], maxindex = 0;
        if (gamemode == Config.GameMode.ARCADE)
        {
            string scoreString = "";
            for (int i = 0; i < scores.Length; i++)
            {
                maxindex = max < scores[i] ? i : maxindex;
                max = scores[maxindex];
                scoreString += "PLAYER " + (i + 1) + " : " + scores[i] + "     ";

            }
            GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 180, 600 * 0.50f, 40)), scoreString, skin.customStyles[3]);

            int count = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] == scores[maxindex])
                    count++;
                if (count > 1)
                    break;
            }
            if (count > 1)
                GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 250, 600 * 0.50f, 40)), "WOW THIS IS A DRAW !", skin.customStyles[3]);
            else
                GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 250, 600 * 0.50f, 40)), "CONGRATS TO THE PLAYER " + (maxindex + 1) + " YOU JUST WON THE GAME !", skin.customStyles[3]);

        }
        else
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > 0)
                {
                    maxindex = i;
                    break;
                }

            }

            GUI.Label(MenuUtils.ResizeGUI(new Rect(250, 250, 600 * 0.50f, 40)), "CONGRATS TO THE PLAYER " + (maxindex + 1) + " YOU JUST WON THE GAME !", skin.customStyles[3]);
        }

        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(300, 300, 100, 80)), "QUIT TO MENU", skin.button))
        {
            GameMgr.Instance.QuitGame();
            m_active = false;
        }


        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(400, 300, 100, 80)), "QUIT TO DESKTOP", skin.button))
        {
            Application.Quit();
        }

    }

    public void setMode(Config.GameMode gamemode, int[] scores)
    {
        this.gamemode = gamemode;
        this.scores = scores;
    }

    public void SwitchState()
    {
        m_active = !m_active;
    }
}
