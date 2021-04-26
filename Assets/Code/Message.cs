using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public Image leftImage;
    public Image rightImage;
    public Text messageText;

    public Color color;
    public bool blink = false;
    public Coroutine blinker;

    // Start is called before the first frame update
    public void SetMessage(string message)
    {
        messageText.text = message;    
    }

    // Update is called once per frame
    void Update()
    {
        if ( blink && blinker == null)
        {
            blinker = StartCoroutine(Blinker());
        }
    }

    public IEnumerator Blinker()
    {
        Color currentColor = color;
        while (blink)
        {
            if ( currentColor.Equals(color))
            {
                currentColor = Color.clear;
            }
            else
            {
                currentColor = color;
            }

            SetColor(currentColor);

            yield return new WaitForSeconds(0.5f);
        }
    }

    void SetColor(Color color)
    {
        leftImage.color = color;
        rightImage.color = color;
        messageText.color = color;
    }
}

