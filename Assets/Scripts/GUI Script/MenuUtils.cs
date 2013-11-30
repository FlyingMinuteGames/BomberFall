/**
 * MenuUtils
 *  --> Collection of generic methods used for the game Menus
 *  
 * Author: Jean-Vincent Lamberti
 * 
 * */

using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class MenuUtils{


    /**
     * KeyCode GetKeyCode(String st)
     *  --> Returns the KeyCode matching the string given in input if between A and Z
     * */
    public static KeyCode GetKeyCode(String st)
    {
        String st2 = st.ToUpper();

        switch (st2)
        {
            case "A": return KeyCode.A;
            case "B": return KeyCode.B;
            case "C": return KeyCode.C;
            case "D": return KeyCode.D;
            case "E": return KeyCode.E;
            case "F": return KeyCode.F;
            case "G": return KeyCode.G;
            case "H": return KeyCode.H;
            case "I": return KeyCode.I;
            case "J": return KeyCode.J;
            case "K": return KeyCode.K;
            case "L": return KeyCode.L;
            case "M": return KeyCode.M;
            case "N": return KeyCode.N;
            case "O": return KeyCode.O;
            case "P": return KeyCode.P;
            case "Q": return KeyCode.Q;
            case "R": return KeyCode.R;
            case "S": return KeyCode.S;
            case "T": return KeyCode.T;
            case "U": return KeyCode.U;
            case "V": return KeyCode.V;
            case "W": return KeyCode.W;
            case "X": return KeyCode.X;
            case "Y": return KeyCode.Y;
            case "Z": return KeyCode.Z;
        }
        return KeyCode.Dollar;
    }
    
     /**
     * String GetStringFromKeycode(KeyCode kc)
     *  --> Returns the String matching the KeyCode given in input if between A and Z
     * */

    public static String GetStringFromKeycode(KeyCode kc)
    {
        switch (kc)
        {
            case KeyCode.A: return "A";
            case KeyCode.B: return "B";
            case KeyCode.C: return "C";
            case KeyCode.D: return "D";
            case KeyCode.E: return "E";
            case KeyCode.F: return "F";
            case KeyCode.G: return "G";
            case KeyCode.H: return "H";
            case KeyCode.I: return "I";
            case KeyCode.J: return "J";
            case KeyCode.K: return "K";
            case KeyCode.L: return "L";
            case KeyCode.M: return "M";
            case KeyCode.N: return "N";
            case KeyCode.O: return "O";
            case KeyCode.P: return "P";
            case KeyCode.Q: return "Q";
            case KeyCode.R: return "R";
            case KeyCode.S: return "S";
            case KeyCode.T: return "T";
            case KeyCode.U: return "U";
            case KeyCode.V: return "V";
            case KeyCode.W: return "W";
            case KeyCode.X: return "X";
            case KeyCode.Y: return "Y";
            case KeyCode.Z: return "Z";
        }
        return "nc";
    }


    /**
     * Rect ResizeGUI(Rect _rect, uniformScale)
     *  --> Returns a scaled version (uniformly or not) of the rectangle given in parameters
     * */
    public static Rect ResizeGUI(Rect _rect, bool uniformScale = false, bool keepSizeProp = false)
    {
        Vector2 scale = new Vector2(Screen.width / 800.0f, Screen.height / 600.0f);
        if (uniformScale)
            scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
        float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = keepSizeProp ? rectWidth : _rect.height * scale.y;
        return new Rect(rectX, rectY, rectWidth, rectHeight);

    }

    /**
     * GUIContent[] SetComboboxRange(int start, int end)
     *  --> Returns a GUIContent array of all values between start and end parameters, used to itinialize purely numerical Combobox
     * */
    public static GUIContent[] SetComboboxRange(int start, int end)
    {
        GUIContent[] gc = new GUIContent[end - start + 1];
        for (int i = start, j = 0; i <= end; i++)
            gc[j++] = new GUIContent("" + i);

        return gc;
    }


    /**
     * bool LoadMapsList()
     *  --> Checks whether or not the user has maps in his Maps folder
     *   - returns true if maps where found
     *   - creates the Maps folder if does not exist
     *   - loads maps list in MenuConfig instance if maps where found
     * */
    public static bool LoadMapsList()
    {
        bool hasMaps = true;
        try
        {
            DirectoryInfo d = new DirectoryInfo(Application.dataPath + "\\Maps");//Assuming Test is your Folder
            MenuConfig.mapFiles = d.GetFiles("*.map"); //Getting Text files
            MenuConfig.maps_string = new String[MenuConfig.mapFiles.Length];

            for (int i = 0, len = MenuConfig.mapFiles.Length; i < len; i++)
            {
                MenuConfig.maps_string[i] = MenuConfig.mapFiles[i].Name;
            }
            if (MenuConfig.mapFiles.Length < 1)
                hasMaps = false;

        }
        catch (Exception)
        {
            hasMaps = false;
            DirectoryInfo d = new DirectoryInfo(Application.dataPath + "\\Maps");
            d.Create();

        }
        return hasMaps;
    }
}
