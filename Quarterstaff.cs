using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Quarterstaff : Weapon
{
    public Quarterstaff ()
    {
        this.DamageType = "bludgeoning";
        this.Name = "Quarterstaff";
        this.isRanged = false;
        this.WeaponType = new string[] {"quarterstaff", "simple weapon"};
        this.Sides = 6;
        this.NumDice = 1;
        this.Bonus = 0;
        this.RepresentWith = 'q';
    }

}