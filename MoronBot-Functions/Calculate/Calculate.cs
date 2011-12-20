using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using MBUtilities.Calc;

namespace Utility
{
    public class Calculate : Function
    {
        List<Operator> Operators { get; set; }

        public Calculate()
        {
            Help = "calc <expr> - Calculates the result of the given expression. " +
                "Supported operations, in decreasing order of precedence, are: ( ), ^, % / *, - +";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            Operators = PopulateOperators();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, @"^(calc(ulate)?)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0) // Expression given
                {
                    AllOperators.Operators = Operators;

                    string stringExpr = Regex.Replace(message.Parameters, @"\w+", MathsConstants);

                    Expression expr = Tokenizer.Split(stringExpr);

                    List<Token> postfixTokens = new List<Token>();
                    bool isError = false;
                    int errorTokenIndex = -1;
                    string errorMessage = string.Empty;

                    Evaluator.Validate(expr, ref postfixTokens, out isError, out errorTokenIndex, out errorMessage);

                    string output = "";

                    if (CalcUtilities.IsArithmeticExpression(expr))
                    {
                        if (!isError)
                        {
                            double result = Evaluator.EvaluateBasic(postfixTokens);
                            output = "Result: " + result.ToString();
                        }
                        else
                        {
                            output = "Error at Token " + (errorTokenIndex + 1) + " ['" + expr.Tokens[errorTokenIndex].LinearToken + "']. " + errorMessage;
                        }
                    }
                    else
                    {
                        output = "Expression is not arithmetic.";
                    }

                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, output, message.ReplyTo) };
                }
                else // No expr given
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give an expression to calculate!", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }

        static string MathsConstants(Match match)
        {
            switch (match.Value)
            {
                case "pi":
                    return "3.14159265358979323846264338327950288419716939937510";
                case "e":
                    return "2.71828182845904523536028747135266249775724709369995";
                case "g":
                    return "9.80665";
                case "G":
                    return Math.Pow(6.67428, 11).ToString();
                case "c":
                    return "299792458";
            }

            return match.Value;
        }

        static List<Operator> PopulateOperators()
        {
            List<Operator> opList = new List<Operator>();

            opList.Add(new Operator()
            {
                SymbolText = "+",
                OperandCount = 2,
                PrecedenceLevel = 5,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x + y
            });

            opList.Add(new Operator()
            {
                SymbolText = "-",
                OperandCount = 2,
                PrecedenceLevel = 5,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x - y
            });

            opList.Add(new Operator()
            {
                SymbolText = "*",
                OperandCount = 2,
                PrecedenceLevel = 4,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x * y
            });

            opList.Add(new Operator()
            {
                SymbolText = "/",
                OperandCount = 2,
                PrecedenceLevel = 4,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x / y
            });

            opList.Add(new Operator()
            {
                SymbolText = "%",
                OperandCount = 2,
                PrecedenceLevel = 4,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x % y
            });

            opList.Add(new Operator()
            {
                SymbolText = "^",
                OperandCount = 2,
                PrecedenceLevel = 3,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => Math.Pow(x, y)
            });

            opList.Add(new Operator()
            {
                SymbolText = "_",
                OperandCount = 1,
                PrecedenceLevel = 2,
                Associativity = OperatorAssociativity.RightToLeft,
                Execute = (x, y) => -x
            });

            opList.Add(new Operator()
            {
                SymbolText = "(",
                OperandCount = 0,
                PrecedenceLevel = 1,
                Associativity = OperatorAssociativity.LeftToRight
            });

            opList.Add(new Operator()
            {
                SymbolText = ")",
                OperandCount = 0,
                PrecedenceLevel = 1,
                Associativity = OperatorAssociativity.LeftToRight
            });

            opList.Add(new Operator()
            {
                SymbolText = "X",
                OperandCount = 0,
                PrecedenceLevel = 0,
                Associativity = OperatorAssociativity.LeftToRight
            });

            return opList;
        }
    }
}
