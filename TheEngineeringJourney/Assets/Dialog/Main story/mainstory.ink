VAR name = "Tande"

Hi {name}, i have heard that you are going to be a engineer. 


i hope you are going to survive this journey. 

-> london

=== london ===
It is going to be a long and hard journeyand you might nope be up for the channgels.  
#"Passepartout," said he. "We are going around the world!"

+ "Are you up for the channgels, Monsieur {name}?"
    -> astonished
+ [Nod curtly.] -> nod


=== astonished ===
"You are in jest!" I told him in dignified affront. "You make mock of me, Monsieur."
"I am quite serious."

+ "But of course"
    -> ending


=== nod ===
I nodded curtly, not believing a word of it.
-> ending


=== ending
"We shall circumnavigate the globe within eighty days." He was quite calm as he proposed this wild scheme. "We leave for Paris on the 8:25. In an hour."
-> END