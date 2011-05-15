using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoronBot.Utilities.Calc
{
    public class Tokenizer
    {
        public static Expression Split(string expressionString)
        {
            List<Token> tokens = new List<Token>();

            //read the input-expression letter-by-letter and build tokens
            string alphaNumericString = string.Empty;

            Token lastOp = CalcUtilities.CreateOperatorToken('X');
            for (int index = 0; index < expressionString.Length; index++)
            {
                char currentChar = expressionString[index];

                if (AllOperators.Find("" + currentChar) != null) //if operator
                {
                    if (alphaNumericString.Length > 0)
                    {
                        tokens.Add(Token.Resolve(alphaNumericString));
                        alphaNumericString = string.Empty;
                    }

                    if (lastOp != null && (((Operator)lastOp.TokenObject).Symbol == OperatorSymbol.None || ((Operator)lastOp.TokenObject).Symbol != OperatorSymbol.CloseParenthesis))
                    {
                        if (currentChar == '-')
                        {
                            currentChar = '_';
                        }
                    }

                    Token op = CalcUtilities.CreateOperatorToken(currentChar);
                    lastOp = op;
                    tokens.Add(op);
                }
                else if (Char.IsLetterOrDigit(currentChar) || currentChar == '.') //if alphabet or digit or dot
                {
                    alphaNumericString += currentChar;
                    lastOp = null;
                }
            }

            //check if any token at last
            if (alphaNumericString.Length > 0)
            {
                tokens.Add(Token.Resolve(alphaNumericString));
            }

            return (new Expression() { Tokens = tokens });
        }
    }
}
