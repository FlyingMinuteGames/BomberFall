using UnityEngine;
using System.Collections;

public class FireUp : APowerUp {

    public override void OnPickUp(GameObject powerGo, int clientGuid)
    {
        base.OnPickUp(powerGo, clientGuid);
        OnUse(powerGo, clientGuid);
    }

    public override void OnUse(GameObject powerGo, int clientGuid)
    {
        GameObject obj;

        if ((obj = ObjectMgr.Instance.Get(clientGuid)) != null)
        {
            obj.SendMessage("RecvIncRadius");
            Packet p = PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_PWR_PICK_UP, 0, "FIRE UP");
            GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        }
    }

}
