VAR name = "Tande"

-> startInteraction

=== startInteraction ===
In the midst of Katrinebjerg, you come across a disheveled guy swaying unsteadily.

#NPC: The Drunken Reveler
"Hey, hic {name}! You know what's hic fantastic? Everything!"

+ "You seem like you're having a good time."
    -> acknowledgeDrunkenness
+ "Are you alright? You seem a bit... tipsy."
    -> expressConcern


=== acknowledgeDrunkenness ===
"Haha, you betcha! This hic is the best day ever. You should join me, {name}!"

+ "Maybe later. Take care of yourself, though."
    -> declineDrunkenInvitation
+ "I'll pass. You enjoy your day!"
    -> declineDrunkenInvitation


=== expressConcern ===
The Drunken Reveler squints, trying to focus. "Concerned? Nah, I'm hic just enjoying life to the fullest!"

+ "Just be careful, alright? It's important to stay safe."
    -> offerHelp
+ "Alright, have fun. Take care of yourself."
    -> declineOfferHelp


=== declineDrunkenInvitation ===
The Drunken Reveler stumbles a bit but manages to keep his balance. "You're missing out on the hic party of the century!"

+ "I'll catch the next one. Stay safe, my friend!"
    -> END


=== offerHelp ===
"Help? I don't need help, {name}! I've got everything under control."

+ "If you say so. Just watch your step, okay?"
    -> declineOfferHelp
+ "Seriously, let me help you get somewhere safe."
    -> insistOnHelp


=== insistOnHelp ===
The Drunken Reveler chuckles. "hic You're a good hic friend, {name}. Lead the way!"

+ "Alright, let's get you to a safe spot."
    -> guideToSafety
+ "Maybe it's best if you rest here. I'll check on you later."
    -> leaveDrunkenReveler


=== declineOfferHelp ===
"No worries, {name}! I'm the master of my hic destiny."

+ "Take care then. See you around!"
    -> END


=== guideToSafety ===
You guide The Drunken Reveler to a quiet spot away from the crowd, ensuring he's safe.

+ "Rest here for a bit. I'll check on you later."
    -> leaveDrunkenReveler
+ "Do you need anything else before I go?"
    -> offerFurtherAssistance


=== leaveDrunkenReveler ===
The Drunken Reveler gives a drowsy nod. "hic Thanks, {name}. You're a real hic pal."

+ "No problem. Take it easy, my friend. See you later."
    -> END


=== offerFurtherAssistance ===
"Nah, I'm hic good. You've done enough, {name}. I'll catch some z's."

+ "Alright, take care. See you around!"
    -> END
