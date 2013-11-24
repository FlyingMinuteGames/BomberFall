using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{

    private GameObject m_displayer;
    private GameObject m_scoreDisplayer;
    private GameObject m_timeDisplayer;
    private TimerScript m_timerscript;
    private ScoreScript m_scoreScript;

    void Start()
    {
        m_displayer = transform.FindChild("Displayer").gameObject;
        m_scoreDisplayer = m_displayer.transform.FindChild("Score").gameObject;
        m_timeDisplayer = m_displayer.transform.FindChild("Timer").gameObject;
        m_timerscript = m_timeDisplayer.GetComponent<TimerScript>();
        m_scoreScript = m_scoreDisplayer.GetComponent<ScoreScript>();

        Deactivate();
    }

    public void Init()
    {
        Debug.Log("Init HUD");
        setHudToTimer();
    }

    void Deactivate()
    {
        m_displayer.SetActive(false);
    }

    void setHudToScore()
    {
        m_timeDisplayer.SetActive(false);
        m_scoreDisplayer.SetActive(true);
    }

    void setHudToTimer()
    {
        m_scoreDisplayer.SetActive(false);
        m_timeDisplayer.SetActive(true);
    }



}
