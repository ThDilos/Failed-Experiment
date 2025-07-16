using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MarqueeText : MonoBehaviour
{
    public RectTransform viewport;
    public RectTransform textContainer;
    public float scrollSpeed = 50f;
    private bool shouldScroll = false;
    private float textWidth;
    private float viewportWidth;

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(textContainer); // force layout update
        textWidth = textContainer.rect.width;
        viewportWidth = viewport.rect.width;

        shouldScroll = textWidth > viewportWidth;

        if (shouldScroll)
        {
            StartCoroutine(ScrollText());
        }
    }

    private IEnumerator ScrollText()
    {
        float startX = 0f;
        float endX = -(textWidth - viewportWidth);

        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                float x = Mathf.Lerp(startX, endX, t);
                textContainer.anchoredPosition = new Vector2(x, 0);
                t += Time.deltaTime * (scrollSpeed / Mathf.Abs(endX));
                yield return null;
            }

            // pause at end
            yield return new WaitForSeconds(1f);

            // scroll back to start
            t = 0f;
            while (t < 1f)
            {
                float x = Mathf.Lerp(endX, startX, t);
                textContainer.anchoredPosition = new Vector2(x, 0);
                t += Time.deltaTime * (scrollSpeed / Mathf.Abs(endX));
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

