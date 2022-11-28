using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GiantRat : Level1Monster {

    public GiantRat() : base()
    {
        this.Name = "Giant Rat";
        this.RepresentWith = 'R';
        this.Strength = 7;
        this.Dexterity = 15;
        this.Constitution = 11;
        this.Intelligence = 2;
        this.Wisdom = 10;
        this.Charisma = 4;
        this.HP = 7;
        this.MaxHP = 7;
        this.ArmorClass = 12;
        this.Darkvision = true;
    }

    public string Bite(Creature c)
    {
        //Melee Weapon Attack:+4 to hit, reach 5ft., one target. Hit: 4 (1d4 + 2) piercing damage.
        int Modifier = this.AbilityModifier(this.Strength);
        int roll = Dice.D20();
        bool Crit = roll == 20;
        int Damage;

        if ((roll + Modifier + 4) > c.ArmorClass || Crit) //Hit
		{
			Damage = Dice.Roll(1, 4, 2) + Modifier;
			if (Crit){ Damage += Dice.D4();}
			c.HP -= Damage;
			return $"The {this.Name}({this.HP}/{this.MaxHP}){(Crit ? " CRITICALLY" : "")} bites {c.Name}({c.HP}/{c.MaxHP}) for {Damage} points of piercing damage!";
		}else 
		{
			return $"The {this.Name}({this.HP}/{this.MaxHP}) tries to bite {c.Name}({c.HP}/{c.MaxHP}) but MISSES!";
		}
    }

    public override string Attack(Creature c)
    {
        if (this.HP <= 0)
        {
            return $"{this.Name} is dead and cannot fight";
        }
        return this.Bite(c);
    }
}