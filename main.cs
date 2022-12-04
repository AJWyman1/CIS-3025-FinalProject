using System;
using System.Media;

class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();

        Console.WriteLine("Would you like to play a game? [y/n]");
        if (Console.ReadKey(true).KeyChar == 'y')
        {
            Console.Clear();
            PlayerCharacter Larry = new PlayerCharacter();
            Larry.EquipWeapon(new Greataxe());

            PotionOfHealing HealthPot = new PotionOfHealing();

            Larry.PickUpItem(new Schimitar());
            Larry.PickUpItem(HealthPot);

            PotionOfHealing HealthPot2 = new PotionOfHealing();
            Larry.PickUpItem(HealthPot2);
            

            GameTest(Larry);
        }
    }

    public static void GameTest(PlayerCharacter c)
    {
        Dungeon d = new Dungeon();
        Console.Clear();

        c.GoTo(1, 1);
        d.PlaceHeroInRoom(c);

        d.PlaceCreatureInRoom();
        d.PlaceCreatureInRoom(new Bandit());

        c.RollInitiative();
        d.PrintMap();

        ConsoleKeyInfo keyInfo;
        do
        {
            
            d.PrintMap();
            PrintCharSheet(c);

            keyInfo = Console.ReadKey(true);
            int lastX = c.X;
            int lastY = c.Y;
            if (keyInfo.KeyChar == 'w')
            {
                if (d.MovementAllowed(c.X - 1, c.Y, c))
                    { c.MoveNorth(); }
            }
            else
            if (keyInfo.KeyChar == 'a')
            {
                if (d.MovementAllowed(c.X, c.Y - 1, c))
                    { c.MoveWest(); }
            }
            else
            if (keyInfo.KeyChar == 's')
            {
                if (d.MovementAllowed(c.X + 1, c.Y, c))
                    { c.MoveSouth(); }
            }
            else
            if (keyInfo.KeyChar == 'd')
            {
                if (d.MovementAllowed(c.X, c.Y + 1, c))
                    { c.MoveEast(); }
            }
            else
            if (keyInfo.KeyChar == 'r')
            {
                Console.WriteLine(c.Rest());
            }
            else
            if (keyInfo.KeyChar == 'p')
            {
                if (!d.LootDict.ContainsKey((c.X, c.Y)))
                {
                    continue;
                }
                //Print out items on the ground
                Container<Item> Loot = d.ItemsOnGround(c.X, c.Y);
                PrintLoot(Loot);
                //Take input
                int input = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
                //Pick up item selected
                try {
                    if (Loot.Entries[input] != null)
                    {
                        c.PickUpItem(Loot.Entries[input]);
                        d.ItemPickedUp(Loot.Entries[input], c.X, c.Y);
                    }
                }catch(IndexOutOfRangeException)
                {
                    
                }
                Console.Clear();
            }
            else
            {
                int input = (int)Char.GetNumericValue(keyInfo.KeyChar) - 1;
                if ( input >= 0 && c.Inventory.Entries[input] != null)
                {
                    Console.Clear();
                    d.PrintMap();
                    PrintCharSheet(c);
                    Console.WriteLine(c.Inventory.Entries[input].Use(c, input));
                }
            }
            
            d.Map[lastX, lastY] = d.EmptyMap[lastX, lastY];
            d.Map[c.X, c.Y] = (char)c;
            
            if (lastX != c.X || lastY != c.Y)
            {
                d.MovementUpdate(c, lastX, lastY);
                Console.Clear();
            }
            
        } while (keyInfo.Key != ConsoleKey.Escape && c.HP > 0);
        if(c.HP <= 0){Console.WriteLine($"YOU DIED{new String(' ', Console.BufferWidth / 2)}");}
    
    }
    
    public static void PrintCharSheet(Creature c)
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
        Console.WriteLine("  ↑");
        Console.SetCursorPosition(width - 24, charsheet.Length+3);
        Console.WriteLine("  W");
        Console.SetCursorPosition(width - 24, charsheet.Length+4);
        Console.WriteLine("←ASD→");
        Console.SetCursorPosition(width - 24, charsheet.Length+5);
        Console.WriteLine("  ↓");
        Console.SetCursorPosition(width - 30, charsheet.Length+6);
        Console.WriteLine("[r]est");
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

}