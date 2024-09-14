## Mine Sweeper solver

Features:
 - Uses screen capture to detect board
 - Automatically click on safe/dangerous squares

Highlights:
 - Can solve solutions which depend on multiple clues
   - No support for more than 2 clues yet
 - Combinatoric yield
   - Extension method to return all combinations of a certain length of items in a list
   - This is used to check the various possible solutions for a given clue and calculate whether each state is valid
     - 
 - Matrix interface
   - Stores and enumerates tiles
   - Has default implementation which simply stores the tiles
   - Has proxy implementation which intercepts requests for tiles and can temporarily replace them with a different value
     - Used when evaluating possible solutions to calculate the state without messing with the actual matrix data

![cropped](https://github.com/user-attachments/assets/ebec2e77-e4bc-4c7f-bc9e-4b8e8744a7d2)

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
