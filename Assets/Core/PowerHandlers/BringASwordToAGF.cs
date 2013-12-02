using UnityEngine;
using System.Collections;

public class BringASwordToAGF : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {

        System.Net.Sockets.TcpClient client = GameMgr.Instance.s.GetTcpClient(clientGuid);
        GameMgr.Instance.s.SendPacketTo(client, PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_PWR_PICK_UP, 0, "BRING A SWORD TO A BOMBFIGHT"));
        Packet p = PacketBuilder.BuildBindOffensiveItem(clientGuid, Config.PowerType.BRING_A_SW_TO_A_GF);
        GameMgr.Instance.s.SendPacketTo(client, p);
        GameObject go = ObjectMgr.Instance.Get(clientGuid);
        BomberController bc = go.GetComponent<BomberController>();
        bc.hasOffensiveItem = (int)Config.PowerType.BRING_A_SW_TO_A_GF;


        base.OnPickUp(powerGo, clientGuid);
    }

    public override void OnUse(GameObject powerGo, int clientGuid)
    {
    }

}
