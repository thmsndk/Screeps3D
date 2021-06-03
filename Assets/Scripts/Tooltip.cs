using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text content;
    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private int characterWrapLimit = 80;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            ResizeTooltip();
        }

        Vector2 position = Input.mousePosition;
        
        // move to mouse
        transform.position = position + new Vector2(50f, 0f);

        // keep tooltip on screen
        var pivotX = position.x / Screen.width;
        var pivotY = position.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

    }

    private void ResizeTooltip()
    {
        //int headerLength = header.text.Length;
        //int contentLength = (int)content.textBounds.size.x;

        layoutElement.enabled = Math.Max(header.preferredWidth, content.preferredWidth) >= layoutElement.preferredWidth; ;
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            this.header.gameObject.SetActive(false);
        }
        else
        {
            this.header.gameObject.SetActive(true);
            this.header.text = header;
        }

        this.content.text = content;

        ResizeTooltip();
    }
}
