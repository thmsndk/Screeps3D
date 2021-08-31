using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Coroutine coroutine;
    public string Header { get; set; }
    public string Content { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {

        this.coroutine = StartCoroutine(ShowTooltip());
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(this.coroutine);

        TooltipSystem.Hide();

    }

    private IEnumerator ShowTooltip()
    {
        if (!string.IsNullOrEmpty(Content))
        {
            yield return new WaitForSeconds(0.1f);

            TooltipSystem.Show(Content, Header); 
        }
    }
}
