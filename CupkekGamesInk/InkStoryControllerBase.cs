using UnityEngine;
using Ink.Runtime;

namespace CupkekGames.Luna.Ink
{
  public abstract class InkStoryControllerBase
  {
    // The ink story that we're wrapping
    private Story _story;
    public Story Story => _story;

    public InkStoryControllerBase(TextAsset inkJSONAsset)
    {
      _story = new Story(inkJSONAsset.text);

      BindInkFuctions();
    }

    protected virtual void BindInkFuctions()
    {
    }

    protected virtual void UnbindInkFuctions()
    {
    }

    public void Dismiss()
    {
      UnbindInkFuctions();
    }
  }
}