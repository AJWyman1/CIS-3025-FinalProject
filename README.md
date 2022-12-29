# CIS-3025-FinalProject
## Alexander Wyman
NVU-Lyndon Fall semester 2022 - Advanced OOP   
Final Project

---

NetHack-Like game created using knowledge gained during CIS-3025 Advanced Object Oriented Programming

This project was created using C# 

Inspired by NetHack and D&D

---

## Features
- Random Level Generation
    - Rooms Random size and don't overlap existing rooms
    - Hallways random
    - Hallways create doors when ending or starting on a room
    - Descending stairs randomly placed 
    - Ascending stairs placed at the same location as descending stairs on Dungeon level above
- Items
    - Weapons
        - default stats
        - override defaults for customized weapons
    - Potions
        - Healing potion
        - ***More coming soon***
    - Food
        - Heals a percentage of Max HP
        - Randomly drops from monsters after their death
- Enemies
    - Level 1, 2, 3
        - increasing XP drop and inventory 
    - Interaction with other enemies
    - Randomized spawn based on dungeon level
    - Move towards and attack player when close enough 
- Player
    - Customize Species and Class
    - Sets Attributes based on Class
    - Starting weapon based on Class
    - Gains levels fighting through dungeon 
- Visuals
    - colors for all dungeon inhabitants, loot, and doors
    - Unable to see inside rooms unless currently inside
    - See pathway ahead
    - Room walls, doors, stairs, corridors, stay visible after player discovers the room
---

---
## ToDo


- Game driver class to handle input and build dungeon
- Split up Dungeon class into smaller classes
    - Dungeon
        - Level
            - Room
                 - Stairs
                 - Doors
            - P-ways
- Resistances do something
- Make mobs more player-like
    - pick up items
    - use items
    - Use stairs
    - better movement
- Optimization 
    - Better drawing algorithm 
- Replay option after death   
    - Play again? [y/n]
- Spawn chance for Mobs
- Better inventory system/ Inventory management
    - Drop items
    - Bag upgrades
    - Name your weapon  
- Ranged combat
    - Ranged weapons/ammo
        - ammo in quiver or in bag
    - Aiming ability 
        - straight line 
            - Choose a direction (N, NW, E, SE, etc)
            - check in direction for a target

            or 
        - Select tile to aim at 
            - aim with '×'
            - '×' blinks with char on map
            - choose where to shoot
        - Distance and damage calculation
- More Items
    - Armor
    - Shields
    - more potions
    - Cargo pants to hold more items? (Joke but also....Maybe?)
- Sneak skill
    - Alert mobs at varying range depending on sneak
    - Player Detected attribute
- Dual wield/off hand equipment  
- Class skills   
- End game screen  
- Final Scores   
- Balance issues 
- Other D&D inspired functionality
    - Advantage/Disadvantage
    - Grapple?

### Needs fixing 
- Stairs in walls

## Berlin interpretation of Rogue-likes

> http://www.roguebasin.com/index.php?title=Berlin_Interpretation

-----
**High value factors**
- Random environment generation
- Permanent-death
- Turn-based
- Grid-based
- Non-Modal
    - All actions take place on the same mode.
    - Every action available to player at any point
- Complexity
    - Several solutions to same problem
    - item/item and item/monster interactions
- Resource management
- Hack 'n' slash
- Exploration and discovery
    - exploration of dungeon levels
    - discovery of the usage of unidentified items
----
**Low value factors**
- Single player character
- Monsters are similar to players
    - inventories, equipment, use items, cast spells etc.
- Tactical challenge
    - Learn tactics before making significant progress
    - focus is tactical challenges opposed to solving puzzles or strategically working big picture 
- ASCII Display
- Dungeons
    - levels composed of rooms and corridors
- Numbers
    - Numbers used to describe the character (HP, attributes, etc.)