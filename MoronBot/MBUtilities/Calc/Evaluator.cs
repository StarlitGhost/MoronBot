using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBUtilities.Calc
{
    public static class Evaluator
    {
        public static double EvaluateBasic(List<Token> postfixTokens)
        {
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in postfixTokens)
            {
                if (token.Type == TokenType.Constant)
                {
                    stack.Push(token);
                }
                else if (token.Type == TokenType.Operator)
                {
                    Token top;
                    switch (((Operator)token.TokenObject).OperandCount)
                    {
                        case 1:
                            top = stack.Pop();

                            stack.Push(EvaluateArithmetic(top, CalcUtilities.CreateNumberConstantToken("0"), token));
                            break;
                        case 2:
                            Token temp = stack.Pop();
                            top = stack.Pop();

                            stack.Push(EvaluateArithmetic(top, temp, token));
                            break;
                    }
                }
            }

            double result = double.NaN;
            if (stack.Count == 1)
            {
                Token top = stack.Pop();
                result = double.Parse(top.LinearToken);
            }

            return result;
        }

        private static Token EvaluateArithmetic(Token token1, Token token2, Token operatorToken)
        {
            //check if both tokens are numeric constants
            double number1 = double.NaN;
            double number2 = double.NaN;
            bool validNumber1 = (token1.Type == TokenType.Constant) && double.TryParse(token1.LinearToken, out number1);
            bool validNumber2 = (token2.Type == TokenType.Constant) && double.TryParse(token2.LinearToken, out number2);

            if (validNumber1 && validNumber2)
            {
                Operator op = AllOperators.Find(operatorToken.LinearToken);
                double returnValue = double.NaN;

                returnValue = op.Execute(number1, number2);

                returnValue = Math.Round(returnValue, 4);
                Constant returnValueConstant = new Constant() { Value = returnValue.ToString() };
                return (new Token(TokenType.Constant, returnValueConstant));
            }
            else
            {
                return (CalcUtilities.CreateNumberConstantToken("1"));
            }
        }

        public static void Validate(Expression expression, ref List<Token> postfixTokens, out bool isError, out int errorTokenIndex, out string errorMessage)
        {
            //initialize index of each token
            int tokenIndex = 0;
            foreach (Token token in expression.Tokens)
            {
                token.Index = tokenIndex;
                tokenIndex += 1;
            }

            Stack<Token> postfixStack = new Stack<Token>();
            foreach (Token token in expression.Tokens)
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
                                isError = true;
                                errorTokenIndex = token.Index;
                                errorMessage = "Mismatched parenthesis.";
                                return;
                            }

                            Token top = postfixStack.Pop();

                            if (CalcUtilities.IsOpenParenthesis(top))
                            {
                                break;
                            }
                            else
                            {
                                postfixTokens.Add(top);
                            }
                        }
                    }
                    else //handle other operators (usually arithmetic)
                    {
                        Operator operator1 = AllOperators.Find(token.LinearToken);

                        while (true)
                        {
                            if (postfixStack.Count == 0)
                            {
                                break;
                            }

                            Token top = postfixStack.Peek();
                            if (CalcUtilities.IsArithmeticOperator(top))
                            {
                                bool readyToPopOperator2 = false;
                                Operator operator2 = AllOperators.Find(top.LinearToken);
                                if (operator1.Associativity == OperatorAssociativity.LeftToRight && operator1.PrecedenceLevel >= operator2.PrecedenceLevel) //'>' operator implies less precedence
                                {
                                    readyToPopOperator2 = true;
                                }
                                else if (operator1.Associativity == OperatorAssociativity.RightToLeft && operator1.PrecedenceLevel > operator2.PrecedenceLevel)
                                {
                                    readyToPopOperator2 = true;
                                }

                                if (readyToPopOperator2)
                                {
                                    postfixStack.Pop();
                                    postfixTokens.Add(top);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        postfixStack.Push(token);
                    }
                }
            }

            //pop entire stack to output
            while (postfixStack.Count > 0)
            {
                Token top = postfixStack.Pop();

                if (CalcUtilities.IsOpenParenthesis(top) || CalcUtilities.IsCloseParenthesis(top))
                {
                    isError = true;
                    errorTokenIndex = top.Index;
                    errorMessage = "Mismatched Parenthesis.";
                    return;
                }
                else
                {
                    postfixTokens.Add(top);
                }
            }

            Stack<Token> evaluateStack = new Stack<Token>();
            foreach (Token token in postfixTokens)
            {
                if (token.Type == TokenType.Constant)
                {
                    evaluateStack.Push(token);
                }
                else if (token.Type == TokenType.Variable)
                {
                    evaluateStack.Push(token);
                }
                else if (token.Type == TokenType.Operator)
                {
                    Token top;
                    switch (((Operator)token.TokenObject).OperandCount)
                    {
                        case 1:
                            if (evaluateStack.Count >= 1)
                            {
                                top = evaluateStack.Pop();

                                //add a dummy constant as result of arithmetic evaluation
                                Token dummyConstantToken = CalcUtilities.CreateNumberConstantToken("1");
                                evaluateStack.Push(dummyConstantToken);
                            }
                            else
                            {
                                isError = true;
                                errorTokenIndex = token.Index;
                                errorMessage = "Missing Operand for Operator '" + token.LinearToken + "'.";
                                return;
                            }
                            break;
                        case 2:
                            if (evaluateStack.Count >= 2)
                            {
                                Token temp = evaluateStack.Pop();
                                top = evaluateStack.Pop();

                                //add a dummy constant as result of arithmetic evaluation
                                Token dummyConstantToken = CalcUtilities.CreateNumberConstantToken("1");
                                evaluateStack.Push(dummyConstantToken);
                            }
                            else
                            {
                                isError = true;
                                errorTokenIndex = token.Index;
                                errorMessage = "Missing Operand for Operator '" + token.LinearToken + "'.";
                                return;
                            }
                            break;
                    }
                }
            }

            if (evaluateStack.Count == 1) //there should be exactly one token left in evaluate-stack
            {
                //leave the last token intact (as we are not evaluating to find the result)
                isError = false;
                errorTokenIndex = -1;
                errorMessage = string.Empty;
            }
            else
            {
                isError = true;
                errorTokenIndex = expression.Tokens.Count - 1;
                errorMessage = "Incomplete Expression.";
            }
        }
    }
}
