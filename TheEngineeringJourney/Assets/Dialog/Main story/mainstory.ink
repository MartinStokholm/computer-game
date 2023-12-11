VAR name = "Tande"

Hi {name}, I have heard that you are going to be an engineer.

-> katrinebjerg

=== katrinebjerg ===
As a young aspiring software engineering student at the University of Aarhus, you find yourself standing in the heart of Katrinebjerg. The journey ahead is filled with challenges, and you must fight through each semester, defeating different professors to absorb their knowledge.

#"The Engineering Journey awaits, {name}. Are you ready to face the trials and become an engineer?"

+ "I am ready."
    -> challenges
+ "This sounds daunting..."
    -> hesitate


=== challenges ===
It won't be easy, {name}. Each semester brings new obstacles and tougher professors. You'll explore the buildings of Katrinebjerg, facing mathematical, electronic, and assembly minions. Upgrade your equipment and seek refuge in the Basement for potions and new gear.

+ "I accept the challenge."
    -> semester1


=== hesitate ===
The path to becoming an engineer is challenging, but it's a journey of growth and knowledge. Embrace the difficulties, and you'll emerge victorious.

+ "I'll give it my best shot."
    -> challenges


=== semester1 ===
You stand at the beginning of your journey. The first semester lies ahead, and the professor awaits in the depths of a building. Explore, fight minions, solve puzzles, and upgrade your skills. Remember, the Basement is your haven.

#"May your coding be bug-free, {name}. The first semester begins!"

-> END
