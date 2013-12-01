using UnityEngine;
using System.Collections;

public class BombUp : APowerUp {

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
            obj.SendMessage("RecvIncBombCount");
            //GameMgr.Instance.PlayAnnounce(Announce.ANNOUNCE_PWR_PICK_UP, 0, "BOMB UP");
            Packet p = PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_PWR_PICK_UP,0, "BOMB UP");
            GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        }
    }

}
