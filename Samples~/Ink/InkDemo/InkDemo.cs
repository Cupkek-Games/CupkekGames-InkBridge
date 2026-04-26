using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Ink.Demo
{
  public class InkDemo : UIViewComponent
  {
    private Button _play; // Reference to the settings button
    [SerializeField] private GameObject _vnPrefab; // Prefab to instantiate
    private VisualNovelInk _instance;

    protected override void Awake()
    {
      base.Awake();

      // Locate the button within the ParentElement
      _play = ParentElement.Q<Button>("Play");

      // Register the click event
      if (_play != null)
      {
        _play.clicked += OnPlayClicked;
      }
      else
      {
        Debug.LogError("Play button not found in the UI");
      }
    }

    private void OnPlayClicked()
    {
      _play.SetEnabled(false);

      // Check if the settings prefab is assigned
      if (_vnPrefab != null)
      {
        GameObject vnObject = Instantiate(_vnPrefab);

        if (vnObject.TryGetComponent<VisualNovelInk>(out _instance))
        {
          _instance.OnEnd += OnEnd;
        }
      }
      else
      {
        Debug.LogError("Visual Novel prefab is not assigned.");
      }
    }

    private void OnEnd()
    {
      _instance.OnEnd -= OnEnd;

      _play.SetEnabled(true);
    }

    private void OnDestroy()
    {
      // Unregister the click event to avoid memory leaks
      if (_play != null)
      {
        _play.clicked -= OnPlayClicked;
      }
    }
  }
}
