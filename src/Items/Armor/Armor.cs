using System;

abstract class Armor : Item
{
    public bool StealthDisadvantage {get; private set;}
    public Armor ()
    {
        this.UsesLeft = 1000;
        this.UseChance = 1.0f;
        this.Color = ConsoleColor.Green;
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

}