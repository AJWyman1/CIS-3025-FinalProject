using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Creature: IMobile, IActionable, IComparable
{
    /* Fields */

    protected int[] attributes;
    
    /* Properties */
    public String Name { get; protected set; }
    public Container<Item> Inventory {get; protected set;}
    public int HP { get; set; }
    public int MaxHP { get; protected set;}
    public int ArmorClass { get; protected set; }
    public bool Darkvision { get; protected set; } 
    public string Resistances { get; protected set; }
	public int GoldPouch { get; protected set;}
    public Weapon EquippedWeapon { get; protected set;}
    public int ToHit { get; protected set; }
    public ConsoleColor Color {get; set;}
    public int XP {get; protected set;}
    public int XPGiven {get; protected set;}
    public int Level {get; protected set;}
    public int DungeonLevel {get; set;}
    
    public bool IsAlive {get; protected set; }

    public int Strength
    {
        get
        {
            return attributes[0];
        }

        protected set
        {
            attributes[0] = value;
        }
    }

    public int Dexterity
    {
        get
        {
            return attributes[1];
        }

        protected set
        {
            attributes[1] = value;
        }
    }

    public int Constitution
    {
        get
        {
            return attributes[2];
        }

        protected set
        {
            attributes[2] = value;
        }
    }

    public int Intelligence
    {
        get
        {
            return attributes[3];
        }

        protected set
        {
            attributes[3] = value;
        }
    }

    public int Wisdom
    {
        get
        {
            return attributes[4];
        }

        protected set
        {
            attributes[4] = value;
        }
    }

    public int Charisma
    {
        get
        {
            return attributes[5];
        }

        protected set
        {
            attributes[5] = value;
        }
    }

    /* ILocatable Properties */

    public int X {get; set;}
    public int Y {get; set;}
    public Direction Facing {get; set;}
    public char RepresentWith { get; set; } //Char representation for print out in Console

    /* IActionable Properties */

    public int Initiative {get; set;}
    

    /* Constructors */

    public Creature()
    {
        this.attributes = new int[6];
        this.Darkvision = false;
        this.HP = 0;          
        this.Resistances = "";
        this.ToHit = 0;
        this.Inventory = new Container<Item>(2);
        this.IsAlive = true;
        this.XP = 0;
        this.XPGiven = 0;
        this.Level = 0;
        this.DungeonLevel = 0;
    }

    public Creature(int X, int Y) : base()
    {
        this.GoTo(X, Y);
    }

    /* methods */

    public void PickUpItem(Item i)
    {
        this.Inventory.Add(i);
    }

    public void GainXP(int Gained)
    {
        this.XP += Gained;
        do
        {
            if (this.XP >= 10)
            {
                this.LevelUp();
            }
        }while(this.XP >= 10);
    }

    public virtual void LevelUp()
    {
        for(int i = 0; i < this.attributes.Length; i++)
                {
                    this.attributes[i] += 1;
                }
                this.XP -= 10;
                this.Level += 1;
                this.XPGiven += 2;
    }


    public void PlaceItemOnGround(int Slot)
    {
        //Removes item from inventory and sets its location to the Creature's current location
        Item i = Inventory.Remove(Slot);
        i.X = this.X;
        i.Y = this.Y;
    }
    public bool InventoryFull()
    {
        for(int i = 0; i < this.Inventory.Entries.Length; i++)
        {
            if (this.Inventory.Entries[i] == null) //Any slot is null means Inventory is not full
            {
                return false; 
            }
        }
        return true; //Inventory full
    }


    /* It is necessary to override ToString() from the object class
       if we want a custom serialized Creature object as a string.
    */
    public override string ToString()
    {
        string output =
            this.Name +
            "\n============" +
            "\nSTR: " + this.Strength +
            "\nDEX: " + this.Dexterity +
            "\nCON: " + this.Constitution +
            "\nINT: " + this.Intelligence +
            "\nWIS: " + this.Wisdom +
            "\nCHA: " + this.Charisma +
            "\n============" +
            "\nHP: " + this.HP +
            "\nAC: " + this.ArmorClass +
            "\nResistances: " + this.Resistances +
            "\n============" +
            "\nDarkvision: " + this.Darkvision + "\n" + 
			"\n============" +
			"\nInventory:\n" +
			this.PrintInventory() +  "\n" +
            this.GoldPouch + "GP" +
            "\n============" +
			"\nEquipped Weapon:" + this.EquippedWeapon;
        return output;
    }

    public static explicit operator char(Creature c) => c.RepresentWith; //Allows for the Creature to be cast to char for print out in the Dungeon

    /* We are required to implement Attack because we implement the
       IActionable interface. However, because Creature is abstract
       and there is no default attack, we will make Attack abstract
       and define specific Attacks in subclasses by overriding. 
    */

    /*                  --------    IActionable methods     --------             */

    public virtual string Attack(Creature c)
	{
		int Damage;
		int Modifier;

		//AC Check
		int roll = Dice.D20();
		bool Crit = roll == 20; //Critical hit on nat 20

        if (this.HP <= 0)
        {
            this.IsAlive = false;
            return $"{this.Name} is dead and cannot fight";
        }

		if (this.EquippedWeapon != null) //Attack with a weapon if equipped 
		{
			if (this.EquippedWeapon.isRanged){ Modifier = this.AbilityModifier(this.Dexterity);} //Ranged weapon modifier is DEX
			else { Modifier = this.AbilityModifier(this.Strength);} //Melee weapon modifier is STR

			if ((roll + Modifier + this.ToHit) > c.ArmorClass || Crit) //Hit
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
		}else //unarmed attack if no weapon equipped
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
    public virtual string Defend()
	{
        if (this.HP <= 0)
        {
            this.IsAlive = false;
            this.RepresentWith = 'x';
        }
		return $"{this.Name} is defending";
	}

	public virtual string DefendAgainst(Creature c)
	{
		return $"{this.Name} defended against {c.Name}";
	}

	public virtual string Rest()
	{
		return $"{this.Name} rested";
	}

    public Item RemoveItem(int Slot, bool ZeroIndexed = true)
	{
		return this.Inventory.Remove(Slot);
	}

    public string Use(IUsable item, int Slot)
    {
        return item.Use(this, Slot);
    }

    /*                  --------    IMobile methods     --------                 */

    public void GoTo(int NewX, int NewY)
    {
        this.X = NewX;
        this.Y = NewY;
    }

    public void MoveNorth(int distance = 1)
    {
        this.X -= distance;
    }
    public void MoveEast(int distance = 1)
    {
        this.Y += distance;
    }
    public void MoveSouth(int distance = 1)
    {
        this.X += distance;
    }
    public void MoveWest(int distance = 1)
    {
        this.Y -= distance;
    }
    public void MoveNorthEast(int distance = 1)
    {
        this.MoveNorth(distance);
        this.MoveEast(distance);
    }
    public void MoveSouthEast(int distance = 1)
    {
        this.MoveSouth(distance);
        this.MoveEast(distance);
    }
    public void MoveSouthWest(int distance = 1)
    {   
        this.MoveSouth(distance);
        this.MoveWest(distance);
    }
    public void MoveNorthWest(int distance = 1)
    {
        this.MoveNorth(distance);
        this.MoveWest(distance);
    }

    public void MoveForward(int distance = 1)
    {
        //Move in the Direction Creature is facing

        if (this.Facing == Direction.NORTH)
        {
            this.MoveNorth(distance);
        }
        else if (this.Facing == Direction.NORTHEAST)
        {
            this.MoveNorthEast(distance);
        }
        else if (this.Facing == Direction.EAST)
        {
            this.MoveEast(distance);
        }
        else if (this.Facing == Direction.SOUTHEAST)
        {
            this.MoveSouthEast(distance);
        }
        else if (this.Facing == Direction.SOUTH)
        {
            this.MoveSouth(distance);
        }
        else if (this.Facing == Direction.SOUTHWEST)
        {
            this.MoveSouthWest(distance);
        }
        else if (this.Facing == Direction.WEST)
        {
            this.MoveWest(distance);
        }
        else if (this.Facing == Direction.NORTHWEST)
        {
            this.MoveNorthWest(distance);
        }
    }

    public void Turn(int Degrees)
    {
        this.Facing = (Direction)(((int)(this.Facing) + Degrees) % 360);
    }

    public void Face(Direction d)
    {
        this.Facing = d;
    }

    /* IComparable Sort by initiative */
    public int CompareTo(object obj)
    {
        if (obj == null){return 1;}

        Creature otherItem = obj as Creature;
        if (otherItem != null)
            return this.Initiative.CompareTo(otherItem.Initiative); //Sort by initiative
        else
            throw new ArgumentException("Not a Creature.");
    }

    /*              --------   Class Methods  --------                       */

    public int AbilityModifier(int ability)
	{
		return (int)Math.Floor((double)((ability - 10) / 2)); //Math.Floor to correctly round down
	}
    public void RollInitiative()
    {
        this.Initiative = Dice.D20() + this.AbilityModifier(this.Dexterity);
    }
    
    public string PrintInventory()
	{
		return this.Inventory.ToString();
	}

    public void EquipWeapon(Weapon w)
    {
        if (this.EquippedWeapon != null)
        {
            if (this.InventoryFull())
            {
                this.EquippedWeapon.X = this.X;
                this.EquippedWeapon.Y = this.Y;
            }else
            {
                this.PickUpItem(this.EquippedWeapon);
            }
        }
        this.EquippedWeapon = w;
    }

    public void UnequipWeapon()
    {
        if (this.InventoryFull())
        {
            this.PickUpItem(this.EquippedWeapon);
        
        }
        
    }
    public string Heal(int h)
    {
        this.HP += h;
		if (this.HP > this.MaxHP){this.HP = this.MaxHP;} //
		return $"{this.Name} 1";
    }
}
