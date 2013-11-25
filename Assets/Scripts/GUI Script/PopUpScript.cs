using UnityEngine;
using System.Collections;

public class PopUpScript : MonoBehaviour {

    private bool active;
    public GUISkin skin;
    public string message;
    private GUI.WindowFunction showPopup;

    private Rect WindowRectangle;
	void Awake() {
        active = false;
        message = "empty default message that shouldn't ever be seen";
        WindowRectangle = new Rect(200, 200, 200, 150);
	}
	
    void OnGUI()
    {
        if (!active)
            return;
        GUI.Window(1337, MenuUtils.ResizeGUI(WindowRectangle), ShowPopUp, "Alert");

    }

    public void ShowPopUp(int wid)
    {
        GUI.BringWindowToFront(wid);
        GUI.BeginGroup(MenuUtils.ResizeGUI(new Rect(0, 0,WindowRectangle.width, WindowRectangle.height)));
            GUI.Label(MenuUtils.ResizeGUI(new Rect(0, 0, 10, 20)), message, skin.label);

            if (GUI.Button(MenuUtils.ResizeGUI(new Rect(10, 30, 100, 30)), "Close", skin.button))
                this.active = false;
        GUI.EndGroup();
        GUI.DragWindow();
    }

    public void ShowMessage(string message)
    {
        this.message = message;
        this.active = true;
    }
}
