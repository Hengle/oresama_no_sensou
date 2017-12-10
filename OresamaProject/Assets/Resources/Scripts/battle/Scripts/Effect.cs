using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {
	public Vector3 offSet;

    private Animator anim;

	// Use this for initialization
	void Start () 
    {
        anim = GetComponent<Animator>();
		Vector3 pos = transform.position + offSet;
		transform.position = pos;
	}


	// Update is called once per frame
	void Update ()
    {
       // Debug.Log("time " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            Destroy(this.gameObject);

	}
}
