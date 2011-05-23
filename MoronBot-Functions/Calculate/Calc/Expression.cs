using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Calc
{
    public class Expression
    {
        public List<Token> Tokens { get; set; }

        public Expression()
        {
            Tokens = new List<Token>();
        }

        public override string ToString()
        {
            return (string.Join("  ", (from token in this.Tokens
                                       select token.LinearToken).ToArray()));
        }
    }
}
