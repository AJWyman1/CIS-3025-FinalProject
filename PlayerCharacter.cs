using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PlayerCharacter : Creature {


	

	/* PlayerCharacter Attributes */

	
	public Race PlayerRace { get; protected set; }
	public string PlayerRaceStr { get; protected set; }
	public Class PlayerClass { get; protected set;}
	public int HitDie { get; protected set;}
	public string Proficiencies { get; protected set;}
	//public bool Lucky {get; protected set;} //Halflings re-roll 1's on attack rolls
	

	public PlayerCharacter() : base()
	{
		this.Resistances = "";
		this.Proficiencies = "";
		this.RepresentWith = '@';
		this.Inventory = new Container<Item>();
		this.SetName();
		this.ChooseRace();
		this.ChooseStats();
		this.AddRacialModifiers();
		this.SelectClass();
		this.SetHitDie();
		this.HP += this.HitDie + AbilityModifier(this.Constitution);
		this.MaxHP = this.HP;
		this.GoldPouch = 50;
		this.ArmorClass = 10 + this.AbilityModifier(this.Dexterity);
		this.Color = ConsoleColor.Blue;
	}

	public PlayerCharacter(Race playerRace, Class playerClass, string playerName, int[] stats)
	{
		this.Resistances = "";
		this.Proficiencies = "";
		this.RepresentWith = '@';
		this.Inventory = new Container<Item>();
		this.Name = playerName;
		this.PlayerRace = playerRace;
		this.attributes = stats;
		this.AddRacialModifiers();
		this.PlayerClass = playerClass;
		this.SetClassProficiencies();
		this.SetHitDie();
		this.HP += this.HitDie + AbilityModifier(this.Constitution);
		this.MaxHP = this.HP;
		this.GoldPouch = 50;
		this.ArmorClass = 10 + this.AbilityModifier(this.Dexterity);
		this.Color = ConsoleColor.Blue;
	}

	public override void LevelUp()
	{
		for(int i = 0; i < this.attributes.Length; i++)
                {
                    this.attributes[i] += 1;
                }
                this.XP -= 10;
                this.Level += 1;
                this.XPGiven += 2;
				this.MaxHP = this.HitDie + AbilityModifier(this.Constitution);
				this.ArmorClass = 10 + this.AbilityModifier(this.Dexterity);
	}

	public override string Attack(Creature c)
	{
		int Damage;
		int Modifier;
		int ToHitBonus = 0;

		//AC Check
		int roll = Dice.D20();
		bool Crit = roll == 20;

		if (roll == 1 && this.PlayerRace == Race.Halfling) //Halflings are 'Lucky'
		{
			roll = Dice.D20();
		}
		
		if (this.EquippedWeapon != null)
		{
			//Apply appropriate Modifier for Melee or Ranged
			if (this.EquippedWeapon.isRanged){ Modifier = this.AbilityModifier(this.Dexterity);}
			else { Modifier = this.AbilityModifier(this.Strength);}

			//Apply To Hit bonus if proficient
			if (isProficient()){ ToHitBonus = 2; }

			if ((roll + Modifier + ToHitBonus) > c.ArmorClass || Crit) //Hit
			{
				Damage = this.EquippedWeapon.RollDamage() + Modifier;
				if (Crit){ Damage += this.EquippedWeapon.RollDamage();}
				c.HP -= Damage;
				c.Defend();
				return $"{this.Name}({this.HP}/{this.MaxHP}){(Crit ? " CRITICALLY" : "")} hits {c.Name}({c.HP}/{c.MaxHP}) with their {this.EquippedWeapon.Name} for {Damage} points of {this.EquippedWeapon.DamageType} damage!";
			}else 
			{
				return $"{this.Name}({this.HP}/{this.MaxHP}) swings their {this.EquippedWeapon.Name} and MISSES {c.Name}({c.HP}/{c.MaxHP})!";
			}
		}else //unarmed attack
		{
			Modifier = this.AbilityModifier(this.Strength);

			if ((roll + Modifier) > c.ArmorClass || Crit) //Hit
			{
				Damage = 1;
				if (Crit){ Damage = 2; }
				c.HP -= Damage;
				c.Defend();
				return $"{this.Name}({this.HP}/{this.MaxHP}){(Crit ? " CRITICALLY" : "")} hits {c.Name}({c.HP}/{c.MaxHP}) with their fists for {Damage} of bludgeoning damage!";
			}else{
				return $"{this.Name}({this.HP}/{this.MaxHP}) swings their fists and MISSES {c.Name}({c.HP}/{c.MaxHP})!";
			}
		}
	}

	public bool isProficient()
	{
		string[] WeaponType = this.EquippedWeapon.WeaponType;
		for(int i = 0; i < WeaponType.Length; i++)
		{
			if (this.Proficiencies.Contains(WeaponType[i]))
			{
				return true;
			}
		}
		return false;
	}
	public override string Defend()
	{
		return "Defending";
	}

	public override string DefendAgainst(Creature c)
	{
		return "Defending against ___";
	}

	public override string Rest()
	{
		int Healed = Dice.Roll(this.HitDie);
		this.HP += Healed;
		if (this.HP > this.MaxHP){this.HP = this.MaxHP;}
		return $"{this.Name} rested and regained {Healed} hit points and is currently {this.HP}/{this.MaxHP}";
	}

	/* Character creation methods */
	public void SetName()
	{
		Console.WriteLine("What is your Character's name?");
		this.Name = Console.ReadLine();
	}

	public void ChooseStats()
	{
		Console.Clear();
		String[] AttributeNames = {"Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"};
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
			
				for (int j = 0; j < this.attributes.Length; j ++){
					if (this.attributes[j] == 0)
					{
						Console.WriteLine($"[{j+1}]: {AttributeNames[j]}");
					}
					else 
					{
						Console.WriteLine($"[-]: {AttributeNames[j]} - {this.attributes[j]}");
					}
				}
				int KeyInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
				lastAtts = attsSelected;
				if (KeyInt >= 0 && KeyInt < 6)
				{
					if (this.attributes[KeyInt] == 0)
					{
						this.attributes[KeyInt] = roll;
						Rolls[0] = 0;
						attsSelected++;
					}
				}
				Console.Clear();
		} while (attsSelected < 6);
	}

	public void ChooseRace()
	{
		int RaceInt;
		do {
			Console.Clear();
			OutputRaces();
			Console.WriteLine("Select a race: ");
			RaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
			
		} while (RaceInt < 0 || RaceInt > 8);

		this.PlayerRace = (Race)RaceInt;
		if (this.HasSubRace(this.PlayerRace))
		{
			this.ChooseSubRace();
		}
	}

	public bool HasSubRace(Race r)
	{
		Race[] HasSubRaceArr = new Race[4] {(Race)1, (Race)2, (Race)3, (Race)6};
		return HasSubRaceArr.Contains(r);
	}

	public void ChooseSubRace()
	{
		int SubRaceInt;
		int[] options;
		
		do {
			options = new int[]{1, 2};
			Console.Clear();
			Console.WriteLine("Select a sub-race:");
			if (this.PlayerRace == (Race)1) //Dwarf
			{
				Console.WriteLine(	"1 - Hill Dwarf\n" +
									"2 - Mountain Dwarf");
			}
			else if (this.PlayerRace == (Race)2)
			{
				options = new int[]{1, 2, 3};
				Console.WriteLine(	"1 - High Elf\n" +
									"2 - Wood Elf\n" +
									"3 - Dark Elf");
			}
			else if (this.PlayerRace == (Race)3)
			{
				Console.WriteLine(	"1 - Forest Gnome\n" +
									"2 - Rock Gnome");
			}
			else if (this.PlayerRace == (Race)6)
			{
				Console.WriteLine(	"1 - Lightfoot Halfling\n" +
									"2 - Stout Halfling");
			}
			Console.WriteLine();
			SubRaceInt = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar);
		} while (!options.Contains(SubRaceInt)); //exits upon selection of acceptable inputs 
		SubRaceInt += ((int)this.PlayerRace * 10);
		
		this.PlayerRace = (Race)SubRaceInt;
	}

	public int[] RaceAttributeModifier()
	{
		int RaceInt = (int)this.PlayerRace;

		int[] modify = new int[6];
        //  0      1      2      3      4      5
        //"STR", "DEX", "CON", "INT", "WIS", "CHA"

		if (RaceInt == 0) //Dragonborne
		{
			modify[0] = 2;
            modify[5] = 1;
		}else if (RaceInt == 11) //HillDwarf
		{
			this.Proficiencies += "battleaxe, handaxe, throwing hammer, warhammer, ";
			this.HP += 1;
			this.Darkvision = true;
			modify[2] = 2;
			modify[4] = 1;
			this.Resistances += "Poison, ";
		}else if (RaceInt == 12) //Mountain Dwarf
		{
			this.Proficiencies += "battleaxe, handaxe, throwing hammer, warhammer, light armor, medium armor, ";
			this.Darkvision = true;
			modify[2] = 2;
			modify[0] = 2;
			this.Resistances += "Poison, ";
		}else if (RaceInt == 21) // High Elf
		{
			this.Proficiencies += "longsword, shortsword, shortbow, longbow, ";
			this.Darkvision = true;
			modify[1] = 2;
			modify[3] = 1;
		}else if (RaceInt == 22) // Wood Elf
		{
			this.Proficiencies += "longsword, shortsword, shortbow, longbow, ";
			this.Darkvision = true;
			modify[1] = 2;
			modify[4] = 1;
		}else if (RaceInt == 23) // Dark Elf
		{
			this.Proficiencies += "rapier, shortsword, hand crossbow, ";
			this.Darkvision = true;
			modify[1] = 2;
			modify[5] = 1;
		}else if (RaceInt == 31) //Forest Gnome
		{
			modify[1] = 1;
			modify[3] = 2;
		}else if (RaceInt == 32) // Rock Gnome
		{
			modify[3] = 2;
			modify[2] = 1;
		}else if (RaceInt == 4) //Half Elf
		{
			this.Darkvision = true;
			modify[5] = 2;
			modify = HalfElfModifier(modify);
		}else if (RaceInt == 5) //Half Orc
		{
			this.Darkvision = true;
			modify[0] = 2;
            modify[2] = 1;
		}else if (RaceInt == 61) // lightfoot Halfling
		{
			modify[1] = 2;
            modify[5] = 1;
		}else if (RaceInt == 62) // Stout Halfling
		{
			modify[1] = 2;
            modify[2] = 1;
			this.Resistances += "Poison, ";
		}else if (RaceInt == 7) //Human
		{
			modify[0] = 1;
            modify[1] = 1;
            modify[2] = 1;
            modify[3] = 1;
            modify[4] = 1;
            modify[5] = 1;
		}else  // Tiefling
		{
			this.Darkvision = true;
			modify[0] = 1;
			modify[5] = 2;
			this.Resistances +="Fire, ";
		}
		return modify;
	}

	public int[] HalfElfModifier(int[] modify)
    {
      String[] AttributeNames = new String[6] {"Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma"};
      int numModified = 0;
      while (numModified < 2)
      {
		Console.Clear();
        int[] avaliable = new int[6];
        for (int i = 0; i < modify.Length; i++)
        {
          if (modify[i] == 0)
          { 
            avaliable[i] = 1;
            Console.WriteLine($"{i + 1}. {AttributeNames[i]}");
          }
        }
        Console.WriteLine("\nSelect one attribute to give +1: ");
		int input = (int)Char.GetNumericValue(Console.ReadKey(true).KeyChar) - 1;
		        
        if (avaliable[input] == 1){
          modify[input] += 1;
          numModified++;
        }
      }
      return modify;
    }

	public void AddRacialModifiers()
	{
		int[] modifiers = this.RaceAttributeModifier();

		for (int i = 0; i < this.attributes.Length; i++)
		{
			this.attributes[i] += modifiers[i];
		}
	}

	public void SelectClass()
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

		this.PlayerClass = (Class)Array.IndexOf(options, input); //Might be my favorite line

		string[] ClassProficiencies = 
		{
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"light armor, medium armor, shield, simple weapon, ",
			"light armor (nonmetal), medium armor (nonmetal), shield (nonmetal), club, dagger, dart, javelin, mace, quarterstaff, scimitar, sickle, sling, spear, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"simple weapon, shortsword, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"dagger, dart, sling, quarterstaff, light crossbow, ",
			"light armor, simple weapon, ",
			"dagger, dart, sling, quarterstaff, light crossbow, "
		};
		this.Proficiencies += ClassProficiencies[(int)this.PlayerClass];
	}

	public void SetClassProficiencies()
	{
		string[] ClassProficiencies = 
		{
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"light armor, medium armor, shield, simple weapon, ",
			"light armor (nonmetal), medium armor (nonmetal), shield (nonmetal), club, dagger, dart, javelin, mace, quarterstaff, scimitar, sickle, sling, spear, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"simple weapon, shortsword, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"dagger, dart, sling, quarterstaff, light crossbow, ",
			"light armor, simple weapon, ",
			"dagger, dart, sling, quarterstaff, light crossbow, "
		};
		this.Proficiencies += ClassProficiencies[(int)this.PlayerClass];
	}

	public void SetProficiencies()
	{
		string[] ClassProficiencies = 
		{
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"light armor, medium armor, shield, simple weapon, ",
			"light armor (nonmetal), medium armor (nonmetal), shield (nonmetal), club, dagger, dart, javelin, mace, quarterstaff, scimitar, sickle, sling, spear, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"simple weapon, shortsword, ",
			"light armor, medium armor, heavy armor, shield, simple weapon, martial weapon, ",
			"light armor, medium armor, shield, simple weapon, martial weapon, ",
			"light armor, simple weapon, hand crossbow, longsword, rapier, shortsword, ",
			"dagger, dart, sling, quarterstaff, light crossbow, ",
			"light armor, simple weapon, ",
			"dagger, dart, sling, quarterstaff, light crossbow, "
		};

		this.Proficiencies += ClassProficiencies[(int)this.PlayerClass];
	}

	public void SetHitDie()
	{
		if (this.PlayerClass == Class.Barbarian)
		{
			this.HitDie = 12;
		}else if (this.PlayerClass == Class.Wizard || this.PlayerClass == Class.Sorcerer)
		{
			this.HitDie = 6;
		}else if (this.PlayerClass == Class.Fighter || this.PlayerClass == Class.Paladin || this.PlayerClass == Class.Ranger)
		{
			this.HitDie = 10;
		}else
		{
			this.HitDie = 8;
		}
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

	public override string ToString()
    {
        string output =
            this.Name + " " + "Level " + this.Level + " " + this.PlayerClass + 
            "\n============" +
            "\nSTR: " + this.Strength +
            "\nDEX: " + this.Dexterity +
            "\nCON: " + this.Constitution +
            "\nINT: " + this.Intelligence +
            "\nWIS: " + this.Wisdom +
            "\nCHA: " + this.Charisma +
            "\n============" +
            "\nHP: " + this.HP + "/" +this.MaxHP +
            "\nAC: " + this.ArmorClass  +
            "\n============" +
            "\nResistances: \n" + 
			this.Resistances.Replace(", ", "\n") +
            "\n============" +
            "\nDarkvision: " + this.Darkvision + "\n" + 
			"\n============" +
			"\nInventory:\n" +
			this.PrintInventory() +  "\n" +
            this.GoldPouch + "GP" +
            "\n============" +
			"\nEquipped Weapon: " + this.EquippedWeapon +
			"\n============" +
			"\nProficiencies: \n" +
			this.Proficiencies.Replace(", ", "\n");
        return output;
    }
}