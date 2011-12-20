using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBUtilities.Calc
{
    public class Operator : ITokenObject
    {
        public string SymbolText { get; set; }
        public int OperandCount { get; set; }

        /// <summary>
        /// Indicates precedence over other operators in the expression. Operators with higher precedence
        /// are evaluated before operators with lower precedence. The precedence-level value starts with 1 (highest).
        /// (i.e. the lesser the number, the higher the precedence).
        /// </summary>
        public int PrecedenceLevel { get; set; }

        public OperatorAssociativity Associativity { get; set; }

        public Func<double, double, double> Execute;

        public Operator()
        {
            SymbolText = string.Empty;
            OperandCount = 0;
            PrecedenceLevel = -1;
            Associativity = OperatorAssociativity.None;
            Execute = null;
        }

        public override string ToString()
        {
            return (SymbolText);
        }
    }

    public static class AllOperators
    {
        public static List<Operator> Operators { get; set; }

        public static Operator Find(string symbolText)
        {
            foreach (Operator op in Operators)
            {
                if (op.SymbolText.Equals(symbolText))
                {
                    return (op);
                }
            }
            return (null);
        }
    }
}
