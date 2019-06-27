using System;

namespace BlazorBoilerplate.Bonus
{
    internal class BonusService : IBonusService
    {
        // TODO: inject something

        public int CalculateBonus(int input)
        {
            var rnd = new Random();
            return input + rnd.Next(1, 11);
        }
    }
}