using UnityEngine;
using System.Collections;

public class ServerGameMgr : MonoBehaviour {

    Server server;
    PoolSystem<GameObject> bomb;
	void Start () {
        server = new Server();
        bomb = new PoolSystem<GameObject>(ResourcesLoader.LoadResources<GameObject>("/prefabs/Bomb"),200);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
