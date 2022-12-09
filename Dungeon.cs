using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Dungeon
{

    public char[,] Map;
    public char[,] EmptyMap;
    public Dictionary<(int, int), Creature> MonsterDict;
    public PlayerCharacter Hero;
    public Dictionary<(int, int), Dictionary<Item, int>> LootDict;
    public Room[] Rooms;
    public Dictionary<(int, int), Room> RoomDict;
    public int Width;
    public int Length;
    private bool MapFinalized;
    public string[] MessageHistory;

    public Dungeon(PlayerCharacter c, int X = 60, int Y = 60, int Rooms = 10)
    {
        this.Hero = c;
        this.Length = X;
        this.Width = Y;
        this.MapFinalized = false;
        this.Map = new char[X, Y];
        FillMap();
        this.EmptyMap = (char[,])this.Map.Clone();
        this.Rooms = new Room[Rooms];
        this.RoomDict = new Dictionary<(int, int), Room>();
        this.CreateRooms(Rooms);
        //this.CreatePasageways(12);
        this.MonsterDict = new Dictionary<(int, int), Creature>();
        this.LootDict = new Dictionary<(int, int), Dictionary<Item, int>>(); //Dictionary of Loot on floor of the dungeon Dict( X, Y : Dict(Item : NumOfType))

        this.EmptyMap = (char[,])this.Map.Clone();
        while (!this.Navigatable())
        {
            this.UpdateAllRooms(true);
            this.CreatePasageways(2);
            this.EmptyMap = (char[,])this.Map.Clone();
            //this.PrintMap();
            this.MonsterDict = new Dictionary<(int, int), Creature>();
        }
        this.MapFinalized = true;

        this.UpdateAllRooms(true);

        this.EmptyMap = (char[,])this.Map.Clone();
        this.MonsterDict = new Dictionary<(int, int), Creature>();
        this.LootDict = new Dictionary<(int, int), Dictionary<Item, int>>(); //Dictionary of Loot on floor of the dungeon Dict( X, Y : Dict(Item : NumOfType))
        this.MessageHistory = new string[5]{"0","1","2","3","4"};
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
        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                this.Map[i, j] = ' ';
            }
        }
    }

    // I WARNED YOU ABOUT THE STAIRS BRO;
    public void PlaceHeroInRoom(PlayerCharacter p)
    {
        this.Hero = p;
        PlaceCreatureInRoom(this.Hero);
    }

    public void PlaceCreatureInRoom(Creature c = null)
    {
        int X;
        int Y;
        do
        {
            X = Dice.Roll(this.Map.GetLength(0) - 1);
            Y = Dice.Roll(this.Map.GetLength(1) - 1);
        } while (this.MonsterDict.ContainsKey((X, Y)) || this.Map[X, Y] != '.'); //Rolls dice for location
        if (c == null)
        {
            int MonsterRoll = Dice.D20();
            if (MonsterRoll <= 5)
            {
                this.MonsterDict.Add((X, Y), new GiantRat());
            }
            else if (MonsterRoll <= 10)
            {
                this.MonsterDict.Add((X, Y), new Bandit());
            }
            else if (MonsterRoll <= 15)
            {
                this.MonsterDict.Add((X, Y), new Hobgoblin());
            }
            else if (MonsterRoll <= 19)
            {
                this.MonsterDict.Add((X, Y), new Orc());
            }
            else if (MonsterRoll == 20)
            {
                this.MonsterDict.Add((X, Y), new Mimic());
            }
        }
        else
        {
            this.MonsterDict.Add((X, Y), c);
        }

        this.MonsterDict[(X, Y)].GoTo(X, Y);
        this.MonsterDict[(X, Y)].RollInitiative();
    }

    public bool MovementAllowed(int X, int Y, Creature c)
    {
        
        if (new char[] { '│', '┌', '┐', '┘', '└', '─', ' ' }.Contains(this.Map[X, Y]))
        {
            return false;
        }
        if (this.MonsterDict.ContainsKey((X, Y)))
        {
            if (this.MonsterDict[(X, Y)].IsAlive)
            {
                PrintFightLog(c, this.MonsterDict[(X, Y)]);
                return false;
            }else
            {
                this.MonsterDict.Remove((X,Y));
            }
        }
        if (this.Map[X, Y] == '━' || this.Map[X, Y] == '┃')
        {
            return true;
        }
        return true;
    }

    public bool MovementAllowed(int X, int Y)
    {
        if (new char[] { '│', '┌', '┐', '┘', '└', '─', ' ' }.Contains(this.Map[X, Y]))
        {
            return false;
        }
        if (this.MonsterDict.ContainsKey((X, Y)))
        {
            return false;
        }
        return true;
    }

    public void PrintFightLog(Creature a, Creature b)
    {
        if(a.RepresentWith == b.RepresentWith)
        {
            return;
        }
        
        //Creature[] fighters = { a, b };
        //Array.Sort(fighters);
        //Array.Reverse(fighters);
        Console.SetCursorPosition(0, this.Length);
        if (a is PlayerCharacter || b is PlayerCharacter)
        {
            if(a.RepresentWith == 'C')
            {
                // Console.WriteLine("As you reach to see what treasures are locked away...".PadRight(Console.WindowWidth));
                // Console.WriteLine($"{a.Attack(b).PadRight(Console.WindowWidth)}");

                this.NewMessage("As you reach to see what treasures are locked away...");
                this.NewMessage(a.Attack(b));
                this.PrintMessageHistory();
                return;
            }
            if(b.RepresentWith == 'C')
            {
                // Console.WriteLine("As you reach to see what treasures are locked away...".PadRight(Console.WindowWidth));
                // Console.WriteLine($"{b.Attack(a).PadRight(Console.WindowWidth)}");

                this.NewMessage("As you reach to see what treasures are locked away...");
                this.NewMessage(b.Attack(a));
                this.PrintMessageHistory();
                return;
            }

            // Console.WriteLine($"{fighters[0].Attack(fighters[1]).PadRight(Console.WindowWidth)}");
            this.NewMessage(a.Attack(b));
            // if (fighters[1].HP > 0) //Benefit of higher initiative
            // {
            //     // Console.WriteLine($"{fighters[1].Attack(fighters[0]).PadRight(Console.WindowWidth)}");
            //     this.NewMessage(fighters[1].Attack(fighters[0]));
            // }
            // else
            // {
            //     fighters[0].GainXP(fighters[1].XPGiven);
            //     // Console.SetCursorPosition(0, this.Length + 1);
            //     // Console.WriteLine($"{fighters[0]} gained {fighters[1].XPGiven} XP! And is now level {fighters[0].Level}!".PadRight(Console.WindowWidth));
            //     //Console.WriteLine("".PadRight(Console.WindowWidth));
            // }
            // if (fighters[0].HP <= 0)
            // {
            //     fighters[1].GainXP(fighters[0].XPGiven);
            //     // Console.SetCursorPosition(0, this.Length + 2);
            //     // Console.WriteLine($"{fighters[1]} gained {fighters[0].XPGiven} XP! And is now level {fighters[1].Level}!".PadRight(Console.WindowWidth));
            //     //Console.WriteLine("".PadRight(Console.WindowWidth));
            // }
            this.PostFightUpdate(a);
            this.PostFightUpdate(b);
        }
        else
        {
            a.Attack(b);
            
            this.NewMessage("You hear fighting in the distance");
            
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
                //fighters[1].Attack(fighters[0]);
            }
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
                this.AddToLootDict(c.EquippedWeapon, c.X, c.Y);
            }

            for (int i = 0; i < c.Inventory.Entries.Length; i++)
            {
                if (c.Inventory.Entries[i] != null)
                {
                    Item loot = c.RemoveItem(i);
                    this.AddToLootDict(loot, c.X, c.Y);
                }
            }

            this.MonsterDict.Remove((c.X, c.Y));
            this.PrintLocation(c.X, c.Y);

            
        }
        PrintLocation(c.X, c.Y);
    }

    public void AddToLootDict(Item loot, int X, int Y)
    {
        if (this.LootDict.ContainsKey((X, Y)))
        {
            if (this.LootDict[(X, Y)].ContainsKey(loot))
            {
                this.LootDict[(X, Y)][loot] += 1; //stack if multiple of same items
            }
            else
            {
                this.LootDict[(X, Y)].Add(loot, 1);
            }
        }
        else
        {
            Dictionary<Item, int> TempDict = new Dictionary<Item, int> { { loot, 1 } };
            this.LootDict.Add((X, Y), TempDict);
        }
    }

    public ConsoleKeyInfo MonsterTurn()
    {
        ConsoleKeyInfo keyInfo = default(ConsoleKeyInfo);
        foreach (KeyValuePair<(int, int), Creature> Mob in this.MonsterDict.OrderByDescending(key => key.Value))
        {
            if (Mob.Value is PlayerCharacter)
            {
                keyInfo = this.PlayerTurn();
            }
            else
            {
                MonsterMove(Mob.Value);
            }
        }
        return keyInfo;
    }

    public void MonsterMove(Creature c)
    {
        if (!c.IsAlive)
        {
            this.MonsterDict.Remove((c.X, c.Y));
            return;
        }
        if (c is Mimic)
        {
            this.PrintLocation(c.X, c.Y);

            if (c.RepresentWith == 'M' && this.NearHero(c.X, c.Y, 1))
            {
                this.PrintFightLog(c, this.Hero);
            }
            return;
        }
        int Move = Dice.D8();
        int lastX = c.X;
        int lastY = c.Y;
        if (Move == 1)
        {
            if (this.MovementAllowed(c.X - 1, c.Y, c))
            { c.MoveNorth(); }
        }
        else
        if (Move == 2)
        {
            if (this.MovementAllowed(c.X, c.Y - 1, c))
            { c.MoveWest(); }
        }
        else
        if (Move == 3)
        {
            if (this.MovementAllowed(c.X + 1, c.Y, c))
            { c.MoveSouth(); }
        }
        else
        if (Move == 4)
        {
            if (this.MovementAllowed(c.X, c.Y + 1, c))
            { c.MoveEast(); }
        }
        else
        if (Move == 5)
        {
            if (this.MovementAllowed(c.X - 1, c.Y - 1, c))
            { c.MoveNorthWest(); }
        }
        else
        if (Move == 6)
        {
            if (this.MovementAllowed(c.X - 1, c.Y + 1, c))
            { c.MoveNorthEast(); }
        }
        else
        if (Move == 7)
        {
            if (this.MovementAllowed(c.X + 1, c.Y - 1, c))
            { c.MoveSouthWest(); }
        }
        else
        if (Move == 8)
        {
            if (this.MovementAllowed(c.X + 1, c.Y + 1, c))
            { c.MoveSouthEast(); }
        }
        if (lastX != c.X || lastY != c.Y)
        {
            this.MovementUpdate(c, lastX, lastY);
        }else
        {
            this.PrintLocation(c.X, c.Y);
        }
    }

    public ConsoleKeyInfo PlayerTurn()
    {
        if(this.Hero.HP <= 0)
        {
            return default(ConsoleKeyInfo);
        }
        
        ConsoleKeyInfo keyInfo;
        Console.SetCursorPosition(0, Console.WindowHeight - 20);
        keyInfo = Console.ReadKey(true);
        int lastX = this.Hero.X;
        int lastY = this.Hero.Y;
        if (keyInfo.KeyChar == 'w')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y,this.Hero))
            { this.Hero.MoveNorth(); }
        }
        else
        if (keyInfo.KeyChar == 'a')
        {
            if (this.MovementAllowed(this.Hero.X, this.Hero.Y - 1,this.Hero))
            { this.Hero.MoveWest(); }
        }
        else
        if (keyInfo.KeyChar == 'x')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y, this.Hero))
            { this.Hero.MoveSouth(); }
        }
        else
        if (keyInfo.KeyChar == 'd')
        {
            if (this.MovementAllowed(this.Hero.X, this.Hero.Y + 1, this.Hero))
            { this.Hero.MoveEast(); }
        }
        else
        if (keyInfo.KeyChar == 'q')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y - 1, this.Hero))
            { this.Hero.MoveNorthWest(); }
        }
        else
        if (keyInfo.KeyChar == 'e')
        {
            if (this.MovementAllowed(this.Hero.X - 1, this.Hero.Y + 1, this.Hero))
            { this.Hero.MoveNorthEast(); }
        }
        else
        if (keyInfo.KeyChar == 'z')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y - 1, this.Hero))
            { this.Hero.MoveSouthWest(); }
        }
        else
        if (keyInfo.KeyChar == 'c')
        {
            if (this.MovementAllowed(this.Hero.X + 1, this.Hero.Y + 1, this.Hero))
            { this.Hero.MoveSouthEast(); }
        }
        else
        if (keyInfo.KeyChar == 'r')
        {
            // **ToDo**
            // Make rest have charges
            Console.WriteLine(this.Hero.Rest());
        }
        else
        if (keyInfo.KeyChar == 'i')
        {
            // **ToDo**
            // Inventory managment, drop items,
            
        }
        else
        if (keyInfo.KeyChar == 'n') //Dungeon building test remove in future
        {
            this.MonsterTurn();
        }
        else
        if (keyInfo.KeyChar == 'p')
        {
            if (this.LootDict.ContainsKey((this.Hero.X, this.Hero.Y)))
            {
                
            
            //Print out items on the ground
            Container<Item> Loot = this.ItemsOnGround(this.Hero.X, this.Hero.Y);
            PrintLoot(Loot);
            //Take input
            int input = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
            //Pick up item selected
            try
            {
                if (Loot.Entries[input] != null)
                {
                    this.Hero.PickUpItem(Loot.Entries[input]);
                    this.ItemPickedUp(Loot.Entries[input], this.Hero.X, this.Hero.Y);
                }
            }
            catch (IndexOutOfRangeException)
            {

            }
            Console.Clear();
            this.PrintMap();
            }
        }
        else
        {
            int input = (int)Char.GetNumericValue(keyInfo.KeyChar) - 1;
            if (input >= 0 && this.Hero.Inventory.Entries[input] != null)
            {
                Console.Clear();

                PrintCharSheet(this.Hero);
                this.PrintMap();

                Console.WriteLine(this.Hero.Inventory.Entries[input].Use(this.Hero, input));
            }
        }
        

        this.Map[lastX, lastY] = this.EmptyMap[lastX, lastY];
        this.Map[this.Hero.X, this.Hero.Y] = (char)this.Hero;

        if (lastX != this.Hero.X || lastY != this.Hero.Y)
        {
            this.ClearMessages();
            this.MovementUpdate(this.Hero, lastX, lastY);
            //Console.Clear();
            //this.PrintMap();
            

        }
        if (this.EmptyMap[this.Hero.X, this.Hero.Y] == '#' || this.EmptyMap[this.Hero.X, this.Hero.Y] == '━' || this.EmptyMap[this.Hero.X, this.Hero.Y] == '┃')
        {
            this.PrintMap();
            this.PrintLocation(this.Hero.X, this.Hero.Y);
        }
        return keyInfo;
    }
    public void PrintCharSheet(Creature c)
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        string[] charsheet = c.ToString().Split('\n');
        for (int i = 0; i < charsheet.Length; i++)
        {
            Console.SetCursorPosition(width - 40, i);
            Console.WriteLine(charsheet[i].PadRight(39));
        }
        Console.SetCursorPosition(width - 40, charsheet.Length);
        Console.WriteLine("".PadRight(40,'*'));
        Console.SetCursorPosition(width - 30, charsheet.Length+1);
        Console.WriteLine("Movement Controls");
        Console.SetCursorPosition(width - 24, charsheet.Length+2);
        Console.WriteLine("↖   ↑   ↗");
        Console.SetCursorPosition(width - 24, charsheet.Length+3);
        Console.WriteLine("  Q W E");
        Console.SetCursorPosition(width - 24, charsheet.Length+4);
        Console.WriteLine("← A   D →");
        Console.SetCursorPosition(width - 24, charsheet.Length+5);
        Console.WriteLine("  Z X C");
        Console.SetCursorPosition(width - 24, charsheet.Length+6);
        Console.WriteLine("↙   ↓   ↘");
        Console.SetCursorPosition(width - 30, charsheet.Length+7);
        Console.WriteLine("[r]est");
        Console.SetCursorPosition(width - 30, charsheet.Length+8);
        Console.WriteLine("[p]ick up");
    }

    public static void PrintLoot(Container<Item> Loot)
    {
        Loot.SortEntries();

        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        string[] Lootable = Loot.ToString().Split('\n');

        Console.SetCursorPosition(width/2, height/2 - 1);
        Console.WriteLine("What do you wish to pick up?");

        for (int i = 0; i < Lootable.Length; i++)
        {
            if (Loot.Entries[i] == null)
            {
                break;
            }
            Console.SetCursorPosition(width/2, height/2 + i);
            Console.WriteLine($"{Lootable[i]}");
        }
        
        
    }


    public void MovementUpdate(Creature c, int LastX, int LastY)
    {
        this.MonsterDict.Remove((LastX, LastY));
        this.MonsterDict.Add((c.X, c.Y), c);
        
        PrintLocation(LastX, LastY);
        
        PrintLocation(c.X, c.Y);
    }

    public void PrintLocation(int i, int j)
    {
        if (NearHero(i, j, 5) || ((GetRoom(i, j) != null && GetRoom(i, j).PlayerInRoom())))
        {
            Console.SetCursorPosition(j, i);

            if (this.MonsterDict.ContainsKey((i, j)))
            {
                if ((GetRoom(i, j) != null && (GetRoom(i, j).PlayerInRoom()) || GetRoom(i, j).isPway))
                {
                    Console.ForegroundColor = this.MonsterDict[(i,j)].Color;
                    Console.Write((char)this.MonsterDict[(i, j)]);
                }
            }
            else if (this.LootDict.ContainsKey((i, j)))
            {
                
                char LootChar = this.EmptyMap[i, j]; //Assign the loot char to the floor char
                foreach (KeyValuePair<Item, int> loot in LootDict[(i, j)])
                {
                    Console.ForegroundColor = loot.Key.Color;
                    LootChar = loot.Key.RepresentWith;
                }
                Console.Write(LootChar);
            }
            else
            {
                if (this.Map[i, j] == '━' || this.Map[i, j] == '┃')
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write(this.Map[i, j]);
            }
           // Console.ResetColor();
        }
        Console.ForegroundColor = default(ConsoleColor);
    }

    public Creature GetCreature(int x, int y)
    {
        return this.MonsterDict[(x, y)];
    }

    public void UpdateAllRooms(bool Override = false)
    {
        foreach (Room r in this.RoomDict.Values)
        {
            this.UpdateMap(r, Override);
        }
    }

    public bool NearHero(int X, int Y, int vision)
    {
        if ((X >= Hero.X - vision && X <= Hero.X + vision) && (Y >= Hero.Y - vision && Y <= Hero.Y + vision))
        {
            return true;
        }
        return false;
    }

    public void PrintMap(bool Override = false)
    {
        foreach (Room r in this.Rooms)
        {
            this.UpdateMap(r, Override);
        }

        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                Console.SetCursorPosition(j, i);
                char CharToWrite = ' ';
                if ((this.GetRoom(i, j) != null && this.GetRoom(i, j).PlayerInRoom()) || Override || NearHero(i, j, 3))
                {
                    if (this.MonsterDict.ContainsKey((i, j)))
                    {
                        if ((this.GetRoom(i, j).PlayerInRoom()) || this.GetRoom(i, j).isPway)
                        {
                            Console.ForegroundColor = this.MonsterDict[(i, j)].Color;
                            CharToWrite = (char)this.MonsterDict[(i, j)];
                        }

                    }
                    else if (this.LootDict.ContainsKey((i, j)))
                    {
                        foreach (KeyValuePair<Item, int> loot in LootDict[(i, j)])
                        {
                            Console.ForegroundColor = loot.Key.Color;
                            CharToWrite = loot.Key.RepresentWith;
                        }
                    }
                    else
                    {
                        
                        CharToWrite = this.Map[i, j];
                    }
                }

                if ((this.GetRoom(i, j) != null && !this.GetRoom(i, j).PlayerInRoom() && this.GetRoom(i, j).Discovered))
                {
                    //Console.ResetColor();
                    CharToWrite = this.EmptyMap[i, j];
                }
                else if ((this.GetRoom(i, j) != null && !this.GetRoom(i, j).PlayerInRoom() && !NearHero(i, j, 1)))
                {
                    //Console.ResetColor();
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

    

    public void PrintLootAtLocation(int X, int Y)
    {
        if (this.LootDict.ContainsKey((X, Y)))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                char LootChar = this.EmptyMap[X, Y]; //Assign the loot char to the floor char
                foreach (KeyValuePair<Item, int> loot in LootDict[(X, Y)])
                {
                    Console.ForegroundColor = loot.Key.Color;
                    LootChar = loot.Key.RepresentWith;
                }
                Console.Write(LootChar);
            }
    }

    public Container<Item> ItemsOnGround(int X, int Y)
    {
        Container<Item> GroundLoot = new Container<Item>();

        if (this.LootDict.ContainsKey((X, Y)))
        {
            foreach (KeyValuePair<Item, int> loot in LootDict[(X, Y)])
            {
                GroundLoot.Add(loot.Key);
            }
        }

        return GroundLoot;
    }

    public void ItemPickedUp(Item loot, int X, int Y)
    {
        this.LootDict[(X, Y)].Remove(loot);
        
        if(this.LootDict[(X, Y)].Count() == 0)
        {
            this.LootDict.Remove((X, Y));
        }
    }

    //Room Creation
    public void CreateRooms(int TotalRooms)
    {
        int CurrentRoom = 0;
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
                        if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[i, j]))
                        {
                            ValidRoom = false;
                            i += 99999; //leave outer loop
                            break;      //leave inner loop
                        }
                    }
                }

                if (ValidRoom)
                {
                    this.Rooms[CurrentRoom] = new Room(TopLeftX, TopLeftY, width, length, Dice.D4(), this.Hero);
                    this.RoomDict.Add((TopLeftX, TopLeftY), this.Rooms[CurrentRoom]);
                    UpdateMap(this.Rooms[CurrentRoom]);
                    CurrentRoom++;

                }
            }

        } while (CurrentRoom < TotalRooms);
    }

    public void UpdateMap(Room CurrentRoom, bool Override = false)
    {
        char[,] NewRoom = CurrentRoom.Representation(!this.MapFinalized || Override);
        for (int i = 0; i < CurrentRoom.Width; i++)
        {
            for (int j = 0; j < CurrentRoom.Length; j++)
            {
                this.Map[i + CurrentRoom.X, j + CurrentRoom.Y] = NewRoom[i, j];
                this.EmptyMap[i + CurrentRoom.X, j + CurrentRoom.Y] = NewRoom[i, j];
            }
        }
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



            if (this.Map[StartX, StartY] == '━' || this.Map[StartX, StartY] == '─')
            {
                try
                {
                    if (this.Map[StartX - 1, StartY] == ' ' || this.Map[StartX - 1, StartY] == '#') //At the top
                    {
                        int Up = Dice.D6();

                        for (int X = StartX; X >= StartX - Up; X--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[X, StartY]))
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
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX - Up, EndY]))
                            {
                                break;
                            }

                            if (this.Map[StartX - Up, EndY] == '┃' || this.Map[StartX - Up, EndY] == '#' || this.Map[StartX - Up, EndY] == '│')
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
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX - Up, EndY]))
                            {
                                break;
                            }

                            if (this.Map[StartX - Up, EndY] == '┃' || this.Map[StartX - Up, EndY] == '#' || this.Map[StartX - Up, EndY] == '│')
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[X, StartY]))
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
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX + Down, EndY]))
                            {
                                break;
                            }
                            if (this.Map[StartX + Down, EndY] == '┃' || this.Map[StartX + Down, EndY] == '#' || this.Map[StartX + Down, EndY] == '│')
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
                            if (new char[] { '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX + Down, EndY]))
                            {
                                break;
                            }
                            if (this.Map[StartX + Down, EndY] == '┃' || this.Map[StartX + Down, EndY] == '#' || this.Map[StartX + Down, EndY] == '│')
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

            if (this.Map[StartX, StartY] == '┃' || this.Map[StartX, StartY] == '│')
            {
                try
                {
                    if (this.Map[StartX, StartY - 1] == ' ' || this.Map[StartX, StartY - 1] == '#') //At the left
                    {
                        int Left = Dice.D6();
                        for (int Y = StartY; Y > StartY - Left; Y--)
                        {
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX, Y]))
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY - Left]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY - Left] == '─' || this.Map[EndX, StartY - Left] == '#' || this.Map[EndX, StartY - Left] == '━')
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY - Left]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY - Left] == '─' || this.Map[EndX, StartY - Left] == '━' || this.Map[EndX, StartY - Left] == '#')
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '─', '.' }.Contains(this.Map[StartX, Y]))
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY + Right]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY + Right] == '─' || this.Map[EndX, StartY + Right] == '━' || this.Map[EndX, StartY + Right] == '#')
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
                            if (new char[] { '│', '┌', '┐', '┘', '└', '.' }.Contains(this.Map[EndX, StartY + Right]))
                            {
                                break;
                            }
                            if (this.Map[EndX, StartY + Right] == '─' || this.Map[EndX, StartY + Right] == '━' || this.Map[EndX, StartY + Right] == '#')
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

    public Room GetRoom(int X, int Y)
    {
        foreach (Room r in this.Rooms)
        {
            if (r.IsInRoom(X, Y))
            {
                return r;
            }
        }
        if (this.RoomDict.Keys.Contains((X, Y)))
        {
            return this.RoomDict[(X, Y)];
        }
        
        return null;
    }

    public void AddVerticalPasageway(int StartX, int StartY, int Up, int Right)
    {
        this.GetRoom(StartX, StartY).AddDoor(StartX, StartY);
        // try {
        if (this.Map[StartX - Up, StartY + Right] != '#')
            this.GetRoom(StartX - Up, StartY + Right).AddDoor(StartX - Up, StartY + Right);
        // }catch(NullReferenceException)
        // {}
        if (Up > 0)
        {
            for (int X = StartX - 1; X >= StartX - Up; X--)
            {
                // this.Map[X, StartY] = '#';
                this.RoomDict.TryAdd((X, StartY), new Room(X, StartY, 1, 1, 0, this.Hero));
            }

            if (Right > 0)
            {
                for (int Y = StartY + 1; Y < StartY + Right; Y++)
                {
                    // this.Map[StartX - Up, Y] = '#';
                    this.RoomDict.TryAdd((StartX - Up, Y), new Room(StartX - Up, Y, 1, 1, 0, this.Hero));
                }
            }
            else //Left
            {
                int Left = Math.Abs(Right);

                for (int Y = StartY - 1; Y > StartY - Left; Y--)
                {
                    // this.Map[StartX - Up, Y] = '#';
                    this.RoomDict.TryAdd((StartX - Up, Y), new Room(StartX - Up, Y, 1, 1, 0, this.Hero));
                }
            }
        }
        else //Down
        {
            int Down = Math.Abs(Up);

            for (int X = StartX + 1; X <= StartX + Down; X++)
            {
                // this.Map[X, StartY] = '#';
                this.RoomDict.TryAdd((X, StartY), new Room(X, StartY, 1, 1, 0, this.Hero));
            }

            if (Right > 0)
            {
                for (int Y = StartY + 1; Y < StartY + Right; Y++)
                {
                    // this.Map[StartX + Down, Y] = '#';
                    this.RoomDict.TryAdd((StartX + Down, Y), new Room(StartX + Down, Y, 1, 1, 0, this.Hero));
                }
            }
            else //Left
            {
                int Left = Math.Abs(Right);

                for (int Y = StartY - 1; Y > StartY - Left; Y--)
                {
                    // this.Map[StartX + Down, Y] = '#';
                    this.RoomDict.TryAdd((StartX + Down, Y), new Room(StartX + Down, Y, 1, 1, 0, this.Hero));
                }
            }
        }
    }

    public void AddHorizontalPasageway(int StartX, int StartY, int Up, int Right)
    {
        this.GetRoom(StartX, StartY).AddDoor(StartX, StartY);
        try
        {
            this.GetRoom(StartX - Up, StartY + Right).AddDoor(StartX - Up, StartY + Right);
        }
        catch (NullReferenceException) { }

        if (Right > 0)
        {
            for (int Y = StartY + 1; Y <= StartY + Right; Y++)
            {
                this.RoomDict.TryAdd((StartX, Y), new Room(StartX, Y, 1, 1, 0, this.Hero));
                // this.Map[StartX, Y] = '#';
            }

            if (Up > 0)
            {
                for (int X = StartX - 1; X > StartX - Up; X--)
                {
                    // this.Map[X, StartY + Right] = '#';
                    this.RoomDict.TryAdd((X, StartY + Right), new Room(X, StartY + Right, 1, 1, 0, this.Hero));
                }
            }
            else //Down
            {
                int Down = Math.Abs(Up);

                for (int X = StartX + 1; X < StartX + Down; X++)
                {
                    // this.Map[X, StartY + Right] = '#';
                    this.RoomDict.TryAdd((X, StartY + Right), new Room(X, StartY + Right, 1, 1, 0, this.Hero));
                }
            }
        }
        else //Left
        {
            int Left = Math.Abs(Right);

            for (int Y = StartY - 1; Y >= StartY - Left; Y--)
            {
                this.RoomDict.TryAdd((StartX, Y), new Room(StartX, Y, 1, 1, 0, this.Hero));
                // this.Map[StartX, Y] = '#';
            }

            if (Up > 0)
            {
                for (int X = StartX - 1; X >= StartX - Up; X--)
                {
                    this.RoomDict.TryAdd((X, StartY - Left), new Room(X, StartY - Left, 1, 1, 0, this.Hero));
                    // this.Map[X, StartY - Left] = '#';
                }
            }
            else //Down
            {
                int Down = Math.Abs(Up);

                for (int X = StartX + 1; X <= StartX + Down; X++)
                {
                    this.RoomDict.TryAdd((X, StartY - Left), new Room(X, StartY - Left, 1, 1, 0, this.Hero));
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
        int StartX = this.Rooms[0].X + 3;
        int StartY = this.Rooms[0].Y + 3;
        B.GoTo(StartX, StartY);


        char[,] WalkableMap = new char[this.Length, this.Width];

        this.MonsterDict.Add((B.X, B.Y), B);
        int Moves = 0;
        do
        {
            Moves = 0;
            Dictionary<(int, int), Creature> WalkerDict = new Dictionary<(int, int), Creature>();

            foreach (KeyValuePair<(int, int), Creature> pair in this.MonsterDict)
            {
                WalkerDict.Add(pair.Key, pair.Value);
            }

            foreach (Creature Walker in WalkerDict.Values)
            {
                while (this.MovementAllowed(Walker.X - 1, Walker.Y))
                {
                    Walker.MoveNorth();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y];

                    Bandit WN = new Bandit();
                    WN.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y), WN);
                }

                while (this.MovementAllowed(Walker.X, Walker.Y - 1))
                {
                    Walker.MoveWest();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y];

                    Bandit WW = new Bandit();
                    WW.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y), WW);
                }

                while (this.MovementAllowed(Walker.X + 1, Walker.Y))
                {
                    Walker.MoveSouth();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y];

                    Bandit WS = new Bandit();
                    WS.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y), WS);
                }

                while (this.MovementAllowed(Walker.X, Walker.Y + 1))
                {
                    Walker.MoveEast();
                    Moves++;
                    WalkableMap[Walker.X, Walker.Y] = this.Map[Walker.X, Walker.Y];

                    Bandit WE = new Bandit();
                    WE.GoTo(Walker.X, Walker.Y);
                    this.MonsterDict.Add((Walker.X, Walker.Y), WE);
                }
            }
        } while (Moves > 0);

        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                if (this.Map[i, j] == '.' && !this.MonsterDict.ContainsKey((i, j)))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
