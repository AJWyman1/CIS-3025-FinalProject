using System;
using System.Linq;
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
            PlayerCharacter Larry = new PlayerCharacter(ChooseRace(), SelectClass(), SetPlayerName(), ChooseStats());
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


    //// PC creation Methods

    

    public static Race ChooseRace()
	{
		int RaceInt;
		do {
			Console.Clear();
			OutputRaces();
			Console.WriteLine("Select a race: ");
			RaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
			
		} while (RaceInt < 0 || RaceInt > 8);

		Race PlayerRace = (Race)RaceInt;
		if (HasSubRace(PlayerRace))
		{
			PlayerRace = ChooseSubRace(PlayerRace);
		}

        return PlayerRace;
	}
    public static void OutputRaces()
    {
        Console.WriteLine(	"1 - Dragonborn\n" +
                            "2 - Dwarf\n" +
                            "3 - Elf\n" +
                            "4 - Gnome\n" +
                            "5 - Half-Elf\n" +
                            "6 - Half-Orc\n" +
                            "7 - Halfling\n" +
                            "8 - Human\n" +
                            "9 - Tiefling\n");

	}

	public static bool HasSubRace(Race r)
	{
		Race[] HasSubRaceArr = new Race[4] {(Race)1, (Race)2, (Race)3, (Race)6};
		return HasSubRaceArr.Contains(r);
	}

    public static Race ChooseSubRace(Race BaseRace)
	{
		int SubRaceInt;
		int[] options;
		
		do {
			options = new int[]{1, 2};
			Console.Clear();
			Console.WriteLine("Select a sub-race:");
			if (BaseRace == (Race)1) //Dwarf
			{
				Console.WriteLine(	"1 - Hill Dwarf\n" +
									"2 - Mountain Dwarf");
			}
			else if (BaseRace == (Race)2)
			{
				options = new int[]{1, 2, 3};
				Console.WriteLine(	"1 - High Elf\n" +
									"2 - Wood Elf\n" +
									"3 - Dark Elf");
			}
			else if (BaseRace == (Race)3)
			{
				Console.WriteLine(	"1 - Forest Gnome\n" +
									"2 - Rock Gnome");
			}
			else if (BaseRace == (Race)6)
			{
				Console.WriteLine(	"1 - Lightfoot Halfling\n" +
									"2 - Stout Halfling");
			}
			Console.WriteLine();
			SubRaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar);
		} while (!options.Contains(SubRaceInt)); //exits upon selection of acceptable inputs 
		SubRaceInt += ((int)BaseRace * 10);
		
		Race PlayerRace = (Race)SubRaceInt;

        return PlayerRace;
	}

    public static Class SelectClass()
	{
		char[] options = {'1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c'};
		char input;
		do{
		Console.Clear();
		Console.WriteLine(	"1  - Barbarian\n" +
							"2  - Bard\n" +
							"3  - Cleric\n" +
							"4  - Druid\n" +
							"5  - Fighter\n" +
							"6  - Monk\n" +
							"7  - Paladin\n" +
							"8  - Ranger\n" +
							"9  - Rogue\n" +
							"a - Sorcerer\n" +
							"b - Warlock\n" +
							"c - Wizard\n");
		Console.WriteLine("Select a Class: ");

		input = Console.ReadKey(true).KeyChar;

		}while(!(options.Contains(input)));

		Console.WriteLine();

		Class PlayerClass = (Class)Array.IndexOf(options, input); //Might be my favorite line
        return PlayerClass;
    }

    public static string SetPlayerName()
	{
        Console.Clear();
		Console.WriteLine("What is your name adventurer?");
		string Name = Console.ReadLine();
        return Name;
	}

    public static int[] ChooseStats()
	{
		Console.Clear();
		String[] AttributeNames = {"Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"};
		int[] attributes = new int[6];
        int[] Rolls = {Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum(), Dice.DropAndSum()};
		int attsSelected = 0;
		int lastAtts = 1;
		do{
				Array.Sort(Rolls);
				Array.Reverse(Rolls);
				int roll = Rolls[0];
				Console.Write("Your remaining rolls are: \t");
				for (int i = 0; i < Rolls.Length; i++)
				{
					Console.Write($"{(Rolls[i] != 0 ? $"{Rolls[i]}\t" : "")}");
				}
				if (lastAtts == attsSelected)
				// {
				//  roll = Rolls[0];
				// }
				// else 
				{
					Console.WriteLine("Try again.");
				}
				Console.WriteLine($"\nYour next roll is a {roll}.\nSelect an attribute to apply:");
			
				for (int j = 0; j < attributes.Length; j ++){
					if (attributes[j] == 0)
					{
						Console.WriteLine($"[{j+1}]: {AttributeNames[j]}");
					}
					else 
					{
						Console.WriteLine($"[-]: {AttributeNames[j]} - {attributes[j]}");
					}
				}
				int KeyInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
				lastAtts = attsSelected;
				if (KeyInt >= 0 && KeyInt < 6)
				{
					if (attributes[KeyInt] == 0)
					{
						attributes[KeyInt] = roll;
						Rolls[0] = 0;
						attsSelected++;
					}
				}
				Console.Clear();
		} while (attsSelected < 6);

        return attributes;
	}

}