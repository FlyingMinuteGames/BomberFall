using UnityEngine;
using System.Collections;

public class Guid : MonoBehaviour {

    private int guid = -1;
    private bool setted = false;
    
    public int SetGUID(int guid){
        this.guid = guid;
        setted = true;
        return guid;
    }

    public int GetGUID()
    {
        return this.guid;
    }
}
