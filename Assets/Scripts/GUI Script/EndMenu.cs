﻿using UnityEngine;
using System.Collections;

public class EndMenu : MonoBehaviour {

    public GUISkin skin;
    public Texture logo;
    public Texture background;
    private Config.GameMode gamemode;

    public bool active = false;

    void Awake()
    {
        Screen.showCursor = true;
        Screen.lockCursor = false;
    }

    void OnGUI()
    {
        if (!active)
            return;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill);

        GUI.Label(MenuUtils.ResizeGUI(new Rect(100, 25, 80, 40)), "GAME OVER");
        GUI.DrawTexture(MenuUtils.ResizeGUI(new Rect(30, 50, 600 * 0.38f, 189 * 0.38f)), logo, ScaleMode.ScaleToFit);


        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 280, 100, 30)), "QUIT TO MENU", skin.button))
        {
            GameMgr.Instance.QuitGame();
            active = false;
        }


        if (GUI.Button(MenuUtils.ResizeGUI(new Rect(35, 320, 100, 30)), "QUIT TO DESKTOP", skin.button))
        {
            Application.Quit();
        }

    }

    public void setMode(Config.GameMode gamemode)
    {
        this.gamemode = gamemode;
    }

    public void SwitchState()
    {
        active = !active;
    }
}