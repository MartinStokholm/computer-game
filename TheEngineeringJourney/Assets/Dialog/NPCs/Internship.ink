VAR name = "Tande"

-> startInteraction

=== startInteraction ===
As you explore Katrinebjerg, you come across an NPC who appears experienced and knowledgeable about internships.

#NPC: The Internship Mentor
"Hello, {name}! Word has it you're on the Engineering Journey. Ready to delve into the realm of internships and gain real-world experience?"

+ "Internships? Tell me more. How do they fit into the Engineering Journey?"
    -> inquireAboutInternships
+ "Real-world experience sounds valuable. Any tips on finding and making the most of internships?"
    -> seekInternshipAdvice


=== inquireAboutInternships ===
The Internship Mentor nods, "Internships are a crucial part of your journey, {name}. They bridge the gap between academia and the professional world. Think of them as hands-on semesters where you apply your engineering skills in real-world scenarios."

+ "Hands-on semesters—interesting! How do internships contribute to my growth as an engineer?"
    -> understandInternshipGrowth
+ "Where can I find internship opportunities, and how do I choose the right one?"
    -> findAndChooseInternship


=== seekInternshipAdvice ===
The Internship Mentor smiles, "Finding and making the most of internships is an art. Network with professionals, explore opportunities in Katrinebjerg, and don't be afraid to step out of your comfort zone. Remember, internships are not just about learning; they're about building connections and showcasing your skills."

+ "Building connections and showcasing skills. Got it. Any specific advice on networking or standing out during internships?"
    -> seekNetworkingAdvice
+ "I'll keep that in mind. Anything else I should know about internships?"
    -> seekAdditionalInternshipAdvice


=== understandInternshipGrowth ===
"Internships offer more than just practical experience. They provide insight into industry practices, help you develop a professional mindset, and often lead to valuable mentorship. It's a journey of growth, both technically and personally."

+ "Insight into industry practices and mentorship—sounds invaluable. How can I make the most of this growth opportunity?"
    -> maximizeInternshipGrowth
+ "A journey of growth. Any tips on navigating this journey successfully?"
    -> seekInternshipSuccessTips


=== findAndChooseInternship ===
"Internship opportunities can be found in various places. Keep an eye on the job boards in Katrinebjerg, attend career fairs, and leverage online platforms. When choosing, consider the industry, the projects you'll be involved in, and the potential for learning and growth."

+ "Job boards, career fairs, and online platforms—got it. Any tips on making the right choice when selecting an internship?"
    -> chooseRightInternship
+ "I'll explore those avenues. Thanks for the guidance on finding and choosing internships!"
    -> END


=== seekNetworkingAdvice ===
"Networking is a powerful tool in the professional world. Attend industry events, connect with alumni, and participate in workshops. Build relationships with professionals in your field, and don't be afraid to express your passion for engineering. Genuine connections often open doors to exciting opportunities."

+ "Industry events, alumni connections, and workshops. I'll start networking actively. Anything else I should keep in mind?"
    -> seekAdditionalInternshipAdvice
+ "Building genuine connections—sounds like a plan. Thanks for the networking advice!"
    -> END


=== seekAdditionalInternshipAdvice ===
"Absolutely! Always be proactive in seeking feedback, be adaptable to new challenges, and maintain a strong work ethic. Internships are not just about the tasks you complete but also about the impression you leave. Make every moment count!"

+ "Proactive feedback, adaptability, and a strong work ethic. Valuable advice. Anything else to add?"
    -> END
+ "I'll keep that in mind. Thanks for the additional advice on internships!"
    -> END


=== maximizeInternshipGrowth ===
"Maximizing growth during internships requires a proactive approach. Seek feedback regularly, take on challenging projects, and don't hesitate to ask questions. Be open to learning from experienced professionals, and use the opportunity to refine your skills and build a strong foundation for your future career."

+ "Proactive feedback, challenging projects, and continuous learning. I'll strive for growth during internships. Any other tips?"
    -> seekInternshipSuccessTips
+ "I'm ready to be proactive and embrace challenges. Thanks for the growth advice during internships!"
    -> END


=== seekInternshipSuccessTips ===
"Success in internships comes down to a combination of technical competence and soft skills. Communicate effectively, collaborate with your team, and showcase your problem-solving abilities. Remember, every successful internship is a stepping stone to a promising engineering career."

+ "Effective communication, collaboration, and problem-solving. Key elements for success. Anything else I should focus on?"
    -> seekAdditionalInternshipAdvice
+ "I'll make sure to hone both technical and soft skills. Thanks for the internship success tips!"
    -> END
    
    === chooseRightInternship ===
"Choosing the right internship requires careful consideration. Evaluate the alignment between the internship and your career goals. Look for opportunities that offer projects matching your interests and provide a supportive learning environment. Additionally, consider the company culture and values to ensure a positive experience."

+ "Alignment with career goals, interesting projects, and a supportive learning environment. I'll keep those criteria in mind. Anything else to consider?"
    -> seekAdditionalInternshipAdvice
+ "Evaluating company culture and values makes sense. Thanks for the advice on choosing the right internship!"
    -> END

