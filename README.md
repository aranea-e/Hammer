# Hammer
A fast-paced real-time card game

This game was created by myself and a friend from the scouting program for our Game Design Merit Badge. It took many forms over the short period we worked on it and it quickly became popular within our scout troop. Originally, there were no jokers, the game was 3x3, aces were wild on the table, and you could only hold 10 cards. During development, the 21 max hand size was added so I didn't need to zoom out and render more cards or anything like that.

# Gameplay

At the start of the game, each player gets 7 cards and a 3x4 grid of face-up cards is placed on the table. Players can see their own cards, but not other peoples' cards. As cards are removed from the table, they are discarded and replaced.

Gameplay works by matching cards in your hand with those on the table. You want to keep your card count low, but you might get punished if you're too hasty, because other players can play on top of your cards. If another player plays on your card, you draw 3 (a "hit"). However, if you or a third player choose to play a fourth card, "Hammer" is declared. The player who it was played on must try to get the cards first while all other players except the one who triggered it can quickly attempt to throw another card into the pile. More of the same card being played can keep reversing the "Hammer" provided that no other card is played on it first. If a pile is not interacted with for a long enough period, then it is removed from the table.

Jokers and Aces are both played as wilds, but cannot be played on top of other players' cards. When a joker is played, the person playing it chooses any other player to draw 2 (a "tap"). This can backfire, as if somebody plays on top of your joker, then the "hit" becomes a draw 5 (2 from the "tap", 3 from the "hit"). When a Joker or Ace appears on the table, then all players may attempt to hit it first. Whoever does receives the card, and can then discard 1 card.

If a player runs out of cards, the game is paused for a moment and all other players draw 3 (a "quake"). The player who ran out also draws 3, but draws an additional card for each time they've triggered a "quake" that game.

Players must keep playing constantly. If a player does not play a card for 5 seconds, they draw 2. Players can at any time request 2 cards and reset this timer.

If the deck ever runs out of cards, the game is paused and all discards are shuffled. All players' cards are counted, and if a player has above 15 cards, they are eliminated. If a player ever would need to draw a 22nd card, they are eliminated immediately.
