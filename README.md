# Chess Thing

## Overview

This project will be an outlet for me to return and finish an old project, writng a game engine for chess, and explore AI via chess.

## To Do List

### Unity Tasks
- [x] Generate Board using function logic writen in python model
- [x] Get piece class setup and array of all pieces
- [x] Upload Sprites
- [x] Transfer FEN interp fx to Unity from python
- [x] Load starting FEN string
- [x] Learn how to use constructors of class
- [x] Clean up the code
  - [x] Move the board initial from ChessPiece to MainGameDriver
  - [x] Move alot of functions from chesspiece to mainGameDriver
- [x] Work on general movement
  - [x] Be able to pickup a piece
  - [x] Prevent the user from being able to pick up more than one at a time
  - [x] Be able to set a piece down
  - [x] Set the selected piece to be on a higher sprite level
  - [x] Set the piece down on the closet grid spot
- [x] Implement game rules
  - [x] Make a mini version of the board that is just index based
    - [x] Fix the gird being horizontally flipped
    - [x] Make the mini board debug more fancy?
  - [x] Destroy piece when a piece is placed on it
  - [x] Redo the board painting using the new board space list
  - [x] Determine the possible moves for a piece
    - [x] Pawn logic
      - [x] Find forward movement
      - [x] Check if pawn has moved
      - [x] Check if forward is blocked by another piece
      - [x] Check if pawn can move to kill
      - [x] Force pawn to be placed on valid spot
    - [x] Knight Logic
      - [x] Find possible moves on board
      - [x] Check if possible move is blocked by ally
    - [x] Angle Movement
      - [x] Find possible moves on board
      - [x] Check if possible move is blocked by ally
    - [x] Cartisian Movement
      - [x] Find possible moves on board
      - [x] Check if possible move is blocked by ally
    - [x] King Movement
  - [x] Display those moves
    - [x] Highlight possible moves
    - [x] Dehighlight possible moves 
  - [x] Be able to "place" a piece on the spot
- [ ] Modern game logic
  - [x] En Passant
  - [x] Code overhall/clean up
  - [x] En passant take doesn't update the minibaord
  - [ ] Check for king in check
    - [x] Add checks for angular checks
    - [x] Add checks for cartisianal checks
    - [ ] Add checks for kightly checks
    - [ ] Add checks for pawnly checks
    - [ ] Add checks for kingly checks
  - [ ] Setup up system to log a king is no longer in check
  - [ ] Castling
    
