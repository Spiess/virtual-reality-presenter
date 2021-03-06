using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VirtualRealityPresenter
{
  public class SlidePreviewController : MonoBehaviour, IPointerClickHandler
  {
    private Outline _outline;
    private Action _onClick;

    public void SetSlide(Texture2D slide, Action onClick)
    {
      var image = GetComponent<RawImage>();
      image.texture = slide;
      var aspectRatio = (float) slide.width / slide.height;
      var t = image.rectTransform;
      var parentHeight = transform.parent.GetComponent<RectTransform>().sizeDelta.y;
      t.sizeDelta = new Vector2(parentHeight * aspectRatio, parentHeight);

      _outline = GetComponent<Outline>();
      _onClick = onClick;
    }

    public void Select()
    {
      _outline.enabled = true;
    }

    public void Deselect()
    {
      _outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      _onClick();
    }
  }
}