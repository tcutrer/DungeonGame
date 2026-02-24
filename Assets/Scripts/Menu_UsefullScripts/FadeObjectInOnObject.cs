using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FadeObjectInOnObject : MonoBehaviour
{

    public float fadeInDuration = 1f; // Duration of the fade-in effect
    private float timeElapsed = 0f;


    public void startFadeIn(){
        StartCoroutine(FadeIn());
        
    }

    private IEnumerator FadeIn(){
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        Renderer renderer = GetComponent<Renderer>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Start fully transparent

        }
        else if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = 0f; // Start fully transparent
            renderer.material.color = color;
        }


        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(timeElapsed / fadeInDuration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            else if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }

            yield return null; // Wait for the next frame
        }
    }
}
