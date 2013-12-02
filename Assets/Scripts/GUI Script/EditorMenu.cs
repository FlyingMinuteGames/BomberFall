using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class EditorMenu : MonoBehaviour {
    
	// Use this for initialization

    private string grid_x="5", grid_y="5",filename="test_map";
    private int x, y;
    private Maps maps = null;
    public Material grid_mat;
    public Material ground_mat;
    public Material cursor_mat;
    public Transform stone;
    public Transform breakable;
    public Transform cursor_tpl;
    public static float maxZoom = 6.0f;
    private int max = 15;
    private int min = 8;


    Plane plane = new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
    GameObject cursor;
    int selGridInt;
    int parseInt(string input)
    {
        int size;
        // best than int.Parse() !
        if(input == null || (size = input.Length) == 0)
            return 0;
        int i = 0,value = 0,s=1;
        while (char.IsWhiteSpace(input[i])) i++; // escape space !
        if(input[i] == '-')
        {
            s = -s;
        }
        while(i < size && input[i] != 0)
        {
            if(input[i] > '9' || input[i] < '0')
                break;
            value = value * 10 + input[i] - '0';
            i++;
            
        }
        return (s * value) > max ? max : ((s * value) < min ? min : (s*value));
    }
	void Start () {

        //tmp hack
        Maps.s_material = ground_mat;
        Maps.s_grid = grid_mat;
        Maps.s_stone = stone;
        Maps.s_breakable = breakable;
        cursor = ((Transform)Instantiate(cursor_tpl)).gameObject;
        DirectoryInfo d = new DirectoryInfo(Application.dataPath + "\\Maps");
        d.Create();

        cursor.transform.position.Set(0,0.52f,0);

	}
	
	// Update is called once per frame
	void Update () {
        float wheel = 0;
        if ((wheel = Input.GetAxis("Mouse ScrollWheel")) < 0)
        {
            Camera.main.transform.position += new Vector3(0, -wheel * 2, 0);
        }
        else if ((wheel = Input.GetAxis("Mouse ScrollWheel")) > 0)
        {

            Camera.main.transform.position += new Vector3(0, -wheel * 2, 0);
            if (Camera.main.transform.position.y < maxZoom)
                Camera.main.transform.position += new Vector3(0, 6 - Camera.main.transform.position.y, 0);
        }
        float hit;

        Ray r;
        if (maps != null)
            if (plane.Raycast(r = Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                
                Vector3 pos = r.GetPoint(hit);
                Vector3 o = new Vector3(pos.x, pos.y, pos.z);
                pos.y = 0.52f;
                bool test;
                pos = maps.NormalizePosition(pos,out test);
                if(!test)
                    return;
                cursor.transform.position = pos;
                //Debug.Log(cursor.transform.position + " " + o);

                if (Input.GetMouseButton(0))
                {
                    
                    IntVector2 tpos = maps.GetTilePosition(new Vector2(pos.x, pos.z));
                    if (tpos != null)
                        maps.AddBlock((MapsTiles)selGridInt, tpos);
                    
                }
                else if (Input.GetMouseButton(1))
                {
                    IntVector2 tpos = maps.GetTilePosition(new Vector2(pos.x, pos.z));
                    if (tpos != null)
                        maps.AddBlock((MapsTiles)0, tpos);
                    
                }

            }
	}

    void addblock(IntVector2 curtile)
    { 

    }
    void OnGUI()
    {
        
        GUI.Box(GuiUtils.ResizeGUI(new Rect(10, 50, 150, 500),true), "");
        GUI.Label(GuiUtils.ResizeGUI(new Rect(15, 60, 80, 30), true), "Grid size :");

        Debug.Log(GUI.GetNameOfFocusedControl());
        GUI.SetNextControlName("x");
        if (GUI.GetNameOfFocusedControl() == "x")
            grid_x = GUI.TextField(GuiUtils.ResizeGUI(new Rect(15, 85, 45, 25), true), grid_x);
        else grid_x = (x = parseInt(GUI.TextField(GuiUtils.ResizeGUI(new Rect(15, 85, 45, 25), true), grid_x))).ToString();
        GUI.SetNextControlName("y");
        if (GUI.GetNameOfFocusedControl() == "y")
            grid_y = GUI.TextField(GuiUtils.ResizeGUI(new Rect(62, 85, 45, 25), true), grid_y);
        else grid_y = (y = parseInt(GUI.TextField(GuiUtils.ResizeGUI(new Rect(62, 85, 45, 25), true), grid_y))).ToString();
        //grid_y = (y = parseInt(GUI.TextField(GuiUtils.ResizeGUI(new Rect(62, 85, 45, 25), true), grid_y))).ToString();
        
        selGridInt = GUI.SelectionGrid(GuiUtils.ResizeGUI(new Rect(30, 150, 100, 100), true), selGridInt, new string[] { "Empty tiles", "Solid block", "Breakable block" }, 1);
        if (GUI.Button(GuiUtils.ResizeGUI(new Rect(15, 120, 140, 25), true), "Generate maps"))
        {

            if (maps == null)
                maps = new Maps(new IntVector2(x, y));
            else
            {
                maps.Clear();
                maps.Size = new IntVector2(x, y);
            }
            maps.Generate();
        
        }
        if (GUI.Button(GuiUtils.ResizeGUI(new Rect(15, 260, 140, 25), true), "Fill"))
        {

            maps.Fill((MapsTiles)selGridInt);

        }

        if (GUI.Button(GuiUtils.ResizeGUI(new Rect(15, 290, 140, 25), true), "Save to"))
        {
            if(maps != null)
                maps.SaveToFile(filename+".map");
        }

        if (GUI.Button(GuiUtils.ResizeGUI(new Rect(15, 320, 140, 25), true), "Load from"))
        {
            maps = Maps.LoadMapsFromFile(filename+".map");
            //maps.Clear();
            //maps.Generate();
        }

        filename = GUI.TextField(GuiUtils.ResizeGUI(new Rect(15, 350, 140, 25), true), filename);
        if (GUI.Button(GuiUtils.ResizeGUI(new Rect(15, 380, 140, 25), true), "Quit"))
        {
            Application.Quit();
        }



    }
}
