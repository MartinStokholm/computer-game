VAR name = "Tande"

-> startInteraction

=== startInteraction ===
In the quiet corner of Katrinebjerg, you notice a student hunched over textbooks, looking visibly exhausted.

#NPC: The Weary Student
"Oh, hey there, {name}. I'm just trying to survive these semesters."

+ "You look exhausted. How's everything going?"
    -> inquireAboutWellBeing
+ "Surviving semesters can be tough. Need any help or advice?"
    -> offerAssistance


=== inquireAboutWellBeing ===
The Weary Student lets out a tired sigh. "Exhausted is an understatement, {name}. These semesters are draining, and I'm barely keeping up."

+ "I can relate. It's a tough journey. Anything specific on your mind?"
    -> listenToConcerns
+ "Hang in there. It'll get better. Anything I can do to help?"
    -> offerHelpToTiredStudent


=== offerAssistance ===
The Weary Student looks up, appreciating the concern. "Thanks, {name}. It's just a rough patch. Trying to push through."

+ "If you ever need someone to talk to or share the load, I'm here."
    -> offerListeningEar
+ "Don't hesitate to ask for help. We're all in this together."
    -> encourageToSeekHelp


=== listenToConcerns ===
The Weary Student opens up, "It's the constant pressure, deadlines, and the fear of falling behind. Just overwhelmed, you know?"

+ "I completely understand. Sometimes it helps to share the load. You're not alone in this."
    -> offerListeningEar
+ "Have you considered talking to a mentor or seeking academic support?"
    -> suggestSeekingHelp


=== offerHelpToTiredStudent ===
The Weary Student manages a weak smile. "I appreciate it, {name}. It's just the fatigue catching up. A bit of encouragement goes a long way."

+ "Take a break when you can. You've earned it. If there's anything specific I can do, let me know."
    -> offerSpecificHelp
+ "We all need support. Don't hesitate to ask for help when needed."
    -> encourageToSeekHelp


=== offerListeningEar ===
"Thanks, {name}. It feels good to talk about it. Sometimes it's just nice to know someone understands."

+ "Anytime you need to vent or discuss things, I'm here. We're all in this together."
    -> END


=== encourageToSeekHelp ===
"You might be right, {name}. I haven't really thought about that. Maybe seeking some help would make a difference."

+ "It's worth considering. There's no shame in reaching out for support. We all need it at times."
    -> END

=== suggestSeekingHelp ===
"It might be beneficial to seek help, {name}. Whether it's talking to a mentor, reaching out to academic support, or just finding someone to share the burden with, seeking help can make a significant difference."

+ "I'll consider that. Thanks for the advice."
    -> END
+ "You're right. I'll look into finding support. Appreciate your suggestion."
    -> END


=== offerSpecificHelp ===
The Weary Student nods appreciatively. "A break sounds amazing. Maybe I'll do that. Thanks for the reminder, {name}."

+ "Take your time. If you need assistance with anything specific, feel free to ask. We're all here to help each other."
    -> END
