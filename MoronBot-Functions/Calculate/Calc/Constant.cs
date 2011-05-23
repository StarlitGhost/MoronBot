using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Calc
{
    public class Constant : ITokenObject
    {
        public string Value { get; set; }

        public Constant()
        {
            Value = string.Empty;
        }

        public override string ToString()
        {
            return (this.Value);
        }
    }
}
