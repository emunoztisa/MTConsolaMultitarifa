using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Clases
{
    public class Combos
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public Combos(string display, int idx)
        {
            Text = display;
            Value = idx;
        }
    }
}
