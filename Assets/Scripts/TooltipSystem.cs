using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : BaseSingleton<TooltipSystem>
{
    [SerializeField] private Tooltip tooltip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Show(string content, string header = "")
    {
        Cursor.visible = false;
        Instance.tooltip.SetText(content, header);
        Instance.tooltip.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        Cursor.visible = true;
        Instance.tooltip.gameObject.SetActive(false);
    }
}
