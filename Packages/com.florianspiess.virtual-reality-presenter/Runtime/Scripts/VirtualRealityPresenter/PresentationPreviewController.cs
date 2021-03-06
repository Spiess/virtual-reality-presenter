using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VirtualRealityPresenter
{
  public class PresentationPreviewController : MonoBehaviour
  {
    public PresentationController presentation;
    public RectTransform scrollContent;
    public SlidePreviewController slidePreviewPrefab;

    private SlidePreviewController[] _previews;

    private void Awake()
    {
      presentation.RegisterPreview(this);
    }

    public void SetSlides(IEnumerable<Texture2D> slides)
    {
      _previews = slides.Select((slide, index) =>
      {
        var slidePreview = Instantiate(slidePreviewPrefab, scrollContent);
        slidePreview.SetSlide(slide, () => presentation.SetSlide(index));
        return slidePreview;
      }).ToArray();
      _previews[0].Select();
    }

    public void SelectSlide(int slideIndex)
    {
      foreach (var preview in _previews)
      {
        preview.Deselect();
      }

      _previews[slideIndex].Select();

      // Scroll to slide
      var ap = scrollContent.anchoredPosition;
      ap.x = -_previews[slideIndex].GetComponent<RectTransform>().anchoredPosition.x;
      scrollContent.anchoredPosition = ap;
    }
  }
}