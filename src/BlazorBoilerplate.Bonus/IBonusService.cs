using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Bonus
{
    public interface IBonusService
    {
        int CalculateBonus(int input);
        string AddStuff(string text);
    }
}
