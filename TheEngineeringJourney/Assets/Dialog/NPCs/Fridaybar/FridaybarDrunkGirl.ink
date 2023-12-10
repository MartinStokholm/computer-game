VAR name = "Tande"

-> startInteraction

=== startInteraction ===
Amidst the lively atmosphere of Katrinebjerg, you spot an energetic girl dancing to imaginary beats.

#NPC: The Party Enthusiast
"Hey, {name}! Ready to turn this place into a dance floor?"

+ "Absolutely! What's the occasion?"
    -> excitedAboutParty
+ "I'm not sure. What's going on?"
    -> inquireAboutParty


=== excitedAboutParty ===
The Party Enthusiast grins. "No occasion needed! Every day is a party waiting to happen. Wanna join the fun?"

+ "Count me in! Let's make it a night to remember."
    -> acceptPartyInvitation
+ "I'm not much of a dancer. Maybe next time."
    -> declinePartyInvitation


=== inquireAboutParty ===
"Didn't you hear? We're turning this place into a party zone! Music, dancing, and good vibes. Interested?"

+ "Tell me more! I'm up for some excitement."
    -> learnMoreAboutParty
+ "I might join. Sounds like fun."
    -> considerParty


=== acceptPartyInvitation ===
The Party Enthusiast cheers. "That's the spirit! Get ready for a night of pure joy, {name}!"

+ "Can't wait! Let's get this party started."
    -> END


=== declinePartyInvitation ===
The Party Enthusiast pouts playfully. "Aw, come on! You're missing out on the best dance moves in Katrinebjerg."

+ "Maybe next time. Have a blast, though!"
    -> END


=== learnMoreAboutParty ===
"The party is spontaneous, {name}! No specific reason, just good times. You should join us. It's all about letting loose and enjoying the moment."

+ "Sounds like a plan! I'll join the festivities."
    -> considerParty
+ "I'll think about it. Thanks for the invitation."
    -> END


=== considerParty ===
"Take your time deciding, {name}. The party won't stop without you, though!"

+ "Alright, I'll join the party tonight. Thanks for inviting me!"
    -> END
+ "I'll see if I can make it. Have fun, though!"
    -> END
