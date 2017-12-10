using UnityEngine;
using System.Collections;

public class MousePosScript : MonoBehaviour {
	Rigidbody2D rb;
	bool isMousePush = false;
    bool isMenuMask = false;

	void Start(){
		rb = this.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector2 WPos = Input.mousePosition;
		Vector2 pos = Camera.main.ScreenToWorldPoint (WPos);
		rb.MovePosition (pos);

		if (!Input.GetMouseButton (0)) isMousePush = false;
		//transform.position = pos;
	}

    /*
	//マップチップの入力検出
	void OnTriggerEnter2D(Collider2D col){
		if (!Input.GetMouseButton (0)) return;

        ChipTap(col.gameObject);
	}
     * */

	//マップチップの入力検出
	void OnTriggerStay2D(Collider2D col){
		//マウスが押されていなければ終了
		if (!Input.GetMouseButton (0)) return;
		//マウスが押された最初のフレームでなければ終了
		if (isMousePush) return;
        isMousePush = true;
        ChipTap(col.gameObject);
	}

    void ChipTap(GameObject c) {
        //マップチップを選択していたら処理する
        if (c.tag != "MapChip") return;
        if (GameController.Gcon.GameSide != Side.Player) return;
        MapChip chip = c.gameObject.GetComponent<MapChip>();
        chip.ChipTap();
    }
}
