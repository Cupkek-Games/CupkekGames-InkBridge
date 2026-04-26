EXTERNAL SetAvatar(index, charKey)

// ============================================================================
// SPEECH BUBBLE USAGE EXAMPLES
// ============================================================================
//
// NAME-BASED SPEAKER IDENTIFICATION (Default, backward compatible):
//   Format: "CharacterName: dialogue text"
//   Example: "Darius: Hello, Lyra!"
//   - Each character gets their own speech bubble when using SpeechBubbleManager
//   - Works with single-bubble mode (backward compatible)
//
// TAG-BASED SPEAKER IDENTIFICATION (Alternative):
//   Format: "#speaker:CharacterName" followed by dialogue
//   Example:
//     #speaker:darius
//     Hello, Lyra!
//   - Tags take priority over name parsing
//   - Useful when you want to separate speaker identification from dialogue text
//
// MULTIPLE SIMULTANEOUS SPEECH BUBBLES:
//   When using SpeechBubbleManager, each unique speaker name gets their own bubble.
//   Multiple characters can speak in sequence, each maintaining their own bubble.
// ============================================================================

~ SetAvatar(0, "Darius")
~ SetAvatar(1, "Lyra")

// Name-based speaker identification (existing format)
Darius: You're up late again, Lyra. Shouldn't you be resting like the others?

* [I have my own clock.]
    -> branch_self_confident
* [Sleep is overrated when the night calls.]
    -> branch_rebellious
* [I simply cannot sleep; the stars keep me awake.]
    -> branch_poetic

=== branch_self_confident ===
Lyra: \(Smiling\) Confident, aren’t we? But confidence without caution can be dangerous.
Darius: Sharpening my blade is different. A dull sword can't protect anyone.
* [I’m prepared for any challenge.]
    -> prepared_path
* [Perhaps I should temper my pride with a bit of caution.]
    -> humble_path

=== branch_rebellious ===
Lyra: \(Frowning\) Rebellious words won’t earn you any favor with fate.
Darius: Sharpening my blade is different. A dull sword can't protect anyone.
* [I thrive in the chaos of the night.]
    -> dark_path
* [Maybe I can learn to harness both chaos and order.]
    -> balanced_path

=== branch_poetic ===
Lyra: \(Laughing softly\) Poetic, but even the stars need rest.
Darius: Sharpening my blade is different. A dull sword can't protect anyone.
* [Then I'll forge a destiny as vibrant as a sunrise.]
    -> art_path
* [But sometimes the beauty of the night masks my weariness.]
    -> fatigue_path

=== prepared_path ===
Lyra: \(Nodding\) Preparedness is admirable, yet even the strongest sword can lose its edge without rest.
Darius: \(Determined\) I’ll keep my edge sharp, no matter what.
-> END

=== humble_path ===
Lyra: \(Softly\) Humility can be as vital as strength; maybe a little rest is in order.
Darius: \(Resigned\) Perhaps you're right, a moment of pause wouldn't hurt.
-> END

=== dark_path ===
Lyra: \(Cautiously\) The night may sharpen you, but its shadows can also cloud judgment.
Darius: \(Defiant\) I walk in the dark, trusting my instincts.
-> END

=== balanced_path ===
Lyra: \(Thoughtfully\) Balance is key. Embrace both the chaos and the calm.
Darius: \(Contemplative\) Maybe the true art is in finding that equilibrium.
-> END

=== art_path ===
Lyra: \(Intrigued\) A destiny painted with passion and fire—just be sure not to burn out.
Darius: \(Inspired\) I’ll let both art and steel guide my way.
-> END

=== fatigue_path ===
Lyra: \(Worried\) Fatigue can be a silent enemy, even for the most resolute.
Darius: \(Quietly\) Perhaps the beauty of the night is a lullaby urging me to rest.
-> END
