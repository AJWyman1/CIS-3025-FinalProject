using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Orc : Level2Monster {

    public Orc() : base()
    {
        this.Name = "Orc";
        this.RepresentWith = 'O';
        this.Strength =     16;
        this.Dexterity =    12;
        this.Constitution = 16;
        this.Intelligence =  7;
        this.Wisdom =       11;
        this.Charisma =     10;
        this.HP =           15;
        this.MaxHP =        15;
        this.ArmorClass =   15;
        this.Darkvision = true;
        this.EquippedWeapon = new Greataxe("Orcish Greataxe", 12, 1, 3);
        this.ToHit = 5;
        this.Color = ConsoleColor.Green;
    }

}