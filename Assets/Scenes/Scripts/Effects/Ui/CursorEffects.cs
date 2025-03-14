using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static CursorEffects;

public class CursorEffects : MonoBehaviour
{
    public static CursorEffects instance;
    public Image[] cursorParts;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    public void TurnRed()
    {
        for(int i =0;i< cursorParts.Length;i++)
        {
            cursorParts[i].DOColor(Color.red, 0.05f);
        }
        
    }
    public void TurnWhite()
    {
        for (int i = 0; i < cursorParts.Length; i++)
        {
            cursorParts[i].DOColor(Color.white, 0.05f);
        }
    }
    // left = 1
    // right = 2
    // down = 3
    // up = 4
    // center = 5
    float leftX = -20;
    float rightX = 20;
    float downY = -20;
    float upY = 20;

    public float maxMoveMulty = 2f;

    public void PrecisionSize(float precision)
    {
        for (int i = 0; i < cursorParts.Length; i++)
        {
            
            Image image = cursorParts[i];
            int part = i + 1;
            float precisionMove = precision;
            precisionMove *= maxMoveMulty/2f;
            switch (part)
            {
                case 1:
                    precisionMove = maxMoveMulty - precisionMove;
                    float positionLeft = Mathf.Clamp(precisionMove * leftX, float.MinValue, leftX);
                    if (precision == 1)
                    {
                        positionLeft = leftX;
                    }
                    image.transform.localPosition = Vector3.Lerp(image.transform.localPosition, new Vector3(positionLeft, 0, 0),Time.fixedDeltaTime * 10);
                    break;
                case 2:
                    precisionMove = precisionMove - maxMoveMulty;
                    float positionRight = Mathf.Clamp(-precisionMove * rightX,rightX, float.MaxValue);
                    if (precision == 1)
                    {
                        positionRight = rightX;
                    }
                    image.transform.localPosition = Vector3.Lerp(image.transform.localPosition, new Vector3(positionRight, 0, 0), Time.fixedDeltaTime * 10);
                    break;
                case 3:
                    precisionMove = maxMoveMulty - precisionMove;
                    float positionDown = Mathf.Clamp(precisionMove * downY, float.MinValue, downY);
                    if (precision == 1)
                    {
                        positionDown = downY;
                    }
                    image.transform.localPosition = Vector3.Lerp(image.transform.localPosition, new Vector3(0, positionDown, 0), Time.fixedDeltaTime * 10);
                    break;
                case 4:
                    precisionMove = maxMoveMulty - precisionMove;
                    float positionUp = Mathf.Clamp(precisionMove * upY, upY, float.MaxValue);
                    if (precision == 1)
                    {
                        positionUp = upY;
                    }
                    image.transform.localPosition = Vector3.Lerp(image.transform.localPosition, new Vector3(0, positionUp, 0), Time.fixedDeltaTime * 10);
                    break;
            }
            
        }
      
    }
}
