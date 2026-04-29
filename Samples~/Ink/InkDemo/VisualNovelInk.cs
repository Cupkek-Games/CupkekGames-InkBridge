using CupkekGames.KeyValueDatabases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

namespace CupkekGames.Luna.Ink.Demo
{
    [RequireComponent(typeof(VisualNovelController))]
    public class VisualNovelInk : MonoBehaviour
    {
        [SerializeField] ChoicePopupController _choicesPopupPrefab;
        [SerializeField] KeyValueDatabase<string, InkCharacter> _charDatabase = new();
        [SerializeField] private TextAsset _inkJSONAsset;
        [SerializeField] private float _skipInterval = .1f;
        // Controllers
        private InkStoryControllerExample _storyController;
        private VisualNovelController _uiController;
        // State
        private Coroutine _skipCoroutine;
        private ChoicePopupController _popupController; // Instance
        // Events
        public event Action OnEnd;

        private void Awake()
        {
            _storyController = new InkStoryControllerExample(_inkJSONAsset);

            _uiController = GetComponent<VisualNovelController>();

            // _currentChars = new() { _charDatabase.GetValue("Lyra"), _charDatabase.GetValue("") }; // Initial size 2 because we have 2 slots to show avatars in UI
        }

        private void OnEnable()
        {
            _uiController.OnContinue += Continue;
            _uiController.OnRestart += Restart;
            _uiController.OnSkip += ToggleSkip;

            _uiController.OnTextStart += TextStart;
            _uiController.OnTextComplete += TextComplete;

            _storyController.OnSetAvatar += SetAvatar;
        }

        private void OnDisable()
        {
            _uiController.OnContinue -= Continue;
            _uiController.OnRestart -= Restart;
            _uiController.OnSkip -= ToggleSkip;

            _uiController.OnTextStart -= TextStart;
            _uiController.OnTextComplete -= TextComplete;

            _storyController.OnSetAvatar -= SetAvatar;
            _storyController.Dismiss();
        }

        private void Start()
        {
            // Start is called after Awake and OnEnable of the _controller
            Restart();
        }

        public void Continue()
        {
            if (_uiController.IsPlaying)
            {
                _uiController.EndCurrent();
                return;
            }

            if (!_storyController.Story.canContinue)
            {
                // Story cannot continue. Check if choices are available or if it's the end.
                if (_storyController.Story.currentChoices.Count > 0)
                {
                    ShowChoices(_storyController.Story.currentChoices);
                }
                else
                {
                    // No further choices available – End of story.
                    OnEnd?.Invoke();
                    Destroy(gameObject);
                }

                return;
            }

            string text = _storyController.Story.Continue();

            string[] split = text.Split(":");

            string name = split.Length > 1 ? split[0] : null;
            string dialogue = split.Length > 1 ? split[1] : text;

            name = name?.Trim();
            dialogue = dialogue.Trim();

            if (_uiController.Continue(dialogue, name, false))
            {
                // _nextIndex++;
            }
        }

        private void ToggleSkip()
        {
            if (_skipCoroutine != null)
            {
                StopCoroutine(_skipCoroutine);
                _skipCoroutine = null;
            }
            else
            {
                _skipCoroutine = StartCoroutine(SkipDialogueCoroutine());
            }
        }

        private IEnumerator SkipDialogueCoroutine()
        {
            while (_storyController.Story.canContinue)
            {
                Continue();

                yield return new WaitForSeconds(_skipInterval);
            }
        }

        public void Restart()
        {
            _storyController.Story.ResetState();

            Continue();
        }

        private void TextComplete()
        {
            _uiController.ShowNext();
        }

        private void TextStart()
        {
            _uiController.HideNext();
        }

        public bool HasNext()
        {
            return _storyController.Story.canContinue;
        }
        // Choices
        private void ShowChoices(List<Choice> choices)
        {
            if (_popupController == null)
            {
                _popupController = Instantiate(_choicesPopupPrefab);
                _popupController.OnButtonClick += OnChoiceMade;
            }

            _popupController.Choices = choices
                .Select(choice => new ChoicePopupChoice(choice.text, "slate"))
                .ToArray();

            _popupController.Fade.FadeIn();
        }
        private void OnChoiceMade(int index)
        {
            _storyController.Story.ChooseChoiceIndex(index);
            Continue();
        }

        // Ink Events
        private void SetAvatar(int index, string key)
        {
            // get character from database
            InkCharacter inkCharacter = _charDatabase.GetValue(key);

            if (index == 0)
            {
                _uiController.ShowAvatarLeft(inkCharacter.Avatar);
            }
            else
            {
                _uiController.ShowAvatarRight(inkCharacter.Avatar);
            }
        }
    }
}