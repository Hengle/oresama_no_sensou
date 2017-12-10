using UnityEngine;
using System.Collections;

public class GameControllerTest : MonoBehaviour {
	public Phase NowPhase;
	public Side NowSide;

	void Awake () {
        GameController.Gcon.GamePhase = NowPhase;
        GameController.Gcon.GameSide = NowSide;
	}

	void Update(){
        NowPhase = GameController.Gcon.GamePhase;
        NowSide = GameController.Gcon.GameSide;
	}
}
