using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoronBot.Utilities.Calc
{
    public class Token
    {
        public TokenType Type { get; set; }
        public ITokenObject TokenObject { get; set; }
        public int Index { get; set; }

        //returns printable token
        public string LinearToken
        {
            get
            {
                return (this.TokenObject.ToString());
            }
        }

        public Token()
        {
            Type = TokenType.None;
            TokenObject = null;
            Index = -1;
        }

        public Token(TokenType type, ITokenObject tokenObject)
        {
            this.Type = type;
            this.TokenObject = tokenObject;
            this.Index = -1;
        }

        public static Token Resolve(string text)
        {
            //check if a number-constant
            double numberValue;
            if (double.TryParse(text, out numberValue))
            {
                Constant constant = new Constant() { Value = text };
                return (new Token(TokenType.Constant, constant));
            }

            //check if operator
            Operator op = AllOperators.Find(text);
            if (op != null)
            {
                return (new Token(TokenType.Operator, op));
            }

            //this token must be a variable
            Variable variable = new Variable() { Name = text };
            return (new Token(TokenType.Variable, variable));
        }
    }
}
