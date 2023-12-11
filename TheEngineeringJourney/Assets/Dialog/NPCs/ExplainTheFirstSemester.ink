VAR name = "Tande"

-> startInteraction

=== startInteraction ===
As you navigate through Katrinebjerg, you encounter a seasoned student who seems willing to share their experiences.

#NPC: The Experienced Scholar
"Hey there, {name}! I see the look of anticipation on your face. First semester, right?"

+ "Yes, it is. Any advice for a newcomer like me?"
    -> seekAdvice
+ "I'm excited but a bit nervous. How was your first semester?"
    -> inquireAboutExperience


=== seekAdvice ===
The Experienced Scholar smiles knowingly. "Ah, the first semester—the initiation. Here's some advice: embrace the challenges, stay organized, and don't be afraid to seek help when needed. It's a journey of growth."

+ "Thanks for the tips. Any specific challenges I should be prepared for?"
    -> askAboutChallenges
+ "I appreciate that. How did you handle the challenges in your first semester?"
    -> learnFromExperience


=== inquireAboutExperience ===
The Experienced Scholar chuckles. "I remember that feeling. First semester is a mix of excitement and uncertainty. But trust me, you'll learn to navigate the complexities. How can I assist you?"

+ "Tell me about your first semester. What challenges did you face?"
    -> learnFromExperience
+ "I'm eager to hear more. What advice do you have for a newcomer like me?"
    -> seekAdvice


=== askAboutChallenges ===
"The challenges? Well, expect a mix of coursework, projects, and getting used to the university pace. Mathematical, electronic, and assembly minions can be tough too. But each hurdle is a step towards becoming a seasoned engineer."

+ "Sounds like a lot. How did you manage those challenges in your first semester?"
    -> learnFromExperience
+ "I'll keep that in mind. How do you suggest handling the workload?"
    -> handleWorkloadAdvice


=== learnFromExperience ===
The Experienced Scholar reminisces, "My first semester was a whirlwind. I faced tough professors, tricky puzzles, and sleepless nights. But overcoming those challenges made me a better engineer. It's all about perseverance, {name}."

+ "Perseverance, got it. Any specific strategies you found helpful?"
    -> askForStrategies
+ "Thanks for sharing. I'll remember that as I begin my journey."
    -> END


=== handleWorkloadAdvice ===
"Balancing the workload is crucial. Prioritize tasks, break them into manageable steps, and don't hesitate to ask for help. Katrine's Basement is a haven; make good use of it."

+ "Great advice. How did you manage your workload effectively?"
    -> askForStrategies
+ "I'll make sure to prioritize and ask for help. Thanks for the guidance."
    -> END


=== askForStrategies ===
The Experienced Scholar shares, "I found setting a schedule, forming study groups, and taking breaks were effective. Also, don't forget to explore Katrinebjerg; you might find hidden gems that aid your journey."

+ "Solid strategies. I'll apply those. Thanks for the insights!"
    -> END
+ "I'll definitely consider those strategies. Anything else you'd like to share?"
    -> inquireFurther


=== inquireFurther ===
"Aside from strategies, connect with fellow students, attend study sessions, and enjoy the journey. It's not just about surviving; it's about thriving and becoming a skilled engineer."

+ "Thriving, got it. Thanks for the valuable advice. I'm ready for the challenge!"
    -> END