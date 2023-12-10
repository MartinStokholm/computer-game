VAR openShop = false
VAR name = "Tande"

-> shop

=== shop ===
#"Welcome, {name}! What can I help you with today?"

+ "I'd like to buy something."
    -> buyMenu
+ "I'll come back later."
    -> exitShop

=== buyMenu ===
Here's what's available for purchase:
    ~ openShop = true
    ->END

    === exitShop ===
You leave Katrine's Basement, carrying your newly acquired items. The challenges of the Engineering Journey await.
    ~ openShop = false
    -> END

-> END