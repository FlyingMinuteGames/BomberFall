using UnityEngine;
using System.Collections;

public class ClientGameMgr : MonoBehaviour {

    Client cl;
    PoolSystem<GameObject> bomb;
    void Start()
    {
        cl = new Client("127.0.0.1");
        bomb = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("/prefabs/Bomb"), 200);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
