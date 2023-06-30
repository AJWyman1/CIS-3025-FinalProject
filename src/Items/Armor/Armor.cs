using System;

abstract class Armor : Item
{
    public bool StealthDisadvantage { get; protected set; }
    public int ArmorClass;
    public bool DexModifier;
    public int MaxMod;
    public string[] ArmorType { get; protected set; }

    //Strength requirement?

    public Armor()
    {
        this.UsesLeft = 1000;
        this.UseChance = 1.0f;
        this.Color = ConsoleColor.Green;
        this.RepresentWith = 'a';
        this.MaxMod = 100;
    }

    public int GetAC(Creature c)
    {
        if (this.DexModifier)
        {
            int mod = c.AbilityModifier("dex");
            if (mod > this.MaxMod)
            {
                mod = this.MaxMod;
            }
            return this.ArmorClass + mod;
        }
        return this.ArmorClass;
    }

    public override string SuccessMessage()
    {
        return $"The {this.Name} is equipped";
    }
    public override string FailureMessage()
    {
        return $"The {this.Name} is unequipped";
    }

    public override string Use(Creature c, int Slot)
    {
        c.RemoveItem(Slot);
        //c.EquipArmor(this);

        return $"{c.Name} equipped the {this.Name}.";
    }

    public override string ToString()
    {
        return $"{this.Name} - AC: {this.ArmorClass}";
    }

}