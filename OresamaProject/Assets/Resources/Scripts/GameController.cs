using UnityEngine;
using System.Collections;

public enum Phase{
	Story,//ストーリー再生中
	Wait,//行動順待機中
	Menu,//行動の選択
    Roulet,//ルーレット回し中
	Move,//移動先の選択
	MenuAtMoveEnd,//移動後の行動の選択
	Battle,//戦闘などの行動中
	//CharaSelect,//行動対象キャラの選択
}

public enum Side{
	Player,//プレイヤー側の手番
	Enemy,//エネミー側の手番
}

public enum WinConditions{
	exteemination,//敵の全滅
	Goal,//特定地点への到達
}

public class GameController : MonoBehaviour {
	public WinConditions win;//勝利条件
	public WinConditions lost;//敗北条件
	public Phase GamePhase;//現在のタイミング
	public Side GameSide;//現在処理中の手番側
	public bool isCharacterSelect;//行動対象のキャラクターを選択中か否か
	public bool isanimation;//アニメーション中か否か
	public GameObject TarnCharacter;//行動中のキャラクター
    public SceneMove sceneMove;

    public static GameController Gcon;

    void Awake() {
        Gcon = this;
        sceneMove.gameObject.SetActive(false);
    }

    /*
	void Start(){
		//StartCoroutine (BattleStart ());
	}
     * */

    //ストーリー終了時に呼び出し
    public void GameStart() {
        StartCoroutine(BattleStart());
        SoundManager.SM.playBGM(SoundManager.BGM.game);

    }

    //戦闘開始時のUI表示
	IEnumerator BattleStart(){
        yield return null;
        if (TarnCharacter != null)
        {
            CameraMoveScript.CameraMove.SetCharacter(TarnCharacter);
        }
		isanimation = true;
        yield return StartCoroutine(UIFadeScript.UIFade.Fade(UIFadeScript.UIFade.WinConditions, 3));
		yield return new WaitForSeconds (0.1f);
		string lostText = "敗北条件：";
		switch (lost) {
		case WinConditions.exteemination:
			lostText += "味方の全滅";
			break;
		case WinConditions.Goal:
			lostText += "防衛網を突破される";
			break;
		}
		yield return StartCoroutine (UIFadeScript.UIFade.Fade (UIFadeScript.UIFade.LosConditions,3));
		yield return new WaitForSeconds (0.1f);
		//yield return StartCoroutine (UIFadeScript.UIFade.TextFade (3, "戦闘開始", Color.black));
		isanimation = false;
        StoryScene.SS.NoneActiveObjectsOperation(true);
	}

	//戦闘終了関数 isWinは勝敗確認用
	Coroutine BEnd = null;
	IEnumerator BattleEnd(){
		if (BEnd != null)
			yield break;
		//string t = "戦闘が終了しました";
        sceneMove.gameObject.SetActive(true);

        GameObject result = null;
        if (GameRoot.GRoot.enemyList.Count <= 0) result = UIFadeScript.UIFade.Win;
        else result = UIFadeScript.UIFade.Los;
        yield return StartCoroutine(UIFadeScript.UIFade.Fade(result, 100000));
	}

	//戦闘終了条件を満たしたか確認用
	bool CheckBattle(){
		bool isEnd = false;

		if (GameRoot.GRoot.playerList.Count <= 0 || GameRoot.GRoot.enemyList.Count <= 0) {
			isEnd = true;
		}

		return isEnd;
	}

	int p = 0;
	int e = 0;
	//デバッグ用に待機時間なしで行動
	void Update(){
        if (GamePhase == Phase.Story) return;
		if (GamePhase == Phase.Wait && CheckBattle () && BEnd == null) {
			BEnd = StartCoroutine (BattleEnd ());
			return;
		}
		if (BEnd != null)
			return;
		if (GamePhase == Phase.Wait) {
			GameObject nChara = GameRoot.GRoot.NextTurn ();
			if (nChara != null)
				SetTarnCharacter (nChara);
			//Debug.Log (c);
		}

	}

	public void SetTarnCharacter(GameObject c){
		if (c == null)
			return;
		switch(c.tag){
		case "Player":
			GameSide = Side.Player;
			break;
		case "Enemy":
			GameSide = Side.Enemy;
			break;
		default:
			GameSide = Side.Player;
			return;
		}
		//Camera.main.gameObject.transform.SetParent (c.transform);
		CameraMoveScript.CameraMove.SetCharacter (c);
		TarnCharacter = c;
		statusWindowScript.SWS.SelectPlayerChenge (c.GetComponent<Character> ());
		TarnStart ();
		if (GameSide == Side.Enemy) {
            StartCoroutine(newAIScript.NAI.AIStart(c.GetComponent<Character>()));
		}
        if (c.GetComponent<Character>() != null)
            Dialog.Dlog.SetText(c.GetComponent<Character>().GetName() + "の行動");
	}

	public void SetGamePhase(Phase p){
		switch (p) {
		case Phase.Story:
			GamePhase = Phase.Story;
			break;
		case Phase.Wait:
			GamePhase = Phase.Wait;//デバッグ用に外し中
			//GamePhase = Phase.Menu;
			break;
		case Phase.Menu:
			GamePhase = Phase.Menu;
			break;
        case Phase.Roulet:
            GamePhase = Phase.Roulet;
            break;
		case Phase.Move:
			GamePhase = Phase.Move;
			break;
		case Phase.MenuAtMoveEnd:
			GamePhase = Phase.MenuAtMoveEnd;
			break;
		case Phase.Battle:
			GamePhase = Phase.Battle;
			break;
		default:
			GamePhase = Phase.Menu;
			break;
		}
	}

	public void TarnStart(){
		SetGamePhase (Phase.Menu);
	}

	public void TarnEnd(){
		SetGamePhase (Phase.Wait);
	}
}
