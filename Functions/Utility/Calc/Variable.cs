using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Calc
{
    public class Variable : ITokenObject
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Variable()
        {
            Name = string.Empty;
            Value = string.Empty;
        }

        public override string ToString()
        {
            return (this.Name);
        }
    }
}
