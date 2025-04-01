using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    Image img;
    Color tempColor;
    private float targetAlpha;
    void Start()
    {
        img = GetComponent<Image>();
        tempColor = img.color;
        float brightness = PlayerPrefs.GetFloat("brightness", 1f);
        float minAlpha = 0.9f;
        float maxAlpha = 0f;
        targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, brightness);
        tempColor.a = targetAlpha;
        img.color = tempColor;

        StartCoroutine(FadeIn(0.15f));
    }

   public IEnumerator FadeIn(float seconds) 
   {
        img.raycastTarget = true;
        float endAlpha = PlayerPrefs.HasKey("brightness")
            ? Mathf.Lerp(0.9f, 0f, PlayerPrefs.GetFloat("brightness"))
            : 0f;

        tempColor = img.color;
        tempColor.a = 1f; // Полная чёрнота в начале
        img.color = tempColor;

        for (float a = 1f; a >= endAlpha; a -= 0.05f)
        {
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(seconds);
        }
        tempColor.a = endAlpha;
        img.color = tempColor;
        img.raycastTarget = false;
    }

   public IEnumerator FadeOut(float seconds) {
        img.raycastTarget = true;

        float startAlpha = img.color.a;
        float endAlpha = 1f;

        for (float t = 0f; t <= 1f; t += 0.05f) {
            float a = Mathf.Lerp(startAlpha, endAlpha, t);
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(seconds);
        }
       
    }
}
