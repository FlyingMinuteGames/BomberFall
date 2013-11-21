using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour {

    public enum PowerType
    {
        AS_NEIL_TM,
        BACK_TO_SCHOOL,
        BOMB_SQUAD,
        BRING_A_SW_TO_A_GF,
        FIRE_UP,
        IMPENETRABLE_TRINKET,
        KICK_IT_LIKE_U_MEAN_IT,
        RANDOM_TELEPORTER,
        SPEED_UP,
        THE_HOME_RUNNER,
        VENGEFUL_PHENIX
    }

    public PowerType type;
    public Texture illustration;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
