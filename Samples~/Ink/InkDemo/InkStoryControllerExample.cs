using System;
using UnityEngine;

namespace CupkekGames.Luna.Ink.Demo
{
    public class InkStoryControllerExample : InkStoryControllerBase
    {
        // Events
        public Action<int, string> OnSetAvatar; // index, name
        public InkStoryControllerExample(TextAsset inkJSONAsset) : base(inkJSONAsset)
        {

        }

        protected override void BindInkFuctions()
        {
            Story.BindExternalFunction("SetAvatar", (int index, string name) => SetAvatar(index, name));
        }

        protected override void UnbindInkFuctions()
        {
            Story.UnbindExternalFunction("SetAvatar");
        }

        private void SetAvatar(int index, string name)
        {
            OnSetAvatar?.Invoke(index, name);
        }
    }
}