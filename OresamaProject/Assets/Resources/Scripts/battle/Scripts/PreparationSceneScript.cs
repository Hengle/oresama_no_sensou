using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PreparationSceneScript : MonoBehaviour
{

	public static PreparationSceneScript pss;

	[SerializeField]
	private GameObject canvas;

	[SerializeField]
	private TextAsset preparationCSV;

	[SerializeField]
	private string iconPath = "Sprite/characters_face/";

	[SerializeField]
	private Vector3 pos = new Vector3(55, 400, 0);

	static public bool StartButtonPused = false;
    static public bool SceneStart = false;

	private float offsetW;
	private float offsetH;

	//private Vector3 offset = new Vector3(95, 0, 0);

	// user later for button feedback
	//private List<PreparationIcon> players = new List<PreparationIcon>();
	//private List<PreparationIcon> enemies = new List<PreparationIcon>();

	private List<string[]> availableChars = new List<string[]>();

	void Awake()
	{
		pss = this;
        SceneStart = true;
	}

	// Use this for initialization
	void Start ()
	{
		offsetW =  ((float)Screen.width / (float)960);
		offsetH = ((float)Screen.height / (float)540);

		pos = new Vector3(62 * offsetW, 420 * offsetH, 0);

		List<string[]> p = new List<string[]>();
		List<string[]> e = new List<string[]>();

		string[] lines = CSVReader.ReadCSV(preparationCSV);
		for (int i = 1; i < lines.Length; i++)
		{
			string[] row = lines[i].Split(","[0]);
			if (row[1] == "1")
				p.Add(row);
			else if (row[1] == "2")
				e.Add(row);
		}

		AddPlayers(p);
		AddEnemies(e);  
	}

	private void AddPlayers(List<string[]> p)
	{
		Vector3 o = new Vector3(0, 0, 0);
		for (int i = 0; i < 8; i++)
		{
			if (i == 4)
			{ 
				pos = pos + new Vector3(0, -100 * offsetH, 0);
				o = new Vector3(0,0,0);
			}

			GameObject icon = (GameObject)Instantiate(Resources.Load("prefabs/PreparationSceneIcon"), (pos + o), this.transform.rotation);
			icon.transform.SetParent(canvas.transform, true);
			icon.transform.localScale = new Vector3(1, 1, 1);

			Sprite s = null;
			string[] row;
			if (i < p.Count)
			{
				row = p[i];
				s = Resources.Load<Sprite>(iconPath + row[2]);
			}
			else
			{
				row = new string[] { "" };
				s = Resources.Load<Sprite>(iconPath + "none");
			}

			icon.GetComponent<PreparationIcon>().Setup(i, row, true);
			o += new Vector3(100*offsetW, 0, 0);

		}
	}

	private void AddEnemies(List<string[]> e)
	{
		Vector3 o = new Vector3(0, 0, 0);
		for (int i = 0; i < 8; i++)
		{
			if (i == 4)
			{
				pos = pos + new Vector3(0, -100 * offsetH, 0);
				o = new Vector3(0, 0, 0);
			}

			GameObject icon = (GameObject)Instantiate(Resources.Load("prefabs/PreparationSceneIcon"), (pos + new Vector3(540*offsetW,100*offsetH,0) + o), this.transform.rotation);
			icon.transform.SetParent(canvas.transform, true);
			icon.transform.localScale = new Vector3(1, 1, 1);

			Sprite s;
			string[] row;
			if (i < e.Count)
			{
				row = e[i];
				s = Resources.Load<Sprite>(iconPath + row[2]);
			}
			else
			{
				row = new string[] { "" };
				s = Resources.Load<Sprite>(iconPath + "none");
			}

			icon.GetComponent<PreparationIcon>().Setup(i, row);
			o += new Vector3(100*offsetW, 0, 0);

		}
	}

	public void setAvailableChars(List<string[]> list)
	{
		availableChars = list;
	}

	public List<string[]> getAvailableChars()
	{
		return availableChars;
	}

	public void StartButton()
	{
		StartButtonPused = true;
		SceneManager.LoadScene(2);
		Debug.Log("Start");
	}

	public void BackButton()
	{
		SceneManager.LoadScene(3);
		Debug.Log("Back");
	}

	public void MapButton()
	{

		SceneManager.LoadScene(3);

		Debug.Log("MapButton");
	}
}


/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PreparationSceneScript : MonoBehaviour
{

    public static PreparationSceneScript pss;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private TextAsset preparationCSV;

    [SerializeField]
    private string iconPath = "Sprite/characters_face/";

    [SerializeField]
    private Vector3 pos = new Vector3(55, 400, 0);

	static public bool StartButtonPused = false;

    //private Vector3 offset = new Vector3(95, 0, 0);

    // user later for button feedback
    //private List<PreparationIcon> players = new List<PreparationIcon>();
    //private List<PreparationIcon> enemies = new List<PreparationIcon>();

    private List<string[]> availableChars = new List<string[]>();

    void Awake()
    {
        pss = this;
    }

    // Use this for initialization
    void Start ()
    {
        List<string[]> p = new List<string[]>();
        List<string[]> e = new List<string[]>();

        string[] lines = CSVReader.ReadCSV(preparationCSV);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] row = lines[i].Split(","[0]);
                if (row[1] == "1")
                    p.Add(row);
                else if (row[1] == "2")
                    e.Add(row);
            }

            AddPlayers(p);
            AddEnemies(e);  
    }
	
	private void AddPlayers(List<string[]> p)
    {
        Vector3 o = new Vector3(0, 0, 0);
        for (int i = 0; i < 8; i++)
        {
            if (i == 4)
            { 
                pos = pos + new Vector3(0, -100, 0);
                o = new Vector3(0,0,0);
            }
            
            GameObject icon = (GameObject)Instantiate(Resources.Load("prefabs/PreparationSceneIcon"), pos + o, this.transform.rotation);
            icon.transform.SetParent(canvas.transform, true);
            icon.transform.localScale = new Vector3(1, 1, 1);

            Sprite s;
            string[] row;
            if (i < p.Count)
            {
                row = p[i];
                s = Resources.Load<Sprite>(iconPath + row[2]);
            }
            else
            {
                row = new string[] { "" };
                s = Resources.Load<Sprite>(iconPath + "none");
            }

            icon.GetComponent<PreparationIcon>().Setup(i, row, true);
            o += new Vector3(100, 0, 0);

        }
    }

    private void AddEnemies(List<string[]> e)
    {
        Vector3 o = new Vector3(0, 0, 0);
        for (int i = 0; i < 8; i++)
        {
            if (i == 4)
            {
                pos = pos + new Vector3(0, -100, 0);
                o = new Vector3(0, 0, 0);
            }

            GameObject icon = (GameObject)Instantiate(Resources.Load("prefabs/PreparationSceneIcon"), pos + new Vector3(540,100,0) + o, this.transform.rotation);
            icon.transform.SetParent(canvas.transform, true);
            icon.transform.localScale = new Vector3(1, 1, 1);

            Sprite s;
            string[] row;
            if (i < e.Count)
            {
                row = e[i];
                s = Resources.Load<Sprite>(iconPath + row[2]);
            }
            else
            {
                row = new string[] { "" };
                s = Resources.Load<Sprite>(iconPath + "none");
            }

            icon.GetComponent<PreparationIcon>().Setup(i, row);
            o += new Vector3(100, 0, 0);

        }
    }

    public void setAvailableChars(List<string[]> list)
    {
        availableChars = list;
    }

    public List<string[]> getAvailableChars()
    {
        return availableChars;
    }

    public void StartButton()
    {
		StartButtonPused = true;
        SceneManager.LoadScene(2);
        Debug.Log("Start");
    }

    public void BackButton()
    {
        SceneManager.LoadScene(3);
        Debug.Log("Back");
    }

    public void MapButton()
    {
		
        SceneManager.LoadScene(3);

        Debug.Log("MapButton");
    }
}
*/