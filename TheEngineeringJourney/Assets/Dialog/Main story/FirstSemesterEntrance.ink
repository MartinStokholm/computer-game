VAR name = "Tande"
VAR readyState = false

-> semester1

=== semester1 ===
You stand at the entrance of the first building, the challenges of the semester lying ahead. The professor is hidden deep within, guarded by minions. Your journey to becoming an engineer begins now.

#"Welcome, {name}, to the first semester of the Engineering Journey. Are you prepared for what lies ahead?"
    -> ready

=== ready ===
+ "I'm ready."

    -> challenges1
+ "Can you give me some advice?"
    -> advice1
+ "I'm not ready"
    -> notReadyYet


=== challenges1 ===
In this semester, you'll face mathematical, electronic, and assembly minions. Defeat them to clear rooms, solve puzzles to access the professor's lair, and upgrade your equipment in Katrine's Basement. Remember, the journey may be tough, but each challenge conquered brings you closer to being an engineer.

+ "I understand."
    -> start1


=== advice1 ===
#"To succeed, {name}, you must strategically navigate each room. Defeat minions, collect loot, and use the Basement wisely. Don't forget to explore; hidden loot may aid your journey. Good luck!"

+ "Thank you for the advice."
    -> advice2
    
=== advice2 ===
#"Are you ready now?"
+ "Can you give me some advice?"
    -> start1
+ "I'm not ready"
    -> notReadyYet

=== notReadyYet ===
#"Come back when you are ready." 
    -> END

=== start1 ===
You step into the first building, backpack ready and determination in your eyes. The semester is a maze of challenges and victories. Will you emerge triumphant?
     ~ readyState = true

-> END
