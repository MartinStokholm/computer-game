VAR name = "Tande"

-> startInteraction

=== startInteraction ===
Amidst the hustle and bustle of Katrinebjerg, you spot an NPC with a cheerful demeanor, seemingly excited about something.

#NPC: The Friday Bar Enthusiast
"Hey, {name}! Guess what? It's Friday! You know what that means, right?"

+ "Of course! It's Friday bar time!"
    -> fridayBarExcitement
+ "What's special about Fridays?"
    -> curiousAboutFridayBar


=== fridayBarExcitement ===
The Friday Bar enthusiast grins. "You got it! Friday bars are the best way to unwind after a challenging week of coding. Wanna join me later?"

+ "Absolutely! I could use a break and some fun."
    -> acceptFridayBarInvitation
+ "I'm not sure. Convince me!"
    -> convinceForFridayBar


=== curiousAboutFridayBar ===
"You don't know? Fridays are when we all gather at the Basement for the Friday bar. It's a tradition! Drinks, music, and good company. You should check it out!"

+ "Sounds interesting! Tell me more about the Friday bar."
    -> learnMoreAboutFridayBar
+ "I might give it a try. Thanks for the info!"
    -> considerFridayBar


=== acceptFridayBarInvitation ===
"Awesome! See you at the Friday bar later, {name}!"

+ "Looking forward to it! Let's have a great time."
    -> END


=== convinceForFridayBar ===
The Friday Bar enthusiast starts listing all the perks. "Trust me, it's the perfect way to unwind. Great atmosphere, good drinks, and it's a chance to bond with fellow students. Plus, you've earned it after a week of coding!"

+ "You've convinced me! I'll join you at the Friday bar."
    -> acceptFridayBarInvitation
+ "I'm still not sure. Maybe next time."
    -> declineFridayBarInvitation


=== learnMoreAboutFridayBar ===
The NPC excitedly shares, "Friday bar is a social gathering at the Basement. There's a DJ, snacks, and a variety of drinks. It's the best way to connect with other students and celebrate the end of the week."

+ "Sounds like a blast! I'll definitely check it out."
    -> considerFridayBar
+ "I'm not much of a party person, but thanks for letting me know."
    -> declineFridayBarInvitation

=== declineFridayBarInvitation ===
The Friday Bar enthusiast looks a bit disappointed. "Oh, that's too bad. Well, if you change your mind, you know where to find us. Have a great Friday, {name}!"

+ "Thanks! Maybe next time. Enjoy the Friday bar!"
    -> END

=== considerFridayBar ===
"Give it a shot, {name}! You won't regret it. See you there!"

+ "Alright, I'll join the Friday bar this week. Thanks for inviting me!"
    -> END
+ "I'll think about it. Maybe next time."
    -> END
