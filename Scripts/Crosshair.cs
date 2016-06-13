using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

    public LayerMask targetMask;
    public SpriteRenderer dot;

    public Color dotHighlightColor;
    Color dotOriginalColor;

    void Start()
    {
        Cursor.visible = false;
        dotOriginalColor = dot.color;
    }


    void Update()
    {
        transform.Rotate(Vector3.forward * -60 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = dotOriginalColor;
        }
    }

}
