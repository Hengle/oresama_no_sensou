using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMoveScript : MonoBehaviour
{
    public GameObject target;//追従対象のオブジェクト
    public float moveSpeed;
    public Vector3 moveDistance;//追従対象との相対距離

    //カメラの移動範囲
    public Vector2 MoveRangeMin = Vector2.zero;
    public Vector2 MoveRangeMax = Vector2.zero;

    public static CameraMoveScript CameraMove;

    void Awake()
    {
        CameraMove = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
        MoveRangeInit();
        subCameraInit();
    }

    public void SetCharacter(GameObject obj)
    {
		target = obj;
        moveDistance = Vector3.zero;
    }

    //カメラの端点
    public Vector3 cameraTopRight;
    public Vector3 cameraTopLeft;
    public Vector3 cameraBottomLeft;
    public Vector3 cameraBottomRight;
    public float cameraRangeWidth;
    public float cameraRangeHeight;
    private float distance;//プレイヤーまでの距離
    //移動範囲の初期化
    private void MoveRangeInit()
    {
        //マップの範囲を取得
        Vector2 range = new Vector2(MapCreateScript.MCS.mapChipSize.x * (MapCreateScript.mapChips.GetLength(0)), MapCreateScript.MCS.mapChipSize.y * (MapCreateScript.mapChips.GetLength(1)));
        if (range.x < 0)
        {
            MoveRangeMin.x = range.x + MapCreateScript.MCS.mapChipSize.x;
            //MoveRangeMax.x = MapCreateScript.MCS.mapChipSize.x;
        }
        else {
            MoveRangeMax.x = range.x - MapCreateScript.MCS.mapChipSize.x / 2;
            //MoveRangeMin.x = -MapCreateScript.MCS.mapChipSize.x / 2;
        }

        if (range.y < 0)
        {
            MoveRangeMin.y = range.y - MapCreateScript.MCS.mapChipSize.y;
            //MoveRangeMax.y = -MapCreateScript.MCS.mapChipSize.y;
        }
        else {
            MoveRangeMax.y = range.y + MapCreateScript.MCS.mapChipSize.y; ;
            //MoveRangeMin.y = MapCreateScript.MCS.mapChipSize.y;
        }

        distance = transform.position.z;//ｚ値を取得
        cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));//左下
        cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));//右上
        cameraTopLeft = new Vector3(cameraBottomLeft.x, cameraTopRight.y, cameraBottomLeft.z);//左上
        cameraBottomRight = new Vector3(cameraTopRight.x, cameraBottomLeft.y, cameraBottomLeft.z);//右下
        cameraRangeWidth = Vector3.Distance(cameraBottomLeft, cameraBottomRight);//横幅
        cameraRangeHeight = Vector3.Distance(cameraTopLeft, cameraBottomLeft);//高さ
    }

    // Update is called once per frame
    void Update()
    {
        moveDistance = Vector3.zero;

        KeyMove();

        MoveRangeInit();

        //SubCameraTouch();
        MainCameraTouch();

        Move();
    }

    void MainCameraTouch() {
        TouchMove();
        main = true;
       // Debug.Log("asdasdasd");
    }

    void SubCameraTouch()
    {
        SubTouchMove();
        main = false;
    }

    private bool main;
    public bool subCamTouched;
    private Phase prePhase;
    public Camera mainCamera;
    public Camera subCamera;
    float subCameraSize;
	private bool ButtonD;
	private Vector2 EndPos;
	private Vector2 StartPos;
	private Vector3 preMousePos;
	private Rect mapRect;
	private Vector3 preTargetPos = Vector3.zero;

    public Vector3 newPos;

	public float diff;//誤差の数値(これ以下の距離の移動入力を無視)
	//入力が移動しても良い程度の数値か確認,誤差の範囲難ならfalse
	public bool isDifferance(float f){
		if (f > diff || f < -diff) {
			return true;
		}
		return false;
	}

    //キー入力確認
    void KeyMove() {
        //カメラの位置を操作
        if (Input.GetKey(KeyCode.LeftArrow))
            moveDistance.x -= moveSpeed;
        if (Input.GetKey(KeyCode.RightArrow))
            moveDistance.x += moveSpeed;
        if (Input.GetKey(KeyCode.UpArrow))
            moveDistance.y += moveSpeed;
        if (Input.GetKey(KeyCode.DownArrow))
            moveDistance.y -= moveSpeed;
    }

    //タッチ入力確認
    void TouchMove() {

        if (GameController.Gcon.isanimation == true)
            return;

        if (subCamera.pixelRect.Contains(Input.mousePosition))
        {
            if (Input.GetMouseButton(0))
            {
                subCamTouched = true;
                SubCameraTouch();
                moveDistance = Vector3.zero;
            } else
                subCamTouched = false;

            return;
        }

        subCamTouched = false;

        if (ButtonD)
        {
            Vector3 mousePos = Input.mousePosition;

            if (mousePos != preMousePos)
            {
                EndPos = mainCamera.ScreenToWorldPoint(mousePos);

                //Debug.Log(StartPos + " : " + EndPos);

                Vector2 dis = StartPos - EndPos;
                if (preMousePos != Input.mousePosition && (isDifferance(dis.x) || isDifferance(dis.y)))
                {
                    moveDistance = StartPos - EndPos;
                }
            }

            //StartPos = new Vector2 (0.0f, 0.0f);
            EndPos = new Vector2(0.0f, 0.0f);
            //ButtonD = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            preMousePos = Input.mousePosition;
            ButtonD = true;

        }

        if (!Input.GetMouseButton(0))
        {
            StartPos = new Vector2(0.0f, 0.0f);
            ButtonD = false;
        }
    }


    void SubTouchMove()
    {
        /*
        if (ButtonD)
        {
            
            Vector3 mousePos = Input.mousePosition;
            if (mousePos != preMousePos)
            {
                EndPos = subCamera.ScreenToWorldPoint(mousePos);

                //Debug.Log(StartPos + " : " + EndPos);

                Vector2 dis = StartPos - EndPos;
                if (preMousePos != Input.mousePosition && (isDifferance(dis.x) || isDifferance(dis.y)))
                {
                    moveDistance = (StartPos - EndPos);
                }
            }

            //StartPos = new Vector2 (0.0f, 0.0f);
            EndPos = new Vector2(0.0f, 0.0f);
            //ButtonD = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
           // Debug.Log("dong");
            StartPos = subCamera.ScreenToWorldPoint(Input.mousePosition);
            preMousePos = Input.mousePosition;
            ButtonD = true;
        }
        else if (!Input.GetMouseButton(0))
        {
            Debug.Log("hello");
            StartPos = new Vector2(0.0f, 0.0f);
            ButtonD = false;
        }
    */
    }

    void Move(){
        //移動目標地点
        if (main)
            newPos = transform.position + moveDistance;
        else { 
            newPos = moveDistance; } //transform.position - moveDistance;
        //追尾対象が動いたら、追尾対象にカメラを戻す
        if (target != null && target.transform.position != preTargetPos)
        {
            preTargetPos = target.transform.position;
            newPos = preTargetPos;
            newPos.z = transform.position.z;
        }  

        if (subCamTouched)
            newPos = subCamera.ScreenToWorldPoint(Input.mousePosition);

        float newX = Mathf.Clamp(newPos.x, MoveRangeMin.x + cameraRangeWidth / 2, MoveRangeMax.x - cameraRangeWidth / 2);
        float newY = Mathf.Clamp(newPos.y, MoveRangeMin.y + cameraRangeHeight / 2, MoveRangeMax.y - cameraRangeHeight / 2); ;

        Vector3 limitPos = new Vector3(newX, newY, distance);//実際の移動地点
        transform.position = limitPos;
   
    }

    void subCameraInit() {
        subCamera = GameObject.FindWithTag("SubCamera").GetComponent<Camera>();
        //Rect r = subCamera.GetComponent<Rect>();
        subCameraSize = subCamera.orthographicSize;
    }
}