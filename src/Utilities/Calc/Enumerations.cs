using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoronBot.Utilities.Calc
{
    public enum TokenType
    {
        None = 0,
        Variable = 1,
        Constant = 2,
        Operator = 3
    }

    public enum OperatorSymbol
    {
        None = 0,
        Add = 1,
        Subtract = 2,
        Multiply = 3,
        Divide = 4,
        Modulus = 5,
        Exponent = 6,
        UnaryMinus = 7,
        OpenParenthesis = 8,
        CloseParenthesis = 9
    }

    public enum OperatorAssociativity
    {
        None = 0,
        LeftToRight = 1,
        RightToLeft = 2
    }
}
