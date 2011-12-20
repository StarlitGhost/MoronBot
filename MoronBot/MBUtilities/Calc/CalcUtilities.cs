using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBUtilities.Calc
{
    public static class CalcUtilities
    {
        public static Token CreateOperatorToken(char op)
        {
            Operator operatorObject = AllOperators.Find("" + op);
            return (new Token(TokenType.Operator, operatorObject));
        }

        public static Token CreateNumberConstantToken(string numberString)
        {
            Constant constant = new Constant() { Value = numberString };
            return (new Token(TokenType.Constant, constant));
        }

        public static bool IsOpenParenthesis(Token token)
        {
            return (token.Type == TokenType.Operator && token.LinearToken.Equals(AllOperators.Find("(").SymbolText));
        }

        public static bool IsCloseParenthesis(Token token)
        {
            return (token.Type == TokenType.Operator && token.LinearToken.Equals(AllOperators.Find(")").SymbolText));
        }

        public static bool IsArithmeticOperator(Token token)
        {
            return (token.Type == TokenType.Operator && !IsOpenParenthesis(token) && !IsCloseParenthesis(token));
        }

        //checks whether the input expression contains only number-constants and operators.
        public static bool IsArithmeticExpression(Expression expression)
        {
            foreach (Token token in expression.Tokens)
            {
                if (!(token.Type == TokenType.Constant || token.Type == TokenType.Operator))
                {
                    return (false);
                }
            }

            return (true);
        }

        public static bool InfixToPostfix(Expression inputExpression, out Expression postfixExpression)
        {
            List<Token> postfixTokens = new List<Token>();
            Stack<Token> postfixStack = new Stack<Token>();

            //process all tokens in input-expression, one by one
            foreach (Token token in inputExpression.Tokens)
            {
                if (token.Type == TokenType.Constant) //handle constants
                {
                    postfixTokens.Add(token);
                }
                else if (token.Type == TokenType.Variable) //handle variables
                {
                    postfixTokens.Add(token);
                }
                else if (token.Type == TokenType.Operator) //handle operators
                {
                    if (CalcUtilities.IsOpenParenthesis(token)) //handle open-parenthesis
                    {
                        postfixStack.Push(token);
                    }
                    else if (CalcUtilities.IsCloseParenthesis(token)) //handle close-parenthesis
                    {
                        //pop all operators off the stack onto the output (until left parenthesis)
                        while (true)
                        {
                            if (postfixStack.Count == 0)
                            {
                                postfixExpression = null; //error: mismatched parenthesis
                                return (false);
                            }

                            Token top = postfixStack.Pop();
                            if (CalcUtilities.IsOpenParenthesis(top)) break;
                            else postfixTokens.Add(top);
                        }
                    }
                    else //handle arithmetic operators
                    {
                        Operator currentOperator = AllOperators.Find(token.LinearToken);

                        if (postfixStack.Count > 0)
                        {
                            Token top = postfixStack.Peek();
                            if (CalcUtilities.IsArithmeticOperator(top))
                            {
                                Operator stackOperator = AllOperators.Find(top.LinearToken);
                                if ((currentOperator.Associativity == OperatorAssociativity.LeftToRight &&
                                     currentOperator.PrecedenceLevel >= stackOperator.PrecedenceLevel) ||
                                    (currentOperator.Associativity == OperatorAssociativity.RightToLeft &&
                                     currentOperator.PrecedenceLevel > stackOperator.PrecedenceLevel)) //'>' operator implies less precedence
                                {
                                    postfixStack.Pop();
                                    postfixTokens.Add(top);
                                }
                            }
                        }

                        postfixStack.Push(token); //push operator to stack
                    }
                }
            }

            //after reading all tokens, pop entire stack to output
            while (postfixStack.Count > 0)
            {
                Token top = postfixStack.Pop();
                if (CalcUtilities.IsOpenParenthesis(top) || CalcUtilities.IsCloseParenthesis(top))
                {
                    postfixExpression = null; //error: mismatched parenthesis
                    return (false);
                }
                else
                {
                    postfixTokens.Add(top);
                }
            }

            postfixExpression = new Expression() { Tokens = postfixTokens };
            return (true);
        }
    }
}
