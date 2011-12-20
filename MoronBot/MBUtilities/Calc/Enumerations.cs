using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBUtilities.Calc
{
    public enum TokenType
    {
        None = 0,
        Variable = 1,
        Constant = 2,
        Operator = 3
    }

    public enum OperatorAssociativity
    {
        None = 0,
        LeftToRight = 1,
        RightToLeft = 2
    }
}
