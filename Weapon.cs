using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Weapon : Item
{
    public string DamageType { get; protected set;}
    public string[] WeaponType { get; protected set; }
    public bool isRanged { get; protected set;}
    protected int Sides;
    protected int NumDice;
    protected int Bonus;
    public Weapon ()
    {
        this.UsesLeft = 10000;
        this.UseChance = 1.0f;
    }
    
    public override string SuccessMessage()
    {
        int dmg = this.RollDamage();
        return $"The {this.Name} does {dmg} of {DamageType} damage!";
    }
    public override string FailureMessage()
    {
        return  $"The {this.Name} fails to connect!";
    }
    public int RollDamage()
    {
        return Dice.Roll(this.NumDice, this.Sides, this.Bonus);
    }

    public override string Use(Creature c, int Slot) //Use applies to when in an inventory; a weapon is used to equip it
    {
        c.RemoveItem(Slot);
        c.EquipWeapon(this);

        return $"{c.Name} equipped the {this.Name}.";
    }

}