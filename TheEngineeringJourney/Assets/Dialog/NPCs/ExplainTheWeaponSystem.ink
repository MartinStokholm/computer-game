VAR name = "Tande"

-> startInteraction

=== startInteraction ===
In the heart of Katrinebjerg, you come across a knowledgeable NPC who seems eager to share insights about the unique weapons used in the Engineering Journey.

#NPC: The Tech Savant
"Hello, {name}! Heard you're gearing up for the Engineering Journey. Ready to dive into the world of computer objects and spells?"

+ "Yes, I'm intrigued. Can you tell me more about the weapons and abilities?"
    -> inquireAboutWeapons
+ "Computer objects and spells? That sounds interesting. How do they work?"
    -> understandComputerWeapons


=== inquireAboutWeapons ===
The Tech Savant grins, "Absolutely! In this journey, your weapons are crafted from computer objects. You can wield them in three ways: hit enemies up close, attack at range, or cast spells from your computer spell book."

+ "Interesting. How do I choose the right weapon for different situations?"
    -> chooseRightWeapon
+ "Tell me more about the computer spell book. How does it work?"
    -> explainComputerSpellBook


=== understandComputerWeapons ===
"Think of your weapons as computerized tools. You can hit enemies with melee weapons, launch attacks from a distance, or unleash magical spells. It's all about adapting to the challenges of each semester."

+ "That's unique. How do I get these computer objects and spells?"
    -> acquireWeaponsAndSpells
+ "Adapting sounds crucial. Any tips on mastering these different techniques?"
    -> seekMasteryTips


=== chooseRightWeapon ===
"The key is to assess the situation. For close encounters, go for melee weapons. When facing multiple enemies or tricky puzzles, ranged attacks work wonders. And for those tough boss battles, unleash the magic from your spell book."

+ "Got it. So, it's about adapting to the challenges of each situation."
    -> understandAdaptation
+ "How do I acquire these weapons and spells?"
    -> acquireWeaponsAndSpells


=== explainComputerSpellBook ===
"The computer spell book is your arsenal of magical abilities. It holds spells that can turn the tide of battle. You'll find and unlock new spells as you progress through the semesters. It's like upgrading your coding skills but in a magical way!"

+ "Sounds fascinating. How do I unlock new spells?"
    -> unlockNewSpells
+ "I'm intrigued. Any specific spells you recommend for a beginner?"
    -> recommendBeginnerSpells


=== acquireWeaponsAndSpells ===
"As you progress through semesters, defeat minions, and conquer challenges, you'll earn computer objects. Visit Katrine's Basement to exchange them for new weapons and spells. The more you explore, the more powerful your arsenal becomes."

+ "Exploration is key. I'll make sure to visit Katrine's Basement regularly. Any other advice?"
    -> seekAdditionalAdvice
+ "I'll keep an eye out for computer objects. Thanks for the guidance!"
    -> END


=== seekMasteryTips ===
"Mastering these techniques takes practice. Engage in battles, experiment with different weapons, and don't shy away from using spells strategically. Oh, and never underestimate the power of a well-timed upgrade from Katrine's Basement."

+ "Practice, experiment, and upgrade. Got it. Anything else I should know?"
    -> seekAdditionalAdvice
+ "I'll keep that in mind. Thanks for the mastery tips!"
    -> END


=== understandAdaptation ===
"Exactly! Adaptability is key. The Engineering Journey throws various challenges at you, and being versatile with your weapons and spells will lead you to victory."

+ "Versatility is crucial. I'll strive to adapt to different situations. Any other insights?"
    -> seekAdditionalAdvice
+ "I appreciate the advice. Versatility it is!"
    -> END


=== unlockNewSpells ===
"As you complete semesters and overcome tougher challenges, you'll unlock new spells naturally. Keep honing your skills, and your magical repertoire will expand."

+ "Looking forward to unlocking new spells. Anything else I should keep in mind?"
    -> seekAdditionalAdvice
+ "I'll focus on honing my skills. Thanks for the info on unlocking spells!"
    -> END


=== recommendBeginnerSpells ===
"For beginners, focus on defensive spells like 'Firewall Shield' to protect yourself. As you grow more confident, experiment with offensive spells like 'Code Lightning.' Balance is the key to success."

+ "Defensive spells sound useful. I'll start with those. Any other recommendations?"
    -> seekAdditionalAdvice
+ "I'll balance offense and defense. Thanks for the beginner spell suggestions!"
    -> END


=== seekAdditionalAdvice ===
"Always be curious and explore. Katrinebjerg has hidden secrets that can aid you. And most importantly, enjoy the journey. It's not just about defeating bosses; it's about becoming a skilled engineer."

+ "Curiosity and exploration—I'll keep that in mind. Thanks for the valuable advice!"
    -> END
+ "Enjoying the journey is important. Thanks for the reminder and advice!"
    -> END
