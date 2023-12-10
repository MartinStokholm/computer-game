VAR name = "Tande"

-> startInformation

=== startInformation ===
As you stroll through Katrinebjerg, you overhear students talking excitedly about the upcoming Friday bar.

#NPC: The Rumor Mill
"Hey, {name}! Have you heard about the Friday bar at the Basement? It's legendary!"

+ "Tell me more."
    -> learnAboutFridayBar
+ "I haven't. What's so special about it?"
    -> curiousAboutFridayBar


=== learnAboutFridayBar ===
The student enthusiastically shares, "The Friday bar is the highlight of the week. The Basement transforms into this vibrant hub with music, snacks, and a variety of drinks. It's the perfect place to unwind and connect with fellow students."

+ "Sounds amazing! I'll check it out."
    -> END


=== curiousAboutFridayBar ===
"You're in for a treat! The Friday bar is a tradition here. Picture this: good music, tasty snacks, and a fantastic atmosphere. It's the ultimate way to celebrate the end of the week."

+ "I'm intrigued! I'll make sure to attend."
    -> END
