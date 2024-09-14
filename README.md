## Mine Sweeper solver

Features:
 - Use screen capture to detect board
 - Automatically click on all safe/dangerous squares

Highlights:
 - Can solve solutions which require 2 clues rather than just 1
   - No support for more than 2 clues yet
 - Combinatoric yield
   - Extension method to return all combinations of a certain length of items in a list
   - This is used to check the various possible solutions for a given clue and calculate whether each state is valid
 - Proxy matrix class
   - Layer above the base matrix representation to temporarily alter various values
   - Used when evaluating possible solutions to calculate the state without messing with the actual matrix data

![mindsweeper](https://github.com/user-attachments/assets/af4486ba-a58d-4bf6-9971-3a77f2627379)


### Todo

 - Cover the remainding edge cases which this algorithm cant handle
 - Can heavily optimise in many places
   - Eg: The screenshot is captured more than required
   - Consolidate the calls to get safe/dangerous/ambivalent clues. No need to iterate the entire array 3 times.
   - *Calculating safe tiles requires knowledge of dangerous tiles but some of that could be done in the same call*
   - Currently it recalculates the entire board for each permutation of each clue. Can optimise by only recalculating surrounding 15 tiles and only recalculating if there are clues in the surrounding 15 tiles.


### Caveats

Keep in mind some games are unsolvable, for example:

![image](https://github.com/user-attachments/assets/06981e01-fd13-47b0-89a9-4b7f8e3a3cb4)
