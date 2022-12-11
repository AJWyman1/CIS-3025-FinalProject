using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Dungeon
{

    public char[,,] Map;
    public char[,,] EmptyMap;
    public Dictionary<(int, int, int), Creature> MonsterDict;
    public PlayerCharacter Hero;
    public Dictionary<(int, int, int), Dictionary<Item, int>> LootDict;
    public Room[][] Rooms;
    public Dictionary<(int, int, int), Room> RoomDict;
    public int Width;
    public int Length;
    private bool MapFinalized;
    public string[] MessageHistory;
    public int CurrentLevel;

    public Dungeon(PlayerCharacter c, int X, int Y, int Rooms = 10)
    {
        this.Hero = c;
        this.Length = X;
        this.Width = Y;
        this.CurrentLevel = 0;
        this.MapFinalized = false;
        this.Map = new char[X, Y, 3];

        FillMap();
        this.EmptyMap = (char[,,])this.Map.Clone();

        this.Rooms = new Room[3][];
        this.Rooms[0] = new Room[5];
        this.Rooms[1] = new Room[10];
        this.Rooms[2] = new Room[1];
        this.RoomDict = new Dictionary<(int, int, int), Room>();
        this.MonsterDict = new Dictionary<(int, int, int), Creature>();
        this.LootDict = new Dictionary<(int, int, int), Dictionary<Item, int>>(); //Dictionary of Loot on floor of the dungeon Dict( X, Y : Dict(Item : NumOfType))
        this.CreateRooms(1, 0);
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine(i);
            this.CreateRooms(this.Rooms[this.CurrentLevel].Length);
            this.EmptyMap = (char[,,])this.Map.Clone();
            while (!this.Navigatable())
            {
                this.UpdateAllRooms(true);
                this.CreatePasageways(2);
                this.EmptyMap = (char[,,])this.Map.Clone();
                this.MonsterDict = new Dictionary<(int, int, int), Creature>();
            }
            Console.WriteLine("Navigatable " + i);
            this.DebugPrint(0, 0, this.CurrentLevel);

            this.Stairs(this.CurrentLevel, this.Rooms[this.CurrentLevel][Dice.Roll(this.Rooms[this.CurrentLevel].Length -1)], true); // Add random Downstairs
            Console.WriteLine("Stairs");
            this.CurrentLevel += 1;
        }

        this.CurrentLevel = 0;

        this.MapFinalized = true;
        //this.Stairs();
        this.UpdateAllRooms(true);

        this.EmptyMap = (char[,,])this.Map.Clone();
        this.MonsterDict = new Dictionary<(int, int, int), Creature>();
        this.PopulateDungeon(13, 25, 1);
        this.LootDict = new Dictionary<(int, int, int), Dictionary<Item, int>>(); //Dictionary of Loot on floor of the dungeon Dict( X, Y : Dict(Item : NumOfType))
        this.MessageHistory = new string[5] { "", "", "", "", "" };
    }

    public void PopulateDungeon(int lvl1, int lvl2, int lvl3)
    {
        for (int i = 0; i < lvl1; i++)
        {
            this.PlaceCreatureInRoom(0);
        }
        for (int i = 0; i < lvl2; i++)
        {
            this.PlaceCreatureInRoom(1);
        }
        for (int i = 0; i < lvl3; i++)
        {
            this.PlaceCreatureInRoom(2);
        }
    }

    public void NewMessage(string Message)
    {
        for (int i = 1; i < this.MessageHistory.Length; i++)
        {
            this.MessageHistory[i - 1] = this.MessageHistory[i];
        }
        this.MessageHistory[this.MessageHistory.Length - 1] = Message;

        this.PrintMessageHistory();
    }

    public void PrintMessageHistory()
    {
        Console.SetCursorPosition(0, this.Length);
        for (int i = 0; i < this.MessageHistory.Length; i++)
        {
            //Console.SetCursorPosition(0, this.Length + i);
            Console.WriteLine(this.MessageHistory[i].PadRight(this.Width));
        }
    }

    public void FillMap()
    {
        for (int k = 0; k < this.Map.GetLength(2); k++)
        {
            for (int i = 0; i < this.Map.GetLength(0); i++)
            {
                for (int j = 0; j < this.Map.GetLength(1); j++)
                {
                    this.Map[i, j, k] = ' ';
                }
            }
        }
    }

    // I WARNED YOU ABOUT THE STAIRS BRO;
    public void Stairs()
    {
        int X;
        int Y;
        for (int k = 0; k < 2; k++)
        {
            do
            {
                X = Dice.Roll(this.Map.GetLength(0) - 1);
                Y = Dice.Roll(this.Map.GetLength(1) - 1);
            } while (this.Map[X, Y, k] != '.'); //Rolls dice for location
            this.GetRoom(X, Y, k).AddStairs(X, Y, true);
            this.Map[X, Y, k] = this.GetRoom(X, Y, k).StairLocations[(X, Y)].RepresentWith;

            do
            {
                X = Dice.Roll(this.Map.GetLength(0) - 1);
                Y = Dice.Roll(this.Map.GetLength(1) - 1);
            } while (this.Map[X, Y, k] != '.'); //Rolls dice for location
            this.GetRoom(X, Y, k).AddStairs(X, Y, false);
            this.Map[X, Y, k] = this.GetRoom(X, Y, k).StairLocations[(X, Y)].RepresentWith;
        }
        do
        {
            X = Dice.Roll(this.Map.GetLength(0) - 1);
            Y = Dice.Roll(this.Map.GetLength(1) - 1);
        } while (this.Map[X, Y, 2] != '.'); //Rolls dice for location
        this.GetRoom(X, Y, 2).AddStairs(X, Y, false);
        this.Map[X, Y, 2] = this.GetRoom(X, Y, 2).StairLocations[(X, Y)].RepresentWith;
    }

    public void Stairs(int Level, Room StairRoom, Boolean Down)
    {
        int X = StairRoom.X;
        int Y = StairRoom.Y;

        X += Dice.Roll(StairRoom.Width - 1);
        Y += Dice.Roll(StairRoom.Length - 1);

        StairRoom.AddStairs(X, Y, Down);

        if (Down)
        {
            this.Map[X, Y, Level] = '<';
            Console.WriteLine($"down stairs created at {X}, {Y}");
            this.CreateRoomContaining(X, Y, Level + 1);
            Console.WriteLine("Containing room made");
        }else
        {
            this.Map[X, Y, Level] = '>';
        }
    }

    public void CreateRoomContaining(int ContainsY, int ContainsX, int Level)
    {
        int width = Dice.Roll(10) + 5;
        int length = Dice.Roll(10) + 5;
        
        int X = ContainsX - length/2;
        int Y = ContainsY - width/2;
        while(X + length > this.Width || Y + width > this.Length)
        {
            width = Dice.Roll(5) + 5;
            length = Dice.Roll(5) + 5;
                
            X = ContainsX - width/2;
            Y = ContainsY - length/2;
            
            Console.WriteLine($"Attempting {width}x{length} at ({X}, {Y}) .     Max {this.Width} {this.Length}");
        }
        this.Rooms[Level][0] = new Room(Y, X, width, length, this.Hero, Level);
        this.RoomDict.Add((X, Y, Level), this.Rooms[Level][0]);
        UpdateMap(this.Rooms[Level][0], Level, true);

        this.Stairs(Level, this.Rooms[Level][0], false, ContainsY, ContainsX);
        this.UpdateAllRooms();
        this.DebugPrint(0, 0 , Level);
    }

    public void Stairs(int Level, Room StairRoom, Boolean Down, int X, int Y)
    {
        StairRoom.AddStairs(X, Y, Down);

        if (Down)
        {
            this.Map[X, Y, Level] = '<';
        }else
        {
            this.Map[X, Y, Level] = '>';
        }
    }

    public void PlaceHeroInRoom(PlayerCharacter p)
    {
        this.Hero = p;
        PlaceCreatureInRoom(this.CurrentLevel, this.Hero);
        this.Stairs(0, this.GetRoom(this.Hero.X, this.Hero.Y, 0), false, this.Hero.X, this.Hero.Y);
    }

    public void PlaceHeroInRoom(PlayerCharacter p, int X, int Y)
    {
        this.Hero = p;
        this.MonsterDict.Add((X, Y, this.CurrentLevel), this.Hero);
    }

    public void PlaceCreatureInRoom(int Level, Creature c = null)
    {
        int X;
        int Y;
        do
        {
            X = Dice.Roll(this.Map.GetLength(0) - 1);
            Y = Dice.Roll(this.Map.GetLength(1) - 1);
        } while (this.MonsterDict.ContainsKey((X, Y, Level)) || this.Map[X, Y, Level] != '.'); //Rolls dice for location
        if (c == null)
        {
            int MonsterRoll = Dice.D20();
            if (Level == 0)
            {
                MonsterRoll = Dice.D8();
            }
            else if (Level == 1)
            {
                MonsterRoll = Dice.Roll(17) + 2;
            }
            else
            {
                MonsterRoll = 20;
            }

            if (MonsterRoll <= 3)
            {
                this.MonsterDict.Add((X, Y, Level), new GiantRat());
            }
            else if (MonsterRoll <= 7)
            {
                this.MonsterDict.Add((X, Y, Level), new Bandit());
            }
            else if (MonsterRoll <= 13)
            {
                this.MonsterDict.Add((X, Y, Level), new Hobgoblin());
            }
            else if (MonsterRoll <= 19)
            {
                this.MonsterDict.Add((X, Y, Level), new Orc());
            }
            else if (MonsterRoll == 20)
            {
                this.MonsterDict.Add((X, Y, Level), new Mimic());
            }
        }
        else
        {
            this.MonsterDict.Add((X, Y, Level), c);
        }

        this.MonsterDict[(X, Y, Level)].GoTo(X, Y);
        this.MonsterDict[(X, Y, Level)].RollInitiative();
        this.MonsterDict[(X, Y, Level)].DungeonLevel = Level;
    }

    public bool AddMob(Creature c, int DungeonLevel)
    {
        if (MovementAllowed(c.X, c.Y, DungeonLevel, c))
        {
            this.MonsterDict.Add((c.X, c.Y, DungeonLevel), c);
            c.DungeonLevel = DungeonLevel;
            return true;
        }
        return false;
    }

    public void RemoveMob(Creature c)
    {
        this.MonsterDict.Remove((c.X, c.Y, c.DungeonLevel));
    }

    public bool MovementAllowed(int X, int Y, int DungeonLevel, Creature c)
    {

        if (new char[] { '│', '┌', '┐', '┘', '└', '─', ' ' }.Contains(this.Map[X, Y, DungeonLevel]))
        {
            return false;
        }
        if (this.MonsterDict.ContainsKey((X, Y, DungeonLevel)))
        {
            if (this.MonsterDict[(X, Y, DungeonLevel)].IsAlive)
            {
                PrintFightLog(c, this.MonsterDict[(X, Y, DungeonLevel)]);
                return false;
            }
            else
            {
                this.MonsterDict.Remove((X, Y, DungeonLevel));
            }
        }
        if (this.Map[X, Y, this.CurrentLevel] == '━' || this.Map[X, Y, this.CurrentLevel] == '┃')
        {
            return true;
        }
        return true;
    }

    public bool MovementAllowed(int X, int Y, int DungeonLevel)
    {
        if (new char[] { '│', '┌', '┐', '┘', '└', '─', ' ' }.Contains(this.Map[X, Y, DungeonLevel]))
        {
            return false;
        }
        if (this.MonsterDict.ContainsKey((X, Y, DungeonLevel)))
        {
            return false;
        }
        return true;
    }

    public void PrintFightLog(Creature a, Creature b)
    {
        if (a.RepresentWith == b.RepresentWith)
        {
            return;
        }

        Console.SetCursorPosition(0, this.Length);
        if (a is PlayerCharacter || b is PlayerCharacter)
        {
            if (a.RepresentWith == 'C')
            {

                this.NewMessage("As you reach to see what treasures are locked away...");
                this.NewMessage(a.Attack(b));
                this.PrintMessageHistory();
                return;
            }
            if (b.RepresentWith == 'C')
            {

                this.NewMessage("As you reach to see what treasures are locked away...");
                this.NewMessage(b.Attack(a));
                this.PrintMessageHistory();
                return;
            }

            this.NewMessage(a.Attack(b));

            this.PostFightUpdate(a);
            this.PostFightUpdate(b);
        }
        else
        {
            a.Attack(b);
            if (a.DungeonLevel == this.Hero.DungeonLevel)
            {
                this.NewMessage("You hear fighting in the distance");
            }

            this.PostFightUpdate(a);
            this.PostFightUpdate(b);

        }
        if (b.HP <= 0) //XP gain
        {
            if (a is PlayerCharacter)
            {
                this.NewMessage($"{a.Name} gains {b.XPGiven} XP!");
            }
            a.GainXP(b.XPGiven);
        }

        this.PrintLocation(a.X, a.Y, a.DungeonLevel);
        this.PrintLocation(b.X, b.Y, b.DungeonLevel);
    }

    public void ClearMessages()
    {
        Console.SetCursorPosition(0, this.Length);
        Console.WriteLine("".PadRight(Console.WindowWidth));
        Console.WriteLine("".PadRight(Console.WindowWidth));
        Console.WriteLine("".PadRight(Console.WindowWidth));
        Console.WriteLine("".PadRight(Console.WindowWidth));

        this.PrintMessageHistory();
    }

    public void PostFightUpdate(Creature c)
    {
        if (!c.IsAlive)
        {
            if (c.EquippedWeapon != null)
            {
                this.AddToLootDict(c.EquippedWeapon, c.X, c.Y, c.DungeonLevel);
            }

            for (int i = 0; i < c.Inventory.Entries.Length; i++)
            {
                if (c.Inventory.Entries[i] != null)
                {
                    Item loot = c.RemoveItem(i);
                    this.AddToLootDict(loot, c.X, c.Y, c.DungeonLevel);
                }
            }

            this.MonsterDict.Remove((c.X, c.Y, c.DungeonLevel));
            this.PrintLocation(c.X, c.Y, c.DungeonLevel);


        }
        PrintLocation(c.X, c.Y, c.DungeonLevel);
    }

    public void AddToLootDict(Item loot, int X, int Y, int DungeonLevel)
    {
        if (this.LootDict.ContainsKey((X, Y, DungeonLevel)))
        {
            if (this.LootDict[(X, Y, DungeonLevel)].ContainsKey(loot))
            {
                this.LootDict[(X, Y, DungeonLevel)][loot] += 1; //stack if multiple of same items
            }
            else
            {
                this.LootDict[(X, Y, DungeonLevel)].Add(loot, 1);
            }
        }
        else
        {
            Dictionary<Item, int> TempDict = new Dictionary<Item, int> { { loot, 1 } };
            this.LootDict.Add((X, Y, DungeonLevel), TempDict);
        }
    }

    public ConsoleKey MonsterTurn()
    {
        ConsoleKey key = default(ConsoleKey);
        foreach (KeyValuePair<(int, int, int), Creature> Mob in this.MonsterDict.OrderByDescending(key => key.Value))
        {
            if (Mob.Value is PlayerCharacter)
            {
                key= this.PlayerTurn();
            }
            else
            {
                MonsterMove(Mob.Value);
            }
        }
        return key;
    }

    public void MonsterMove(Creature c)
    {
        if (!c.IsAlive)
        {
            this.MonsterDict.Remove((c.X, c.Y, c.DungeonLevel));
            return;
        }
        if (c is Mimic)
        {
            this.PrintLocation(c.X, c.Y, c.DungeonLevel);

            if (c.RepresentWith == 'C')
            {
                return;
            }
        }

        if (this.NearHero(c.X, c.Y, c.DungeonLevel, 1))
        {
            this.PrintFightLog(c, this.Hero);
            return;
        }

        int lastX = c.X;
        int lastY = c.Y;
        int LastDungeonLevel = c.DungeonLevel;

        int Move = Dice.D8();
        if (this.NearHero(c.X, c.Y, c.DungeonLevel, 5))
        {
            Move = Dice.D4();
            if (c.X < this.Hero.X || (c.X == this.Hero.X && c.Y > this.Hero.Y)) //W, SW, S, SE
            {
                Move += 4;
            }

            if (c.Y < this.Hero.Y || (c.Y == this.Hero.Y && c.X < this.Hero.X)) //NE, E, SE, S
            {
                if (Move % 2 == 1)
                {
                    Move += 1;
                }
            }
            else //SW, W, NW, N
            {
                if (Move % 2 == 0)
                {
                    Move -= 1;
                }
            }

        }

        if (Move == 1)
        {
            if (this.MovementAllowed(c.X - 1, c.Y, c.DungeonLevel, c))
            { c.MoveNorth(); }
        }
        else
        if (Move == 5)
        {
            if (this.MovementAllowed(c.X, c.Y - 1, c.DungeonLevel, c))
            { c.MoveWest(); }
        }
        else
        if (Move == 6)
        {
            if (this.MovementAllowed(c.X + 1, c.Y, c.DungeonLevel, c))
            { c.MoveSouth(); }
        }
        else
        if (Move == 4)
        {
            if (this.MovementAllowed(c.X, c.Y + 1, c.DungeonLevel, c))
            { c.MoveEast(); }
        }
        else
        if (Move == 3)
        {
            if (this.MovementAllowed(c.X - 1, c.Y - 1, c.DungeonLevel, c))
            { c.MoveNorthWest(); }
        }
        else
        if (Move == 2)
        {
            if (this.MovementAllowed(c.X - 1, c.Y + 1, c.DungeonLevel, c))
            { c.MoveNorthEast(); }
        }
        else
        if (Move == 7)
        {
            if (this.MovementAllowed(c.X + 1, c.Y - 1, c.DungeonLevel, c))
            { c.MoveSouthWest(); }
        }
        else
        if (Move == 8)
        {
            if (this.MovementAllowed(c.X + 1, c.Y + 1, c.DungeonLevel, c))
            { c.MoveSouthEast(); }
        }
        if (lastX != c.X || lastY != c.Y)
        {
            this.MovementUpdate(c, lastX, lastY);
        }
        else
        {
            this.PrintLocation(c.X, c.Y, c.DungeonLevel);
        }
    }

    public ConsoleKey PlayerTurn()
    {
        if (this.Hero.HP <= 0)
        {
            return default(ConsoleKey);
        }

        ConsoleKey key;
        Console.SetCursorPosition(0, Console.WindowHeight - 20);
        key = Console.ReadKey(true).Key;
        int lastX = this.Hero.X;
        int lastY = this.Hero.Y;
        if ((char)key == 'W')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveNorth(); }
        }
        else
        if ((char)key == 'A')
        {
            if (this.MovementAllowed(this.Hero.X, this.Hero.Y - 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveWest(); }
        }
        else
        if ((char)key  == 'X')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveSouth(); }
        }
        else
        if ((char)key  == 'D')
        {
            if (this.MovementAllowed(this.Hero.X, this.Hero.Y + 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveEast(); }
        }
        else
        if ((char)key == 'Q')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y - 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveNorthWest(); }
        }
        else
        if ((char)key  == 'E')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y + 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveNorthEast(); }
        }
        else
        if ((char)key  == 'Z')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y - 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveSouthWest(); }
        }
        else
        if ((char)key  == 'C')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y + 1, this.Hero.DungeonLevel, this.Hero))
            { this.Hero.MoveSouthEast(); }
        }
        else
        if ((char)key  == 'R')
        {
            // **ToDo**
            // Make rest have charges
            bool EnemyNearby = false;
            for(int x = this.Hero.X - 10; x < this.Hero.X + 10; x++)
            {
                for(int y = this.Hero.Y - 10; y < this.Hero.Y + 10; y++)
                {
                    if (x != this.Hero.X && y != this.Hero.Y)
                        if (this.MonsterDict.ContainsKey((x, y, this.Hero.DungeonLevel)))
                        {
                            EnemyNearby = true;
                        }
                }
            }
            if (EnemyNearby)
            {
                this.NewMessage("You can not rest with enemies nearby!");
            }
            else
            {
                this.NewMessage(this.Hero.Rest());
            }
            
        }
        else
        if ((char)key  == 'I')
        {
            // **ToDo**
            // Inventory managment, drop items,

        }
        else
        if ((char)key  == 'S')
        {
            //  A[s]cend/De[s]cend
            int LevelChange = 0;
            if (this.EmptyMap[this.Hero.X, this.Hero.Y, this.CurrentLevel] == '<') //Down
            {
                LevelChange += 1;
            }
            if (this.EmptyMap[this.Hero.X, this.Hero.Y, this.CurrentLevel] == '>') //Up
            {
                LevelChange -= 1;
            }
            if (LevelChange == -1)
            {
                this.NewMessage("Would you like to exit the dungeon? ");
            }
            if (LevelChange != 0)
            {
                if(this.AddMob(this.Hero,this.CurrentLevel + LevelChange))
                {
                    
                    this.MonsterDict.Remove((this.Hero.X, this.Hero.Y, this.Hero.DungeonLevel - LevelChange));
                    this.CurrentLevel += LevelChange;
                    Console.Clear();
                    this.PrintMap();
                }
            }
        }
        else
        if ((char)key  == 'P')
        {
            if (this.LootDict.ContainsKey((this.Hero.X, this.Hero.Y, this.Hero.DungeonLevel)))
            {


                //Print out items on the ground
                Container<Item> Loot = this.ItemsOnGround(this.Hero.X, this.Hero.Y, this.Hero.DungeonLevel);
                PrintLoot(Loot);
                //Take input
                int input = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
                //Pick up item selected
                try
                {
                    if (Loot.Entries[input] != null)
                    {
                        this.Hero.PickUpItem(Loot.Entries[input]);
                        this.ItemPickedUp(Loot.Entries[input], this.Hero.X, this.Hero.Y, this.Hero.DungeonLevel);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    this.NewMessage("You try to pick up something that isn't there");
                }
                Console.Clear();
                this.PrintMap();
                this.PrintMessageHistory();
            }
        }
        else
        {
            int input = (int)Char.GetNumericValue((char)key ) - 1;
            if (input >= 0 && this.Hero.Inventory.Entries[input] != null)
            {
                Console.Clear();

                PrintCharSheet(this.Hero);
                this.PrintMap();
                Item item = this.Hero.Inventory.Entries[input];
                this.NewMessage(item.Use(this.Hero, input));
                if (item.UsesLeft < 1)
                {
                    this.Hero.RemoveItem(input);
                }
            }
        }


        this.Map[lastX, lastY, this.CurrentLevel] = this.EmptyMap[lastX, lastY, this.CurrentLevel];
        this.Map[this.Hero.X, this.Hero.Y, this.CurrentLevel] = (char)this.Hero;

        if ((lastX != this.Hero.X || lastY != this.Hero.Y))
        {
            this.NewMessage("");
            this.MovementUpdate(this.Hero, lastX, lastY);
            //Console.Clear();
            //this.PrintMap();


        }
        if (this.EmptyMap[this.Hero.X, this.Hero.Y, this.CurrentLevel] == '#' || this.EmptyMap[this.Hero.X, this.Hero.Y, this.CurrentLevel] == '━' || this.EmptyMap[this.Hero.X, this.Hero.Y, this.CurrentLevel] == '┃')
        {
            this.PrintMap();
            this.PrintLocation(this.Hero.X, this.Hero.Y, this.Hero.DungeonLevel);
        }
        
        return key;
    }
    public void PrintCharSheet(Creature c)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        string[] charsheet = c.ToString().Split('\n');
        for (int i = 0; i < charsheet.Length; i++)
        {
            Console.SetCursorPosition(width - 45, i);
            Console.WriteLine(charsheet[i].PadRight(39));
        }
        Console.SetCursorPosition(width - 45, charsheet.Length);
        Console.WriteLine("".PadRight(45, '*'));
        Console.SetCursorPosition(width - 30, charsheet.Length + 1);
        Console.WriteLine("Movement Controls");
        Console.SetCursorPosition(width - 24, charsheet.Length + 2);
        Console.WriteLine("↖   ↑   ↗");
        Console.SetCursorPosition(width - 24, charsheet.Length + 3);
        Console.WriteLine("  Q W E");
        Console.SetCursorPosition(width - 24, charsheet.Length + 4);
        Console.WriteLine("← A   D →");
        Console.SetCursorPosition(width - 24, charsheet.Length + 5);
        Console.WriteLine("  Z X C");
        Console.SetCursorPosition(width - 24, charsheet.Length + 6);
        Console.WriteLine("↙   ↓   ↘");
        Console.SetCursorPosition(width - 30, charsheet.Length + 7);
        Console.WriteLine("[r]est");
        Console.SetCursorPosition(width - 30, charsheet.Length + 8);
        Console.WriteLine("[p]ick up");
        Console.SetCursorPosition(width - 30, charsheet.Length + 9);
        Console.WriteLine("Climb [s]tairs");
        Console.SetCursorPosition(width - 30, charsheet.Length + 10);
        Console.WriteLine($"You are on level {this.CurrentLevel}");
    }

    public static void PrintLoot(Container<Item> Loot)
    {
        Loot.SortEntries();

        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        string[] Lootable = Loot.ToString().Split('\n');

        Console.SetCursorPosition(width / 2, height / 2 - 1);
        Console.WriteLine("What do you wish to pick up?");

        for (int i = 0; i < Lootable.Length; i++)
        {
            if (Loot.Entries[i] == null)
            {
                break;
            }
            Console.SetCursorPosition(width / 2, height / 2 + i);
            Console.WriteLine($"{Lootable[i]}");
        }
    }

    public void MovementUpdate(Creature c, int LastX, int LastY)
    {
        this.MonsterDict.Remove((LastX, LastY, c.DungeonLevel));
        this.MonsterDict.Add((c.X, c.Y, c.DungeonLevel), c);

        PrintLocation(LastX, LastY, c.DungeonLevel);

        PrintLocation(c.X, c.Y, c.DungeonLevel);
    }

    public void PrintLocation(int i, int j, int Level)
    {
        if ((NearHero(i, j, Level, 5) || ((GetRoom(i, j, Level) != null && GetRoom(i, j, Level).PlayerInRoom()))) && Level == this.Hero.DungeonLevel)
        {
            Console.SetCursorPosition(j, i);

            if (this.MonsterDict.ContainsKey((i, j, this.CurrentLevel)))
            {
                if ((GetRoom(i, j, Level) != null && (GetRoom(i, j, Level).PlayerInRoom()) || GetRoom(i, j, Level).isPway))
                {
                    Console.ForegroundColor = this.MonsterDict[(i, j, Level)].Color;
                    Console.Write((char)this.MonsterDict[(i, j, Level)]);
                }
            }
            else if (this.LootDict.ContainsKey((i, j, Level)))
            {

                char LootChar = this.EmptyMap[i, j, Level]; //Assign the loot char to the floor char
                foreach (KeyValuePair<Item, int> loot in LootDict[(i, j, Level)])
                {
                    Console.ForegroundColor = loot.Key.Color;
                    LootChar = loot.Key.RepresentWith;
                }
                Console.Write(LootChar);
            }
            else
            {
                if (this.Map[i, j, Level] == '━' || this.Map[i, j, Level] == '┃')
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write(this.Map[i, j, this.CurrentLevel]);
            }
            // Console.ResetColor();
        }
        Console.ForegroundColor = default(ConsoleColor);
    }

    public Creature GetCreature(int x, int y, int Level)
    {
        return this.MonsterDict[(x, y, Level)];
    }

    public void UpdateAllRooms(bool Override = false)
    {
        foreach (KeyValuePair<(int, int, int), Room> r in this.RoomDict)
        {
            this.UpdateMap(r.Value, r.Key.Item3, Override);
        }
    }

    public bool NearHero(int X, int Y, int Level, int vision)
    {
        if (Level == this.Hero.DungeonLevel)
        {
            if ((X >= Hero.X - vision && X <= Hero.X + vision) && (Y >= Hero.Y - vision && Y <= Hero.Y + vision))
            {
                return true;
            }
        }
        return false;
    }

    public void PrintMap(bool Override = false)
    {
        foreach (Room r in this.Rooms[this.CurrentLevel])
        {
            this.UpdateMap(r, this.CurrentLevel, Override);
        }
        if (this.CurrentLevel == this.Hero.DungeonLevel)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < this.Map.GetLength(0); i++)
            {
                for (int j = 0; j < this.Map.GetLength(1); j++)
                {
                    Console.SetCursorPosition(j, i);
                    char CharToWrite = ' ';
                    if ((this.GetRoom(i, j, this.CurrentLevel) != null && !this.GetRoom(i, j, this.CurrentLevel).PlayerInRoom() && this.GetRoom(i, j, this.CurrentLevel).Discovered))
                    {
                        CharToWrite = this.EmptyMap[i, j, this.CurrentLevel];
                    }
                    else
                    if ((this.GetRoom(i, j, this.CurrentLevel) != null && this.GetRoom(i, j, this.CurrentLevel).PlayerInRoom()) || Override || NearHero(i, j, this.CurrentLevel, 3))
                    {
                        this.PrintLocation(i, j, this.CurrentLevel);
                        continue;
                    }
                    else if ((this.GetRoom(i, j, this.CurrentLevel) != null && !this.GetRoom(i, j, this.CurrentLevel).PlayerInRoom() && !NearHero(i, j, this.CurrentLevel, 1)))
                    {
                        CharToWrite = ' ';
                    }
                    if (CharToWrite == '━' || CharToWrite == '┃')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(CharToWrite);
                    Console.ForegroundColor = default(ConsoleColor);
                }
                Console.Write("\n");
            }
        }
    }

    public Container<Item> ItemsOnGround(int X, int Y, int Level)
    {
        Container<Item> GroundLoot = new Container<Item>();

        if (this.LootDict.ContainsKey((X, Y, Level)))
        {
            foreach (KeyValuePair<Item, int> loot in LootDict[(X, Y, Level)])
            {
                GroundLoot.Add(loot.Key);
            }
        }

        return GroundLoot;
    }

    public void ItemPickedUp(Item loot, int X, int Y, int Level)
    {
        this.LootDict[(X, Y, Level)].Remove(loot);

        if (this.LootDict[(X, Y, Level)].Count() == 0)
        {
            this.LootDict.Remove((X, Y, Level));
        }
    }

    public void UpdateMap(Room CurrentRoom, int Level, bool Override = false)
    {
        char[,] NewRoom = CurrentRoom.Representation(!this.MapFinalized || Override);
        for (int i = 0; i < CurrentRoom.Width; i++)
        {
            for (int j = 0; j < CurrentRoom.Length; j++)
            {
                this.Map[i + CurrentRoom.X, j + CurrentRoom.Y, Level] = NewRoom[i, j];
                this.EmptyMap[i + CurrentRoom.X, j + CurrentRoom.Y, Level] = NewRoom[i, j];
            }
        }
    }

    //Room Creation
    public void CreateRooms(int TotalRooms, int CurrentRoom = 1)
    {
        do
        {
            bool ValidRoom = true;
            //char[] RoomChars = new char[]{'│', '┌', '┐', '┘', '└', '─', '.'};

            int TopLeftX = Dice.Roll(this.Map.GetLength(0) - 2);
            int TopLeftY = Dice.Roll(this.Map.GetLength(1) - 2);

            int width = Dice.Roll(10) + 5;
            int length = Dice.Roll(10) + 5;

            if (((width + TopLeftX + 1) < this.Map.GetLength(0)) && ((length + TopLeftY + 1) < this.Map.GetLength(1)))
            {
                for (int i = TopLeftX - 1; i < TopLeftX + width + 1; i++) // Check if boarders or overlaps an existing room
                {
                    for (int j = TopLeftY - 1; j < TopLeftY + length + 1; j++)
                    {
                        if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[i, j, this.CurrentLevel]))
                        {
                            ValidRoom = false;
                            i += 99999; //leave outer loop
                            break;      //leave inner loop
                        }
                    }
                }

                if (ValidRoom)
                {
                    this.Rooms[this.CurrentLevel][CurrentRoom] = new Room(TopLeftX, TopLeftY, width, length, this.Hero, this.CurrentLevel);
                    this.RoomDict.Add((TopLeftX, TopLeftY, this.CurrentLevel), this.Rooms[this.CurrentLevel][CurrentRoom]);
                    UpdateMap(this.Rooms[this.CurrentLevel][CurrentRoom], this.CurrentLevel);
                    CurrentRoom++;
                }
            }

        } while (CurrentRoom < TotalRooms);
    }



    //Passageway generation
    public void CreatePasageways(int TotalHallways)
    {
        int passageways = 0;
        int Attempt = 0;
        do
        {
            int StartX = Dice.Roll(this.Map.GetLength(0) - 1);
            int StartY = Dice.Roll(this.Map.GetLength(1) - 1);
            bool ValidPassageway = false;

            // int StartX = Location.Item1;
            // int StartY = Location.Item2;



            if (this.Map[StartX, StartY, this.CurrentLevel] == '━' || this.Map[StartX, StartY, this.CurrentLevel] == '─')
            {
                try
                {
                    if (this.Map[StartX - 1, StartY, this.CurrentLevel] == ' ' || this.Map[StartX - 1, StartY, this.CurrentLevel] == '#') //At the top
                    {
                        int Up = Dice.D6();

                        for (int X = StartX; X >= StartX - Up; X--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[X, StartY, this.CurrentLevel]))
                            {
                                Up -= 1;
                                X = StartX;
                            }
                        }

                        if (Up == 0)
                        {
                            continue;
                        }

                        int Right = 0;

                        for (int EndY = StartY; EndY < this.Map.GetLength(1); EndY++)
                        {
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX - Up, EndY, this.CurrentLevel]))
                            {
                                break;
                            }

                            if (this.Map[StartX - Up, EndY, this.CurrentLevel] == '┃' || this.Map[StartX - Up, EndY, this.CurrentLevel] == '#' || this.Map[StartX - Up, EndY, this.CurrentLevel] == '│')
                            {
                                AddVerticalPasageway(StartX, StartY, Up, Right);
                                ValidPassageway = true;
                                break;
                            }

                            Right += 1;
                        }
                        int Left = 0;
                        for (int EndY = StartY; EndY > 0; EndY--)
                        {
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX - Up, EndY, this.CurrentLevel]))
                            {
                                break;
                            }

                            if (this.Map[StartX - Up, EndY, this.CurrentLevel] == '┃' || this.Map[StartX - Up, EndY, this.CurrentLevel] == '#' || this.Map[StartX - Up, EndY, this.CurrentLevel] == '│')
                            {
                                AddVerticalPasageway(StartX, StartY, Up, Left);
                                ValidPassageway = true;
                                break;
                            }
                            Left -= 1;
                        }
                    }
                    else //At bottom
                    {
                        int Down = Dice.D6();

                        for (int X = StartX; X >= StartX + Down; X++)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[X, StartY, this.CurrentLevel]))
                            {
                                Down -= 1;
                                X = StartX;
                            }
                        }

                        if (Down == 0)
                        {
                            continue;
                        }

                        int Right = 0;
                        for (int EndY = StartY; EndY < this.Map.GetLength(1); EndY++)
                        {
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX + Down, EndY, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[StartX + Down, EndY, this.CurrentLevel] == '┃' || this.Map[StartX + Down, EndY, this.CurrentLevel] == '#' || this.Map[StartX + Down, EndY, this.CurrentLevel] == '│')
                            {
                                AddVerticalPasageway(StartX, StartY, Down * -1, Right);
                                ValidPassageway = true;
                                break;
                            }
                            Right += 1;
                        }
                        int Left = 0;
                        for (int EndY = StartY; EndY > 0; EndY--)
                        {
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX + Down, EndY, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[StartX + Down, EndY, this.CurrentLevel] == '┃' || this.Map[StartX + Down, EndY, this.CurrentLevel] == '#' || this.Map[StartX + Down, EndY, this.CurrentLevel] == '│')
                            {
                                AddVerticalPasageway(StartX, StartY, Down * -1, Left);
                                ValidPassageway = true;
                                break;
                            }
                            Left -= 1;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                { continue; }
            }

            if (this.Map[StartX, StartY, this.CurrentLevel] == '┃' || this.Map[StartX, StartY, this.CurrentLevel] == '│')
            {
                try
                {
                    if (this.Map[StartX, StartY - 1, this.CurrentLevel] == ' ' || this.Map[StartX, StartY - 1, this.CurrentLevel] == '#') //At the left
                    {
                        int Left = Dice.D6();
                        for (int Y = StartY; Y > StartY - Left; Y--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX, Y, this.CurrentLevel]))
                            {
                                Left -= 1;
                                Y = StartY;
                            }
                        }

                        if (Left == 0)
                        {
                            continue;
                        }

                        int Up = 0; //Build hall up

                        for (int EndX = StartX; EndX >= 0; EndX--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY - Left, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY - Left, this.CurrentLevel] == '─' || this.Map[EndX, StartY - Left, this.CurrentLevel] == '#' || this.Map[EndX, StartY - Left, this.CurrentLevel] == '━')
                            {
                                AddHorizontalPasageway(StartX, StartY, Up, Left * -1);
                                ValidPassageway = true;
                                break;
                            }
                            Up += 1;
                        }

                        int Down = 0; //build hall down

                        for (int EndX = StartX; EndX <= this.Map.GetLength(0); EndX++)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY - Left, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY - Left, this.CurrentLevel] == '─' || this.Map[EndX, StartY - Left, this.CurrentLevel] == '━' || this.Map[EndX, StartY - Left, this.CurrentLevel] == '#')
                            {
                                AddHorizontalPasageway(StartX, StartY, Down, Left * -1);
                                ValidPassageway = true;
                                break;
                            }
                            Down -= 1;
                        }
                    }
                    else //At Right
                    {
                        int Right = Dice.D6();
                        for (int Y = StartY; Y < StartY + Right; Y++)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX, Y, this.CurrentLevel]))
                            {
                                Right -= 1;
                                Y = StartY;
                            }
                        }

                        if (Right == 0)
                        {
                            continue;
                        }

                        int Up = 0; //Build hall up

                        for (int EndX = StartX; EndX >= 0; EndX--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY + Right, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY + Right, this.CurrentLevel] == '─' || this.Map[EndX, StartY + Right, this.CurrentLevel] == '━' || this.Map[EndX, StartY + Right, this.CurrentLevel] == '#')
                            {
                                AddHorizontalPasageway(StartX, StartY, Up, Right);
                                ValidPassageway = true;
                                break;
                            }
                            Up += 1;
                        }

                        int Down = 0; //build hall down

                        for (int EndX = StartX; EndX <= this.Map.GetLength(0); EndX++)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY + Right, this.CurrentLevel]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY + Right, this.CurrentLevel] == '─' || this.Map[EndX, StartY + Right, this.CurrentLevel] == '━' || this.Map[EndX, StartY + Right, this.CurrentLevel] == '#')
                            {
                                AddHorizontalPasageway(StartX, StartY, Down, Right);
                                ValidPassageway = true;
                                break;
                            }
                            Down -= 1;
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                { continue; }
            }
            if (ValidPassageway)
            {
                passageways += 1;
                this.UpdateAllRooms();
                // Console.WriteLine($"Passageway {passageways} completed at {StartX}, {StartY}");
                // PrintGrid(grid);
                // char Waiting = Console.ReadKey(true).KeyChar;
                //Console.Clear();
            }
            Attempt += 1;
            if (Attempt > 10)
            {
                break;
            }
        } while (passageways < TotalHallways);
    }

    public Room GetRoom(int X, int Y, int Level)
    {
        for (int i = 0; i < this.Rooms[Level].Length; i++)
        {
            if (this.Rooms[Level][i].IsInRoom(X, Y, Level))
            {
                return this.Rooms[Level][i];
            }
        }
        if (this.RoomDict.Keys.Contains((X, Y, Level)))
        {
            return this.RoomDict[(X, Y, Level)];
        }
        return null;
    }

    public void DebugPrint(int X, int Y, int Level)
    {
        for (int k = 0; k < 3; k++)
        {
            for (int i = 0; i < this.Map.GetLength(0); i++)
            {
                for (int j = 0; j < this.Map.GetLength(1); j++)
                {
                    Console.Write(this.Map[i, j, k]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("=================================================");
            Console.WriteLine();
        }
    }

    public void AddVerticalPasageway(int StartX, int StartY, int Up, int Right)
    {
        this.GetRoom(StartX, StartY, this.CurrentLevel).AddDoor(StartX, StartY);
        // try {
        if (this.Map[StartX - Up, StartY + Right, this.CurrentLevel] != '#')
            this.GetRoom(StartX - Up, StartY + Right, this.CurrentLevel).AddDoor(StartX - Up, StartY + Right);
        // }catch(NullReferenceException)
        // {}
        if (Up > 0)
        {
            for (int X = StartX - 1; X >= StartX - Up; X--)
            {
                // this.Map[X, StartY] = '#';
                this.RoomDict.TryAdd((X, StartY, this.CurrentLevel), new Room(X, StartY, 1, 1, this.Hero, this.CurrentLevel));
            }

            if (Right > 0)
            {
                for (int Y = StartY + 1; Y < StartY + Right; Y++)
                {
                    // this.Map[StartX - Up, Y] = '#';
                    this.RoomDict.TryAdd((StartX - Up, Y, this.CurrentLevel), new Room(StartX - Up, Y, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
            else //Left
            {
                int Left = Math.Abs(Right);

                for (int Y = StartY - 1; Y > StartY - Left; Y--)
                {
                    // this.Map[StartX - Up, Y] = '#';
                    this.RoomDict.TryAdd((StartX - Up, Y, this.CurrentLevel), new Room(StartX - Up, Y, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
        }
        else //Down
        {
            int Down = Math.Abs(Up);

            for (int X = StartX + 1; X <= StartX + Down; X++)
            {
                // this.Map[X, StartY] = '#';
                this.RoomDict.TryAdd((X, StartY, this.CurrentLevel), new Room(X, StartY, 1, 1, this.Hero, this.CurrentLevel));
            }

            if (Right > 0)
            {
                for (int Y = StartY + 1; Y < StartY + Right; Y++)
                {
                    // this.Map[StartX + Down, Y] = '#';
                    this.RoomDict.TryAdd((StartX + Down, Y, this.CurrentLevel), new Room(StartX + Down, Y, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
            else //Left
            {
                int Left = Math.Abs(Right);

                for (int Y = StartY - 1; Y > StartY - Left; Y--)
                {
                    // this.Map[StartX + Down, Y] = '#';
                    this.RoomDict.TryAdd((StartX + Down, Y, this.CurrentLevel), new Room(StartX + Down, Y, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
        }
    }

    public void AddHorizontalPasageway(int StartX, int StartY, int Up, int Right)
    {
        try
        {
            this.GetRoom(StartX, StartY, this.CurrentLevel).AddDoor(StartX, StartY);

            this.GetRoom(StartX - Up, StartY + Right, this.CurrentLevel).AddDoor(StartX - Up, StartY + Right);
        }
        catch (NullReferenceException)
        {
            this.DebugPrint(0, 0, 0);
        }

        if (Right > 0)
        {
            for (int Y = StartY + 1; Y <= StartY + Right; Y++)
            {
                this.RoomDict.TryAdd((StartX, Y, this.CurrentLevel), new Room(StartX, Y, 1, 1, this.Hero, this.CurrentLevel));
                // this.Map[StartX, Y] = '#';
            }

            if (Up > 0)
            {
                for (int X = StartX - 1; X > StartX - Up; X--)
                {
                    // this.Map[X, StartY + Right] = '#';
                    this.RoomDict.TryAdd((X, StartY + Right, this.CurrentLevel), new Room(X, StartY + Right, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
            else //Down
            {
                int Down = Math.Abs(Up);

                for (int X = StartX + 1; X < StartX + Down; X++)
                {
                    // this.Map[X, StartY + Right] = '#';
                    this.RoomDict.TryAdd((X, StartY + Right, this.CurrentLevel), new Room(X, StartY + Right, 1, 1, this.Hero, this.CurrentLevel));
                }
            }
        }
        else //Left
        {
            int Left = Math.Abs(Right);

            for (int Y = StartY - 1; Y >= StartY - Left; Y--)
            {
                this.RoomDict.TryAdd((StartX, Y, this.CurrentLevel), new Room(StartX, Y, 1, 1, this.Hero, this.CurrentLevel));
                // this.Map[StartX, Y] = '#';
            }

            if (Up > 0)
            {
                for (int X = StartX - 1; X >= StartX - Up; X--)
                {
                    this.RoomDict.TryAdd((X, StartY - Left, this.CurrentLevel), new Room(X, StartY - Left, 1, 1, this.Hero, this.CurrentLevel));
                    // this.Map[X, StartY - Left] = '#';
                }
            }
            else //Down
            {
                int Down = Math.Abs(Up);

                for (int X = StartX + 1; X <= StartX + Down; X++)
                {
                    this.RoomDict.TryAdd((X, StartY - Left, this.CurrentLevel), new Room(X, StartY - Left, 1, 1, this.Hero, this.CurrentLevel));
                    // this.Map[X, StartY - Left] = '#';
                }
            }
        }
    }
    public bool IsWall(char C)
    {
        return !(new char[] { '│', '─', '┌', '┐', '┘', '└', '.' }.Contains(C));
    }

    public bool Navigatable()
    {
        this.UpdateAllRooms(true);

        Bandit B = new Bandit();
        int StartX = this.Rooms[this.CurrentLevel][0].X + 3;
        int StartY = this.Rooms[this.CurrentLevel][0].Y + 3;
        B.GoTo(StartX, StartY);


        char[,] WalkableMap = new char[this.Length, this.Width];

        this.MonsterDict.Add((B.X, B.Y, this.CurrentLevel), B);
        int Moves = 0;
        do
        {
            Moves = 0;
            Dictionary<(int, int, int), Creature> WalkerDict = new Dictionary<(int, int, int), Creature>();

            foreach (KeyValuePair<(int, int, int), Creature> pair in this.MonsterDict)
            {
                WalkerDict.Add(pair.Key, pair.Value);
            }

            foreach (Creature Walker in WalkerDict.Values)
            {
                while (this.MovementAllowed(Walker.X - 1, Walker.Y, this.CurrentLevel))
                {
                    Walker.MoveNorth();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y, this.CurrentLevel];

                    Bandit WN = new Bandit();
                    WN.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y, this.CurrentLevel), WN);
                }

                while (this.MovementAllowed(Walker.X, Walker.Y - 1, this.CurrentLevel))
                {
                    Walker.MoveWest();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y, this.CurrentLevel];

                    Bandit WW = new Bandit();
                    WW.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y, this.CurrentLevel), WW);
                }

                while (this.MovementAllowed(Walker.X + 1, Walker.Y, this.CurrentLevel))
                {
                    Walker.MoveSouth();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y, this.CurrentLevel];

                    Bandit WS = new Bandit();
                    WS.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y, this.CurrentLevel), WS);
                }

                while (this.MovementAllowed(Walker.X, Walker.Y + 1, this.CurrentLevel))
                {
                    Walker.MoveEast();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y, this.CurrentLevel];

                    Bandit WE = new Bandit();
                    WE.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y, this.CurrentLevel), WE);
                }
            }
        } while (Moves > 0);

        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                if (this.Map[i, j, this.CurrentLevel] == '.' && !this.MonsterDict.ContainsKey((i, j, this.CurrentLevel)))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
