using CupkekGames.KeyValueDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

namespace CupkekGames.Luna.Ink.Demo
{
    [RequireComponent(typeof(VisualNovelController))]
    public class InkTextEffectsDemo : MonoBehaviour
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
        private ChoicePopupController _popupController;

        // Events
        public event Action OnEnd;

        private void Awake()
        {
            _storyController = new InkStoryControllerExample(_inkJSONAsset);
            _uiController = GetComponent<VisualNovelController>();
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
                if (_storyController.Story.currentChoices.Count > 0)
                {
                    ShowChoices(_storyController.Story.currentChoices);
                }
                else
                {
                    OnEnd?.Invoke();
                }

                return;
            }

            string text = _storyController.Story.Continue();
            string[] split = text.Split(":");
            string name = split.Length > 1 ? split[0] : null;
            string dialogue = split.Length > 1 ? split[1] : text;

            name = name?.Trim();
            dialogue = dialogue.Trim();

            _uiController.Continue(dialogue, name, false);
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

        private void SetAvatar(int index, string key)
        {
            InkCharacter inkCharacter = _charDatabase.GetValue(key);
            if (inkCharacter == null)
            {
                return;
            }

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

