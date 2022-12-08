using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Mimic : Level3Monster {

    private bool falseForm;
    public Mimic() : base()
    {
        this.Name = "Mimic";
        this.RepresentWith = 'C';
        this.Strength = 17;
        this.Dexterity = 12;
        this.Constitution = 15;
        this.Intelligence = 5;
        this.Wisdom = 13;
        this.Charisma = 8;
        this.HP = 58;
        this.MaxHP = 58;
        this.ArmorClass = 12;
        this.Darkvision = true;
        this.falseForm = true;
        this.Color = ConsoleColor.Yellow;
    }

    

    public string Bite(Creature c)
    {
        //Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 7 (1d8 + 3) piercing damage plus 4 (1d8) acid damage.
        int Modifier = this.AbilityModifier(this.Strength);
        int roll = Dice.D20();
        bool Crit = roll == 20;
        int Damage;

        if ((roll + Modifier + 5) > c.ArmorClass || Crit) //Hit
		{
			Damage = Dice.Roll(1, 8, 3) + Modifier;
			if (Crit){ Damage += Dice.D8();}
			c.HP -= Damage;
            int AcidDmg = Dice.D8();
            c.HP -= AcidDmg;
            c.Defend();
			return $"The {this.Name}({this.HP}/{this.MaxHP}){(Crit ? " CRITICALLY" : "")} bites {c.Name}({c.HP}/{c.MaxHP}) for {Damage} points of piercing damage and {AcidDmg} points of acid damage!";
		}else 
		{
			return $"The {this.Name}({this.HP}/{this.MaxHP}) tries to bite {c.Name}({c.HP}/{c.MaxHP}) but MISSES!";
		}
    }

    public string ShapeChange()
    {
        this.falseForm = false;
        this.RepresentWith = 'M';
        return "The chest opens to reveal many rows of sharp teeth! That's no chest! Its a Mimic!!";

    }

    public override string Attack(Creature c)
    {
        if(this.falseForm)
        {
            return this.ShapeChange();
        }
        if (this.HP <= 0)
        {
            return $"{this.Name} is dead and cannot fight";
        }
        return this.Bite(c);
    }
}