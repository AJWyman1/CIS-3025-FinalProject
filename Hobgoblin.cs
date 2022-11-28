using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Hobgoblin : Level2Monster {

    public Hobgoblin() : base()
    {
        this.Name = "Hobgoblin";
        this.RepresentWith = 'H';
        this.Strength =     13;
        this.Dexterity =    12;
        this.Constitution = 12;
        this.Intelligence = 10;
        this.Wisdom =       10;
        this.Charisma =      9;
        this.HP =           11;
        this.MaxHP =        11;
        this.ArmorClass =   18;
        this.Darkvision = true;
        this.EquippedWeapon = new Longsword("Hobgoblin Longsword",1, 10, 1);
        this.ToHit = 3;
    }

}