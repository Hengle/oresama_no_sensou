using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleSceneUI : MonoBehaviour {

	[SerializeField]
	private Image faceIcon;

	[SerializeField]
	private Text charNameText;

	[SerializeField]
	private RectTransform uGuiElement;

	[SerializeField]
	private Vector2 OnPosition;
	private Vector2 OutPosition;

	// Use this for initialization
	void Start ()
	{
		OnPosition = uGuiElement.anchoredPosition;
		transform.position += new Vector3(0, 1000, 0);
		OutPosition = uGuiElement.anchoredPosition;
	}

	public void Setup()
	{
		OnPosition = uGuiElement.anchoredPosition;
		transform.position += new Vector3(0, 1000, 0);
		OutPosition = uGuiElement.anchoredPosition;
	}

	public void SetText(string s)
	{
		if (charNameText != null)
			charNameText.text = s;
	}

	public void SetSprite(Sprite s)
	{
		faceIcon.sprite = s;
	}


	public Vector2 targetPosition;
	public void ShowUI()
	{
		iTween.ValueTo(uGuiElement.gameObject, iTween.Hash(
			"from", uGuiElement.anchoredPosition,
			"to", OnPosition,
			"time", 1.0f,
			"onupdatetarget", this.gameObject,
			"onupdate", "MoveGuiElement"));
	}

	public void HideUI()
	{
		iTween.ValueTo(uGuiElement.gameObject, iTween.Hash(
			"from", uGuiElement.anchoredPosition,
			"to", OutPosition,
			"time", 1.0f,
			"onupdatetarget", this.gameObject,
			"onupdate", "MoveGuiElement"));
	}

	public void MoveGuiElement(Vector2 position)
	{
		uGuiElement.anchoredPosition = position;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) { 
			// ShowUI();
		}
	}
}



