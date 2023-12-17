VAR name = "Tande"

-> startInteraction

=== startInteraction ===
As you walk through Katrinebjerg, you notice an NPC standing alone near the Basement entrance.

#NPC: The Nervous Freshman
"Oh, hi {name}! I'm really nervous about my first semester. I've heard it's tough."

+ Don't worry, you'll do great! Everyone is nervous at the beginning.
    -> encourage
+ I totally understand. The first semester can be overwhelming.
    -> empathize


=== encourage ===
You think so? I'm just not sure if I can keep up with all the coding and stuff.

+ Absolutely! It's normal to feel nervous, but you'll learn and improve.
    -> continueExploring


=== empathize ===
It's a lot, right? I feel like I'm drowning in information. How did you handle it when you started?

+ I felt the same way. But you gotta keep your head up!
    -> continueExploring
+ Let's navigate this journey together. I can show you some helpful resources and tips.
    -> continueExploring


=== continueExploring ===
The nervous freshman smiles, feeling a bit more at ease.
    -> END
