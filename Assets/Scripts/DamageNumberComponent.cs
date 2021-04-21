using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberComponent : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    public float Alpha;
    public float yOffset;
    private float startYPosition;
    public void Initialise(float number)
    {
        startYPosition = transform.position.y;
        if (number < 0)
        {
            number *= -1;
            textMesh.color = Color.red;
        }
        else
        {
            textMesh.color = Color.green;
        }

        var colour = textMesh.color;
        colour.a = 0;
        textMesh.color = colour;

        textMesh.text = number.ToString("0");
    }

    public void Update()
    {
        var colour = textMesh.color;
        colour.a = Alpha;
        textMesh.color = colour;
        var position = transform.position;
        position.y = startYPosition + yOffset;
        transform.position = position;

        if (colour.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
