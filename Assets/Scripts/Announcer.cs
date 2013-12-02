using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Announce
{
    ANNOUNCE_PLAYER_KILL,
    ANNOUNCE_CHANGE_PHASE,
    ANNOUNCE_CHANGE_NOW,
    ANNOUNCE_PWR_PICK_UP,
    ANNOUNCE_KILL_BY_SW
}

public class Announcer : MonoBehaviour {
    private struct Format
    {
        public Format(Color _color,string _v1,string _v2 = null, string _v3 = null)
        {
            color = _color;
            text = new string[3][];
            text[0] = CompileFormat(_v1);
            text[1] = _v2 != null ? CompileFormat(_v2) : null;
            text[2] = _v3 != null ? CompileFormat(_v3) : null;
        }
        private string[] CompileFormat(string format)
        {
            List<string> l = new List<string>();
            int b = 0;
            string str;
            for(int i = 0, len = format.Length; i < len;)
            {
                
                if(format[i++] != '%')
                    continue;
                if(i >= len)
                    break;
                if(format[i++] != 'v')
                    continue;
                str = format.Substring(b, i - 2 - b);
                l.Add(str);
                b = i;

            }
            str = format.Substring(b);
            l.Add(str);
            return l.ToArray();
        }
        public string[][] text;
        public Color color;
    }
    private static Announcer s_instance = null;
    public static Announcer Instance
    {
        get { return s_instance; }
    }
    private TextMesh m_text;
    private Animation m_anim;
	
    private static Dictionary<Announce,Format> m_announce = new Dictionary<Announce,Format>()
    {
        {Announce.ANNOUNCE_PLAYER_KILL, new Format(Color.red,"Player %v is dead")},
        {Announce.ANNOUNCE_CHANGE_PHASE, new Format(Color.red,"Change phase in %vs")},
        {Announce.ANNOUNCE_CHANGE_NOW, new Format(Color.red,"Change phase NOW !")},
        {Announce.ANNOUNCE_PWR_PICK_UP, new Format(Color.red,"%v")},
        {Announce.ANNOUNCE_KILL_BY_SW, new Format(Color.red,"Player %v has been violently beheaded!")}
    };

	void Start () {
        s_instance = this;
        m_text = GetComponent<TextMesh>();
        m_anim = GetComponent<Animation>();

	}

    public void PlayAnnounce(Announce a,int variant, params string[] values)
    {
        Format announce = m_announce[a];
        m_text.text = FormatAnnounce(announce.text[variant],values);
        m_text.color = announce.color;
        m_anim.Rewind();
        m_anim.Play();
    }

    private string FormatAnnounce(string[] tpl,params string[] values)
    {
        if (tpl == null)
            return "";
        StringBuilder builder = new StringBuilder();
        for (int i, j = i = 0; i < tpl.Length; )
        {
            builder.Append(tpl[i++]);

            if(j >= values.Length)
                break;
            builder.Append(values[j++]);
        }
        return builder.ToString();
    }
}
