using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Bandit : Level1Monster {

    public Bandit() : base()
    {
        this.Name = "Bandit";
        this.RepresentWith = 'B';
        this.Strength =     11;
        this.Dexterity =    12;
        this.Constitution = 12;
        this.Intelligence = 10;
        this.Wisdom =       10;
        this.Charisma =     10;
        this.HP =           11;
        this.MaxHP =        11;
        this.ArmorClass =   12;
        this.Darkvision = false;
        this.ToHit = 3;
        this.EquippedWeapon = new Schimitar();
    }
}