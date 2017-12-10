using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryScene : MonoBehaviour
{
    [SerializeField]
    private Image imageLeft;

    [SerializeField]
    private Image imageRight;

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text textBox;

    [SerializeField]
    private TextAsset storyCSV;

    private int id = 1;
    private int storyLength;

    private bool isPrinting;

    private string imagePath;

    private string fullText;
    private string textInBox;

    private IEnumerator textPrinter;

    public List<GameObject> NoneActiveObjects;//ストーリー再生中は停止するオブジェクト

    public static StoryScene SS;
    void Awake() {
        SS = this;
    }

    // Use this for initialization
    void Start ()
    {
        //画像フォルダの階層宣言
        imagePath = "Sprite/characters_halfSize/";

        //ストーリーファイルの読み込み
        storyLength = CSVReader.GetLength(storyCSV, "$");
        ReadStoryCSV(id);
        GameController.Gcon.isanimation = true;
        NoneActiveObjectsOperation(false);

        SoundManager.SM.playBGM(SoundManager.BGM.story);
    }

    //引数v番目の行を読み込み・出力
    void ReadStoryCSV(int v)
    {
        // 0番号
        // 1キャラ名（表示の名前）
        // 2キャラ場左
        // 3キャラ場右
        // 4文
        // 5移動

        string[] storyLine = CSVReader.ReadLine(storyCSV, v, "$");

        for (int i = 0; i < storyLine.Length; i++) {
            storyLine[i] = storyLine[i].Replace("\"", "");
        }

        nameText.text = storyLine[1];
        string imageText = storyLine[2];

        if (storyLine[2].Length > 0)
        {
            if (imageText.IndexOf("_half") > 0) {
                
            }
            imageText = imageText.Replace("_half", "");
            imageLeft.gameObject.SetActive(true);
            imageLeft.sprite = Resources.Load<Sprite>(imagePath + imageText);
        }
        else {
            imageLeft.gameObject.SetActive(false);
            imageLeft.sprite = Resources.Load<Sprite>(imagePath + "none"); // change this sprite to something transperant
        }

        imageText = storyLine[3];
        if (storyLine[3].Length > 0)
        {
            imageText = imageText.Replace("_half", "");
            imageRight.gameObject.SetActive(true);
            imageRight.sprite = Resources.Load<Sprite>(imagePath + imageText);
        }
        else {
            imageRight.gameObject.SetActive(false);
            imageRight.sprite = Resources.Load<Sprite>(imagePath + "none"); // change this sprite to something transperant
        }

        fullText = storyLine[4];
        textPrinter = PrintText(fullText);
        StartCoroutine(textPrinter);

        if (storyLine[5] == null || storyLine[5] == "") { return; }
        //マップがないなら移動なし
        //if (MapCreateScript.mapChips == null) return;

        //移動の書式
        //（例）盗賊1|18:1|盗賊2|19:3|盗賊3|20:2|お嬢さん|18:2
        //｜で分割してスタック
        //スタックした文字列を解釈
        //1.「：」が含まれない
        //　その名前のキャラクターを検索し、そのキャラクターを操作対象とする
        //2.「：」が含まれる
        //　「：」の両端を「ｘ：ｙ」とし、操作中のキャラの移動パスに追加する
        //
        //最後にすべてのキャラを移動させる

        string[] move = storyLine[5].Split("|"[0]);
        Character target = null;
        List<Vector2> paths = new List<Vector2>();
        foreach (string s in move) {
            if (s == "") continue;
            if (s.IndexOf(":") < 0)
            {
                MoveEnter(target, paths);
                paths = new List<Vector2>();
                target = CharacterIDFind(int.Parse(s));
            }
            else
            {
                string[] pos = s.Split(":"[0]);
                Vector2 p = new Vector2(int.Parse(pos[0]), int.Parse(pos[1]));
                paths.Add(p);
            }
        }
        MoveEnter(target, paths);
    }

    Character CharacterIDFind(int id) {
        Character target = null;

        int Length = GameRoot.GRoot.enemyList.Count;
        for (int i = 0; i < Length; i++) {
            Character c = GameRoot.GRoot.enemyList[i].GetComponent<Character>();
            if (id == c.GetID()) {
                target = c;
                break;
            }
        }
        if (target != null)
        {
            //Debug.Log(name + ":" + target);
            return target;
        }

        Length = GameRoot.GRoot.playerList.Count;
        for (int i = 0; i < Length; i++)
        {
            Character c = GameRoot.GRoot.playerList[i].GetComponent<Character>();
            if (id == c.GetID())
            {
                target = c;
                break;
            }
        }
        //Debug.Log(name + ":" + target);
        return target;
    }

    void MoveEnter(Character target,List<Vector2> path) {
        if (target == null || path.Count == 0) return;
        //Debug.Log(target + "認識");

        int[,] moveTo = new int[path.Count, 2];

        for (int i = 0; i < path.Count; i++) {
            moveTo[i, 0] = (int)path[i].x;
            moveTo[i, 1] = (int)path[i].y;
        }

        CharacterMove CMove = target.GetComponent<CharacterMove>();
        //StartCoroutine(CMove.PathMove(moveTo));
        delaydChar.Enqueue(CMove);
        delaydPath.Enqueue(moveTo);
        if(!isRunning){
            StartCoroutine(DelaydMove());
        }
    }

    Queue<int[,]> delaydPath = new Queue<int[,]>();
    [SerializeField]
    Queue<CharacterMove> delaydChar = new Queue<CharacterMove>();

    public bool isMoving = false;
    public bool isRunning = false;
    IEnumerator DelaydMove() {
        isRunning = true;
        while (delaydChar.Count > 0)
        {
            if (isMoving)
            {
                if (!delaydChar.Peek().GetIsMoving())
                {
                    delaydChar.Dequeue();
                    delaydPath.Dequeue();
                    isMoving = false;
                }
            }
            else {
                StartCoroutine(delaydChar.Peek().PathMove(delaydPath.Peek()));
                CameraMoveScript.CameraMove.SetCharacter(delaydChar.Peek().gameObject);
                isMoving = true;
            }
            yield return null;
        }
        delaydPath = new Queue<int[,]>();
        delaydChar = new Queue<CharacterMove>();
        isRunning = false;
    }

    void MoveEnd() {
        if (delaydChar.Count > 0) {
            delaydChar.Peek().MoveEnd();
            //Debug.Log("end to" + delaydChar.Peek());
        }
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0) || isSkip)
        {
            if (isRunning) {
                MoveEnd();
            }
            else if (isPrinting)
            {
                isPrinting = false;
                textBox.text = fullText;
                StopCoroutine(textPrinter);
            }
            else
            {

                id++;
                if (id <= storyLength)
                    ReadStoryCSV(id);
                else {
                    //ストーリー再生終了
                    //NoneActiveObjectsOperation(true);
                    GameController.Gcon.isanimation = false;
                    GameController.Gcon.GameStart();
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator PrintText(string text)
    {
        isPrinting = true;
         
        int i = 0;
        textInBox = "";
        while (i < text.Length)
        {

        textBox.text = textInBox; //storyLine[4];
            textInBox += text[i++];
            yield return new WaitForSeconds(0.07f);
        }

        textBox.text = fullText;

        isPrinting = false;
    }

    bool isSkip = false;
    public void StorySkip() {
        isSkip = true;
    }

    public void NoneActiveObjectsOperation(bool isActive) {
        foreach (GameObject obj in NoneActiveObjects) {
            if (obj != null) {
                obj.SetActive(isActive);
            }
        }
    }
}
