using UnityEngine;
using System.Collections;

public class SpeedUp : APowerUp {

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
            obj.SendMessage("RecvIncSpeedMult");
            //GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), PacketBuilder.BuildSpeedUpPacket(clientGuid, obj.GetComponent<BomberController>().GetSpeedMult()));
            
            GameMgr.Instance.s.SendPacketBroadCast(PacketBuilder.BuildSpeedUpPacket(clientGuid, obj.GetComponent<BomberController>().GetSpeedMult()));

            //GameMgr.Instance.PlayAnnounce(Announce.ANNOUNCE_PWR_PICK_UP, 0, "BOMB UP");
            Packet p = PacketBuilder.BuildPlayAnnouncePacket(Announce.ANNOUNCE_PWR_PICK_UP, 0, "SPEED UP");
            GameMgr.Instance.s.SendPacketTo(GameMgr.Instance.s.GetTcpClient(clientGuid), p);
        }

    }

}
