using UnityEngine;
using System.Collections;

public class Network_TEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Server s = new Server();
        Client c = new Client("127.0.0.1");
        c.SendPacket(PacketBuilder.BuildMovePlayerPacket(0,1|2|4,new Vector3(1,2,3)));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
