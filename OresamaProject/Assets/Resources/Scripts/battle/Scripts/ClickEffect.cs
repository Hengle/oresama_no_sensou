using UnityEngine;
using System.Collections;

public class ClickEffect : MonoBehaviour
{

    public GameObject clickAnimation;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10.0f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Instantiate(clickAnimation, mousePos, Quaternion.identity);
        }

    }
}
