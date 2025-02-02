﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace VirtualRealityPresenter
{
  public class PresentationController : MonoBehaviour
  {
    public string slideDirectoryPath;

    public InputAction nextSlide;
    public InputAction previousSlide;

    private Texture2D[] _slides;

    private RawImage _projector;
    private AspectRatioFitter _arf;
    private int _currentSlide;

    private readonly List<PresentationPreviewController> _previews = new List<PresentationPreviewController>();

    private async void Start()
    {
      _projector = GetComponent<RawImage>();
      _arf = GetComponent<AspectRatioFitter>();

      // Get all the files in the given directory
      var files = Directory.GetFiles(slideDirectoryPath);

      var imageExtensions = new[] {".jpg", ".jpeg", ".png"};

      // Filter the files by the allowed types
      var imageFiles = files.Where(file => imageExtensions.Any(file.EndsWith)).ToList();

      // Check that at least a single slide was found
      if (imageFiles.Count == 0)
      {
        var allowedExtensions = string.Join(", ", imageExtensions);
        Debug.LogError($"Could not find any valid image files (endings: {allowedExtensions}) at the location: " +
                       $"{slideDirectoryPath}");
        return;
      }

      // Load the slide images
      var data = await Task.Run(() => imageFiles.Select(File.ReadAllBytes));
      _slides = data.Select(rawData =>
      {
        var tex = new Texture2D(2, 2);
        tex.LoadImage(rawData);
        return tex;
      }).ToArray();

      if (_slides.Length == 0)
      {
        Debug.LogError("Could not load any slides!");
      }
      else
      {
        // Initialize presentation previews
        foreach (var preview in _previews)
        {
          preview.SetSlides(_slides);
        }

        SetSlide(0);
      }

      // Set input bindings
      nextSlide.performed += _ => NextSlide();
      previousSlide.performed += _ => PreviousSlide();
    }

    private void OnEnable()
    {
      nextSlide.Enable();
      previousSlide.Enable();
    }

    private void OnDisable()
    {
      nextSlide.Disable();
      previousSlide.Disable();
    }

    public void RegisterPreview(PresentationPreviewController preview)
    {
      _previews.Add(preview);
    }

    /// <summary>
    /// Switch the presentation to the next slide (unless already showing the last slide).
    /// </summary>
    public void NextSlide()
    {
      if (_currentSlide + 1 < _slides.Length)
      {
        SetSlide(_currentSlide + 1);
      }
    }

    /// <summary>
    /// Switch the presentation to the previous slide (unless already showing the first slide).
    /// </summary>
    public void PreviousSlide()
    {
      if (_currentSlide > 0)
      {
        SetSlide(_currentSlide - 1);
      }
    }

    /// <summary>
    /// Switch to the slide with the given index.
    /// </summary>
    /// <param name="slideIndex">Index of the slide to switch to</param>
    public void SetSlide(int slideIndex)
    {
      if (slideIndex >= _slides.Length || slideIndex < 0)
      {
        Debug.LogError($"Slide index {slideIndex} out of bounds! ({_slides.Length} slides)");
        return;
      }

      _currentSlide = slideIndex;
      var slide = _slides[slideIndex];
      _projector.texture = slide;
      _arf.aspectRatio = (float) slide.width / slide.height;

      // Update previews
      foreach (var preview in _previews)
      {
        preview.SelectSlide(slideIndex);
      }
    }
  }
}