using CupkekGames.KeyValueDatabases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CupkekGames.Luna;
using Ink.Runtime;
using UnityEngine;

namespace CupkekGames.Luna.Ink.Demo
{
    public class SpeechBubbleInk : MonoBehaviour
    {
        [SerializeField] ChoicePopupController _choicesPopupPrefab;
        [SerializeField] KeyValueDatabase<string, InkCharacter> _charDatabase = new();
        [SerializeField] private TextAsset _inkJSONAsset;
        [SerializeField] private SpeechBubbleManager _bubbleManager;
        [SerializeField] private SpeechBubbleController _mainBubble; // For backward compatibility

        // Controllers
        private InkStoryControllerExample _storyController;
        private SpeechBubbleController _currentBubble; // Currently active bubble
        private Dictionary<string, int> _speakerToIndex = new Dictionary<string, int>(); // Track speaker positions
        private int _nextSpeakerIndex = 0; // Next available speaker index

        private HashSet<SpeechBubbleController>
            _subscribedBubbles = new HashSet<SpeechBubbleController>(); // Track subscribed bubbles

        // State
        private ChoicePopupController _popupController; // Instance

        // Events
        public event Action OnEnd;

        private void Awake()
        {
            _storyController = new InkStoryControllerExample(_inkJSONAsset);

            // Backward compatibility: if no manager, use main bubble
            if (_bubbleManager == null && _mainBubble == null)
            {
                _mainBubble = GetComponent<SpeechBubbleController>();
            }

            // Set initial current bubble
            _currentBubble = _mainBubble;
        }

        private void OnEnable()
        {
            // Subscribe to main bubble if using single-bubble mode
            if (_mainBubble != null)
            {
                SubscribeToBubble(_mainBubble);
            }

            _storyController.OnSetAvatar += SetAvatar;
        }

        private void OnDisable()
        {
            // Unsubscribe from all subscribed bubbles
            foreach (var bubble in _subscribedBubbles)
            {
                if (bubble != null)
                {
                    bubble.OnContinue -= Continue;
                    bubble.OnTextStart -= TextStart;
                    bubble.OnTextComplete -= TextComplete;
                }
            }

            _subscribedBubbles.Clear();

            _storyController.OnSetAvatar -= SetAvatar;
            _storyController.Dismiss();
        }

        private void SubscribeToBubble(SpeechBubbleController bubble)
        {
            if (bubble != null && !_subscribedBubbles.Contains(bubble))
            {
                bubble.OnContinue += Continue;
                bubble.OnTextStart += TextStart;
                bubble.OnTextComplete += TextComplete;
                _subscribedBubbles.Add(bubble);
            }
        }

        private void Start()
        {
            // Start is called after Awake and OnEnable of the _controller
            Restart();
        }

        public void Continue()
        {
            // Determine which bubble to use
            SpeechBubbleController activeBubble = _currentBubble;

            // Check if current bubble is playing (for multi-bubble, check the one that triggered this)
            if (activeBubble != null && activeBubble.IsPlaying)
            {
                activeBubble.EndCurrent();
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
                    if (_bubbleManager != null)
                    {
                        _bubbleManager.ClearAllBubbles();
                    }

                    Destroy(gameObject);
                }

                return;
            }

            string text = _storyController.Story.Continue();

            // Parse speaker from tags first (priority)
            string speakerName = ParseSpeakerFromTags(_storyController.Story);

            // If no tag, parse from dialogue text (name: dialogue format)
            if (string.IsNullOrEmpty(speakerName))
            {
                string[] split = text.Split(":");
                speakerName = split.Length > 1 ? split[0]?.Trim() : null;
                text = split.Length > 1 ? split[1] : text;
            }

            text = text.Trim();

            // Get or create bubble for this speaker
            if (_bubbleManager != null)
            {
                // Multi-bubble mode: get/create bubble for speaker
                if (string.IsNullOrEmpty(speakerName))
                {
                    speakerName = _bubbleManager.DefaultSpeaker;
                }

                activeBubble = _bubbleManager.GetOrCreateBubble(speakerName);
                SubscribeToBubble(activeBubble);
            }
            else if (_mainBubble != null)
            {
                // Single-bubble mode: use main bubble
                activeBubble = _mainBubble;
            }
            else
            {
                Debug.LogError("SpeechBubbleInk: No bubble manager or main bubble assigned!");
                return;
            }

            _currentBubble = activeBubble;

            // Get character info if name is provided
            Sprite avatarLeft = null;
            Sprite avatarRight = null;
            SpeechBubbleArrowPosition arrowPosition = SpeechBubbleArrowPosition.None;

            if (!string.IsNullOrEmpty(speakerName))
            {
                InkCharacter character = _charDatabase.GetValue(speakerName);
                if (character != null)
                {
                    // Determine speaker index (for arrow position)
                    if (!_speakerToIndex.ContainsKey(speakerName))
                    {
                        _speakerToIndex[speakerName] = _nextSpeakerIndex % 2;
                        _nextSpeakerIndex++;
                    }

                    int speakerIndex = _speakerToIndex[speakerName];

                    if (speakerIndex == 0)
                    {
                        avatarLeft = character.Avatar;
                        arrowPosition = SpeechBubbleArrowPosition.BottomLeft;
                    }
                    else
                    {
                        avatarRight = character.Avatar;
                        arrowPosition = SpeechBubbleArrowPosition.BottomRight;
                    }
                }
            }

            if (activeBubble.Continue(text, avatarLeft, avatarRight, arrowPosition, false))
            {
                // Dialogue started successfully
            }
        }

        /// <summary>
        /// Parses speaker name from Ink tags. Looks for #speaker:name format.
        /// </summary>
        /// <param name="story">The Ink story instance.</param>
        /// <returns>Speaker name if found in tags, null otherwise.</returns>
        private string ParseSpeakerFromTags(Story story)
        {
            if (story == null || story.currentTags == null)
            {
                return null;
            }

            foreach (string tag in story.currentTags)
            {
                if (tag.StartsWith("speaker:", StringComparison.OrdinalIgnoreCase))
                {
                    string speakerName = tag.Substring("speaker:".Length).Trim();
                    if (!string.IsNullOrEmpty(speakerName))
                    {
                        return speakerName;
                    }
                }
            }

            return null;
        }

        public void Restart()
        {
            _storyController.Story.ResetState();
            _speakerToIndex.Clear();
            _nextSpeakerIndex = 0;

            if (_bubbleManager != null)
            {
                _bubbleManager.ClearAllBubbles();
            }

            Continue();
        }

        private void TextComplete()
        {
            if (_currentBubble != null && _currentBubble.AutoShowNext)
            {
                _currentBubble.ShowNext();
            }
        }

        private void TextStart()
        {
            if (_currentBubble != null)
            {
                _currentBubble.HideNext();
            }
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
            // Store the speaker index mapping for arrow positioning
            if (!_speakerToIndex.ContainsKey(key))
            {
                _speakerToIndex[key] = index;
            }
        }
    }
}