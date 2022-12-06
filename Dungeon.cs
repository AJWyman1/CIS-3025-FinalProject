using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Dungeon {

    public char[,] Map;
    public char[,] EmptyMap;
    public Dictionary<(int, int), Creature> MonsterDict;
    public PlayerCharacter Hero;
    public Dictionary<(int, int), Dictionary< Item, int>> LootDict;
    public Room[] Rooms;

    public Dungeon(int X = 60, int Y = 60, int Rooms = 10)
    {
        this.Map = new char[X,Y];
        FillMap();
        this.Rooms = new Room[Rooms];
        this.CreateRooms(Rooms);
        this.CreatePasageways(2);
        this.EmptyMap = (char[,])this.Map.Clone();
        this.MonsterDict = new Dictionary<(int, int), Creature>(); 
        this.LootDict = new Dictionary<(int, int), Dictionary< Item, int>>(); //Dictionary of Loot on floor of the dungeon Dict( X, Y : Dict(Item : NumOfType))
    }

    public void FillMap()
    {
        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                this.Map[i,j] = ' ';
            }
        }
    }

    // Room generation algorithm 
    // Hallway connecting algorithm 
    public void PlaceHeroInRoom(PlayerCharacter p)
    {
        this.Hero = p;
        PlaceCreatureInRoom(this.Hero);
    }

    public void PlaceCreatureInRoom(Creature c = null)
    {
        int X;
        int Y;
        do{
            X = Dice.Roll(this.Map.GetLength(0) - 1);
            Y =  Dice.Roll(this.Map.GetLength(1) - 1);
        }while(this.MonsterDict.ContainsKey((X, Y)) || this.Map[X,Y] != '.'); //Rolls dice for location
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
        else {
            this.MonsterDict.Add((X, Y), c);
        }

        this.MonsterDict[(X, Y)].GoTo(X, Y);
        this.MonsterDict[(X, Y)].RollInitiative();
        this.MonsterDict[(X, Y)].PickUpItem(new PotionOfHealing());
        this.MonsterDict[(X, Y)].PickUpItem(new PotionOfHealing());
        this.Map[X,Y] = (char)this.MonsterDict[(X, Y)];
    }

    public bool MovementAllowed(int X, int Y, Creature c)
    {
        if (new char[]{'│', '┌', '┐', '┘', '└', '─', ' '}.Contains(this.Map[X,Y]))
        {
            return false;
        }
        if (this.MonsterDict.ContainsKey((X, Y)))
        {
            if (this.MonsterDict[(X,Y)].IsAlive)
            {
                PrintFightLog(c, this.MonsterDict[(X,Y)]);
                return false;
            }
        }
        return true;
    }

    public void PrintFightLog(Creature a, Creature b)
    {
        Creature[] fighters = {a, b};
        Array.Sort(fighters);
        //Console.SetCursorPosition(0, 60);
        
        Console.WriteLine($"{fighters[0].Attack(fighters[1]).PadRight(Console.WindowWidth)}");
        if(fighters[1].HP > 0) //Benefit of higher initiative
        {
        Console.WriteLine($"{fighters[1].Attack(fighters[0]).PadRight(Console.WindowWidth)}");
        }
        this.PostFightUpdate(a);
        this.PostFightUpdate(b);
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
            this.MonsterDict.Remove((c.X,c.Y));
        }
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
                Dictionary<Item, int> TempDict = new Dictionary<Item, int>{{loot, 1}};
                this.LootDict.Add((X, Y), TempDict);    
            }
    }

    public void MovementUpdate(Creature c, int LastX, int LastY)
    {
        this.MonsterDict.Remove((LastX, LastY));
        this.MonsterDict.Add((c.X, c.Y), c);
    }

    public Creature GetCreature(int x, int y)
    {
        return this.MonsterDict[(x, y)];
    }


    public void PrintMap()
    {
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < this.Map.GetLength(0); i++)
        {
            for (int j = 0; j < this.Map.GetLength(1); j++)
            {
                if(this.MonsterDict.ContainsKey((i, j)))
                {
                    Console.Write((char)this.MonsterDict[(i, j)]);
                }
                else if (i == this.Hero.X && j == this.Hero.Y)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write((char)this.Hero);
                }
                else if (this.LootDict.ContainsKey((i, j)))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    char LootChar = this.EmptyMap[i, j]; //Assign the loot char to the floor char
                    foreach(KeyValuePair<Item, int> loot in LootDict[(i, j)])
                    {
                        LootChar = loot.Key.RepresentWith;
                    }
                    Console.Write(LootChar);
                }
                else
                {
                    if (this.EmptyMap[i, j] == '━' || this.EmptyMap[i, j] == '┃')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(this.EmptyMap[i, j]);
                }
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }

    public void PrintLootable(int X, int Y)
    {
        //string output = "";
        if (this.LootDict.ContainsKey((X, Y)))
        {
            int i = 1;
            foreach(KeyValuePair<Item, int> loot in LootDict[(X, Y)])
            {
                Console.Write($"{loot.Value} {loot.Key}\n");
                i ++;
            }
            //return output;
        }
        else 
        {
            //return "";
        }
    }

    public Container<Item> ItemsOnGround(int X, int Y)
    {
        Container<Item> GroundLoot = new Container<Item>();

        if (this.LootDict.ContainsKey((X, Y)))
        {
            foreach(KeyValuePair<Item, int> loot in LootDict[(X, Y)])
            {
                GroundLoot.Add(loot.Key);
            }
        }

        return GroundLoot;
    }

    public void ItemPickedUp(Item loot, int X, int Y)
    {
        this.LootDict[(X, Y)].Remove(loot);
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
                for (int i = TopLeftX - 1; i < TopLeftX + width + 1; i ++) // Check if boarders or overlaps an existing room
                {
                    for (int j = TopLeftY - 1; j < TopLeftY + length + 1; j ++)
                    {
                    if(new char[]{'│', '┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[i,j]))
                        {
                            ValidRoom = false; 
                            i += 99999; //leave outer loop
                            break;      //leave inner loop
                        }
                    } 
                }

            if (ValidRoom)
                {
                    this.Rooms[CurrentRoom] = new Room(TopLeftX, TopLeftY, width, length, Dice.D4());
                    char[,] NewRoom = this.Rooms[CurrentRoom].Representation();
                    for (int i = 0; i <  width; i ++)
                    {
                        for (int j = 0; j < length; j ++)
                        {
                            this.Map[i + TopLeftX, j + TopLeftY] = NewRoom[i,j];
                        }
                    }
                    CurrentRoom ++;
                }
            }
            
        }while(CurrentRoom < TotalRooms);
    }

    //Passageway generation
    public void CreatePasageways(int TotalHallways)
    {
      int passageways = 0;
      do
      {
        int StartX = Dice.Roll(this.Map.GetLength(0) - 1);
        int StartY = Dice.Roll(this.Map.GetLength(1) - 1);
        bool ValidPassageway = false;

        if (this.Map[StartX, StartY] == '─')
        {
          try{
            if (this.Map[StartX - 1, StartY] == ' ' || this.Map[StartX - 1, StartY] == '#') //At the top
            {
              int Up = Dice.D6();

              for(int X = StartX; X >= StartX - Up; X--)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[X, StartY]))
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
            
              for(int EndY = StartY; EndY < this.Map.GetLength(1); EndY++)
              {
                if (new char[]{'┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX - Up, EndY]))
                {
                  break;
                }
                
                if (this.Map[StartX - Up, EndY] == '│')
                {
                  AddVerticalPasageway(StartX, StartY, Up, Right);
                  ValidPassageway = true;
                  break;
                }
                
                Right += 1;
              }
              int Left = 0;
              for(int EndY = StartY; EndY > 0; EndY--)
              {
                if (new char[]{'┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX - Up, EndY]))
                {
                  break;
                }
                
                if (this.Map[StartX - Up, EndY] == '│')
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
              
              for(int X = StartX; X >= StartX + Down; X++)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[X, StartY]))
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
              for(int EndY = StartY; EndY < this.Map.GetLength(1); EndY++)
              {
                if (new char[]{'┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX + Down, EndY]))
                {
                  break;
                }
                if (this.Map[StartX + Down, EndY] == '│' || this.Map[StartX + Down, EndY] == '#')
                {
                  AddVerticalPasageway(StartX, StartY, Down * -1, Right);
                  ValidPassageway = true;
                  break;
                }
                Right += 1;
              }
              int Left = 0;
              for(int EndY = StartY; EndY > 0; EndY--)
              {
                if (new char[]{'┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX + Down, EndY]))
                {
                  break;
                }
                if (this.Map[StartX + Down, EndY] == '│' || this.Map[StartX + Down, EndY] == '#')
                {
                  AddVerticalPasageway(StartX, StartY, Down * -1, Left);
                  ValidPassageway = true;
                  break;
                }
                Left -= 1;
              }
            }
          }catch (IndexOutOfRangeException)
          {continue;}
        }

        if (this.Map[StartX, StartY] == '│')
        {
          try{
            if (this.Map[StartX, StartY - 1] == ' ' || this.Map[StartX, StartY - 1] == '#') //At the left
            {
              int Left = Dice.D6();
              for(int Y = StartY; Y > StartY - Left; Y--)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX, Y]))
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

              for(int EndX = StartX; EndX >= 0; EndX--)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '.'}.Contains(this.Map[EndX, StartY - Left]))
                {
                  break;
                }
                if (this.Map[EndX, StartY - Left] == '─' || this.Map[EndX, StartY - Left] == '#')
                {
                  AddHorizontalPasageway(StartX, StartY, Up, Left * -1);
                  ValidPassageway = true;
                  break;
                }
                Up += 1;
              }

              int Down = 0; //build hall down
              
              for(int EndX = StartX; EndX <= this.Map.GetLength(0); EndX++)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '.'}.Contains(this.Map[EndX, StartY - Left]))
                {
                  break;
                }
                if (this.Map[EndX, StartY - Left] == '─' || this.Map[EndX, StartY - Left] == '#')
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
              for(int Y = StartY; Y < StartY + Right; Y++)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '─', '.'}.Contains(this.Map[StartX, Y]))
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

              for(int EndX = StartX; EndX >= 0; EndX--)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '.'}.Contains(this.Map[EndX, StartY + Right]))
                {
                  break;
                }
                if (this.Map[EndX, StartY + Right] == '─' || this.Map[EndX, StartY + Right] == '#')
                {
                  AddHorizontalPasageway(StartX, StartY, Up, Right);
                  ValidPassageway = true;
                  break;
                }
                Up += 1;
              }

              int Down = 0; //build hall down
              
              for(int EndX = StartX; EndX <= this.Map.GetLength(0); EndX++)
              {
                if (new char[]{'│', '┌', '┐', '┘', '└', '.'}.Contains(this.Map[EndX, StartY + Right]))
                {
                  break;
                }
                if (this.Map[EndX, StartY + Right] == '─' || this.Map[EndX, StartY + Right] == '#')
                {
                  AddHorizontalPasageway(StartX, StartY, Down, Right);
                  ValidPassageway = true;
                  break;
                }
                Down -= 1;
              }
            }
          }catch (IndexOutOfRangeException)
          {continue;}
        }
        if (ValidPassageway)
        {
          passageways += 1;
          // Console.WriteLine($"Passageway {passageways} completed at {StartX}, {StartY}");
          // PrintGrid(grid);
          // char Waiting = Console.ReadKey(true).KeyChar;
          //Console.Clear();
        }
      }while (passageways < TotalHallways);
    }

    public void  AddVerticalPasageway(int StartX, int StartY, int Up, int Right)
    {
        if (Up > 0)
        {
        for(int X = StartX; X >= StartX - Up; X--)
            {
            this.Map[X, StartY] = '#';
            }
        
        if (Right > 0)
        {
            for(int Y = StartY; Y <= StartY + Right; Y++)
            {
            this.Map[StartX - Up, Y] = '#';
            }
        }
        else //Left
        {
            int Left = Math.Abs(Right);

            for(int Y = StartY; Y >= StartY - Left; Y--)
            {
            this.Map[StartX - Up, Y] = '#';
            }
        }
        }
        else //Down
        {
        int Down = Math.Abs(Up);

        for(int X = StartX; X <= StartX + Down; X++)
            {
            this.Map[X, StartY] = '#';
            }
        
        if (Right > 0)
        {
            for(int Y = StartY; Y <= StartY + Right; Y++)
            {
            this.Map[StartX + Down, Y] = '#';
            }
        }
        else //Left
        {
            int Left = Math.Abs(Right);

            for(int Y = StartY; Y >= StartY - Left; Y--)
            {
            this.Map[StartX + Down, Y] = '#';
            }
        }
        }
    }

    public void AddHorizontalPasageway(int StartX, int StartY, int Up, int Right)
    {
        if (Right > 0)
        {
            for(int Y = StartY; Y <= StartY + Right; Y++)
                {
                    this.Map[StartX, Y] = '#';
                }
            
            if (Up > 0)
            {
                for(int X = StartX; X >= StartX - Up; X--)
                {
                    this.Map[X, StartY + Right] = '#';
                }
            }
            else //Down
            {
                int Down = Math.Abs(Up);

                for(int X = StartX; X <= StartX + Down; X++)
                {
                    this.Map[X, StartY + Right] = '#';
                }
            }
        }
        else //Left
        {
        int Left = Math.Abs(Right);

        for(int Y = StartY; Y >= StartY - Left; Y--)
            {
            this.Map[StartX, Y] = '#';
            }
        
        if (Up > 0)
        {
            for(int X = StartX; X >= StartX - Up; X--)
            {
            this.Map[X, StartY - Left] = '#';
            }
        }
        else //Down
        {
            int Down = Math.Abs(Up);

            for(int X = StartX; X <= StartX + Down; X++)
            {
            this.Map[X, StartY - Left] = '#';
            }
        }
        }
    }
}
