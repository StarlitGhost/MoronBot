using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CwIRC;
using MoronBot.Utilities.Calc;

namespace MoronBot.Functions.Utility
{
    class Calc : Function
    {
        public Calc()
        {
            Help = "calc <expr>\t\t- Calculates the result of the given expression. " +
                "Supported operations, in decreasing order of precedence, are: ( ), ^, % / *, - +";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, @"^(calc(ulate)?)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0) // Expression given
                {
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
    }
}
