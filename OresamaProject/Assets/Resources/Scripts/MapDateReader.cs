using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

public class Layer2D
{
	public int width; // 幅
	public int height; // 高さ
	public int[,] _vals = null; // マップデータ
	public bool[,] walk = null; // 背景扱いか否か

	// 作成
	public void Create(int width, int height) {
		if (_vals != null)
			return;
		this.width = width;
		this.height = height;
		_vals = new int[width, height];
		walk = new bool[width, height];
		//初期値を-1000にする（空の状態）
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				_vals [i, j] = -1000;
			}
		}
	}

	// 値の取得
	// @param x X座標
	// @param y Y座標
	// @return 指定の座標の値 (領域外を指定したら-1)
	public int Get(int x, int y) {
		if(x < 0 || x >= width) { return -1; }
		if(y < 0 || y >= height) { return -1; }
		return _vals [x, y];
	}

	// 値の設定
	// @param x X座標
	// @param y Y座標
	// @param val 設定する値
	public void Set(int x, int y, int val) {
		if (val < 0) return;
		if(x < 0 || x >= width) { return; }
		if(y < 0 || y >= height) { return; }
		_vals[x, y] = val;
	}

	// デバッグ出力
	public void Dump() {
		//print ("[Layer2D] (w,h)=("+width+","+height+")");
		for(int y = 0; y < height; y++) {
			string s = "";
			for(int x = 0; x < width; x++) {
				s += Get (x, y) + ",";
			}
			//print (s);
		}
	}

}

public class MapDateReader : MonoBehaviour {
	
	/// <summary>
	/// マップのデータ読み込み
	/// </summary>
	/// <returns>[読み込む情報の種類,x軸,y軸]
	/// 読み込む情報の種類　1 歩行可能領域,2 背景領域,3 イベント番号,4 オブジェクト番号
	/// </returns>
	/// <param name="path">Path.</param>
	public static Layer2D[] AllRead(TextAsset path){
		Layer2D maplayer = new Layer2D ();
		Layer2D objectsLayer = new Layer2D ();
        Layer2D eventLayer = new Layer2D();


		//レイヤーの設定
		//Layer2D layer = new Layer2D();
		//データのload
		TextAsset mapDate = path/*Resources.Load (path)as TextAsset*/;
		List<int> firstGid = new List<int> ();

		//Xmlの解析開始
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (mapDate.text);
		XmlNodeList mapList = xmlDoc.GetElementsByTagName ("map");
		foreach (XmlNode map in mapList) {
			XmlNodeList childList = map.ChildNodes;
			foreach(XmlNode child in childList) {
				// マップ属性を取得
				XmlAttributeCollection attrs = child.Attributes;

				if (child.Name == "tileset") {
					firstGid.Add (int.Parse (attrs.GetNamedItem ("firstgid").Value));
					//Debug.Log (firstGid [firstGid.Count - 1]);
				}

				if(child.Name != "layer") { continue; } // layerノード以外は見ない

				//Debug.Log (child.Attributes.GetNamedItem("name").Value);


				string lName = child.Attributes.GetNamedItem ("name").Value;
				bool back = false;
                //bool _event = false;
				//背景レイヤーの場合
                if (lName == "back")
                    back = true;

				Layer2D l;
				int first = 0;

				if (lName == "objects") {
					l = objectsLayer;
					first = firstGid [1];
                }
                else if (lName == "event"){
                    //_event = true;
                    Debug.Log("EventLayer");
                    l = eventLayer;
                    first = firstGid[2];
                }
                else {
					l = maplayer;
					first = firstGid [0];
				}
				

				if (l._vals == null) {
					int w = int.Parse(attrs.GetNamedItem("width").Value); // 幅を取得
					int h = int.Parse(attrs.GetNamedItem("height").Value); // 高さを取得
					l.Create(h,w);
				}

				XmlNode node = child.FirstChild; // 子ノードは<data>のみ
				XmlNode n = node.FirstChild; // テキストノードを取得
				string val = n.Value; // テキストを取得
				// CSV(マップデータ)を解析
				string[] line = val.Split ('\n');
				int y = 0;//書き込み場所確認用
				bool lineRead;
				int x = 0;
				string debu;
				for (int i = 0; i < line.Length; i++) {
					lineRead = false;
					debu = i + "行目:";
					string[] s = line [i].Split (',');
					for(int j = 0;j < l.width;j++){
						int v = 0;
						if (j >= s.Length)
							break;
						if(int.TryParse(s[j], out v) == false) { 
							continue;
						}
						//v--;//空を示すデータが0なのに０番からマップチップを用意しているので、全部-1する。-1が空。
						v -= first;
						if (back) {
							if (l.walk [y, x] == false) {
								l.Set (y, x, v);
							}
						} else if(v >= 0){
							l.Set (y, x, v);
							l.walk [y, x] = true;
						}

						debu += v + ",";
						x++;
						lineRead = true;
						/*if (v == 1)
							Debug.Log ("1");*/
					}
					if (lineRead) {
						y++;
						x = 0;
					}
					//Debug.Log (debu);
				}
			}

		}
		//////////////////////////////////
		//Debug.Log(maplayer.Get(0,0));
		Layer2D[] layers = {maplayer,objectsLayer,eventLayer};
		return layers;
	}

	public static int[,] Read(TextAsset path/*string path*/){
		//レイヤーの設定
		Layer2D layer = new Layer2D();
		//データのload
		TextAsset mapDate = path/*Resources.Load (path)as TextAsset*/;

		//Xmlの解析開始
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (mapDate.text);
		XmlNodeList mapList = xmlDoc.GetElementsByTagName ("map");
		foreach (XmlNode map in mapList) {
			XmlNodeList childList = map.ChildNodes;
			foreach(XmlNode child in childList) {
				if(child.Name != "layer") { continue; } // layerノード以外は見ない

				// マップ属性を取得
				XmlAttributeCollection attrs = child.Attributes;
				int w = int.Parse(attrs.GetNamedItem("width").Value); // 幅を取得
				int h = int.Parse(attrs.GetNamedItem("height").Value); // 高さを取得
				// レイヤー生成
				layer.Create(h, w);
				XmlNode node = child.FirstChild; // 子ノードは<data>のみ
				XmlNode n = node.FirstChild; // テキストノードを取得
				string val = n.Value; // テキストを取得
				// CSV(マップデータ)を解析
				string[] line = val.Split ('\n');
				int y = 0;//書き込み場所確認用
				bool lineRead;
				int x = 0;
				string debu;
				for (int i = 0; i < line.Length; i++) {
					lineRead = false;
					debu = i + "行目:";
					string[] s = line [i].Split (',');
					for(int j = 0;j < layer.width;j++){
						int v = 0;
						if (j >= s.Length)
							break;
						if(int.TryParse(s[j], out v) == false) { 
							continue;
						}
						v--;//空を示すデータが0なのに０番からマップチップを用意しているので、全部-1する。-1が空。
						layer.Set (y, x, v);
						debu += v + ",";
						x++;
						lineRead = true;
						/*if (v == 1)
							Debug.Log ("1");*/
					}
					if (lineRead) {
						y++;
						x = 0;
					}
					//Debug.Log (debu);
				}
				/*
				//Debug.Log ("実際のデータ（と思しきもの）");
				for (int i = 0; i < layer.height; i++) {
					string debu2 = i + "行目:";
					for (int j = 0; j < layer.width; j++) {
						debu2 += layer.Get (j, i).ToString () + ",";
					}
					Debug.Log (debu2);
				}
				*/
			}

		}

		//縦,横　に変換(しないことになりました)
		//int[,] mapChip = new int[layer.height, layer.width];
		int[,] mapChip = new int[layer.width, layer.height];
		for(int i = 0;i < layer.width;i++){
			for(int j = 0;j < layer.height;j++){
				//mapChip [j, i] = layer.Get (i, j);
				mapChip [i, j] = layer.Get (i, j);
			}
		}

		return mapChip;
	}
}
