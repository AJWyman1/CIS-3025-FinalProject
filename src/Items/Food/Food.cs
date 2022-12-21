class Food : Item
{
    FoodTypes Type;
    public Food (FoodTypes Type)
    {
        this.Type = Type;
        this.RepresentWith = 'f';
        this.Color = System.ConsoleColor.Cyan;
        this.Name = GetName();
    }

    public string GetName()
    {
        if(this.Type == FoodTypes.Apple)
        {
            return "Apple";
        }
        else
        if(this.Type == FoodTypes.PizzaSlice)
        {
            return "Slice of Pizza";
        }
        else
        if(this.Type == FoodTypes.IceCream)
        {
            return "Icecream cone";
        }
        else
        if(this.Type == FoodTypes.RadishSalad)
        {
            return "Radish salad";
        }
        else
        if(this.Type == FoodTypes.DeviledEggs)
        {
            return "Deviled eggs";
        }
        else
        if(this.Type == FoodTypes.Kebab)
        {
            return "Kebab";
        }
        else
        if(this.Type == FoodTypes.VeggieBurger)
        {
            return "Veggie Burger";
        }
        else
        if(this.Type == FoodTypes.Spaghetti)
        {
            return "Spaghetti";
        }
        else
        if(this.Type == FoodTypes.Chicken)
        {
            return "Chicken";
        }
        else
        if(this.Type == FoodTypes.Stew)
        {
            return "Stew";
        }
        else
        {
            return "Rare Candy";
        }
    }


    public override string Use(Creature c, int Slot)
    {
        int healed = 0;
        if(this.Type == FoodTypes.Apple)
        {
            healed = (int)(c.MaxHP * .1);
        }
        else
        if(this.Type == FoodTypes.PizzaSlice)
        {
            healed = (int)(c.MaxHP * .2);
        }
        else
        if(this.Type == FoodTypes.IceCream)
        {
            healed = (int)(c.MaxHP * .3);
        }
        else
        if(this.Type == FoodTypes.RadishSalad)
        {
            healed = (int)(c.MaxHP * .4);
        }
        else
        if(this.Type == FoodTypes.DeviledEggs)
        {
            healed = (int)(c.MaxHP * .5);
        }
        else
        if(this.Type == FoodTypes.Kebab)
        {
            healed = (int)(c.MaxHP * .6);
        }
        else
        if(this.Type == FoodTypes.VeggieBurger)
        {
            healed = (int)(c.MaxHP * .7);
        }
        else
        if(this.Type == FoodTypes.Spaghetti)
        {
            healed = (int)(c.MaxHP * .8);
        }
        else
        if(this.Type == FoodTypes.Chicken)
        {
            healed = (int)(c.MaxHP * .9);
        }
        else
        if(this.Type == FoodTypes.Stew)
        {
            healed = c.MaxHP;
        }
        else
        {
            c.GainXP(10);
            return $"{c.Name} ate the Rare Candy and gained a level!";
        }

        c.Heal(healed);

        return $"{c.Name} ate the {this.Name} regained {healed} hit points and is currently {c.HP}/{c.MaxHP}!";
    }


    public override string SuccessMessage()
    {
        return "Success";
    }
    public override string FailureMessage()
    {
        return  $"Fail.";
    }

}