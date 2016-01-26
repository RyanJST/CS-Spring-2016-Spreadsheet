﻿// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016
//PS2 branch

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        List<string> formulaArray = new List<string>();
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>



        public Formula(string formula)
        {
            int leftParen = 0;
            int rightParen = 0;
            double test;
            int j = 0;
            
            foreach(string s in GetTokens(formula))
            {
                formulaArray.Add(s);
            }

            if(formula.Length == 0)
            {
                throw new FormulaFormatException("No tokens detected"); 
            }
            
            if(!char.IsLetterOrDigit(formula[0]) && formulaArray[0] != "(")
            {
                throw new FormulaFormatException("Starting token must be a: number, variable, or opening Parenthese");
            }

            if (!char.IsLetterOrDigit(formula[formula.Count() -1]) && formulaArray[formulaArray.Count()] != ")")
            {
                throw new FormulaFormatException("Ending token must be a: number, variable, or opening Parenthese");
            }

            if(formulaArray.Count(x => x == "(") != formulaArray.Count(x => x == ")"))
            {
                throw new FormulaFormatException("The number of open and close parenthesis do not match");
            }

            for(int i = 0; i < formulaArray.Count() -1; i++)
            {
                
                if(formulaArray[i] == "(" || formulaArray[i] == "+" || formulaArray[i] == "*" || formulaArray[i] == "-" || formulaArray[i] == "/")
                {
                    if (char.IsLetterOrDigit(formulaArray[i + 1][0]) || formulaArray[i + 1] == "(")
                    {
                        if (formulaArray[i] == "(")
                        {
                            leftParen++;
                        }
                    }
                    else
                    {
                        throw new FormulaFormatException("The only thing that can follow a parenthese or operator is a number, variable, or opening parenthese");
                    }
                }
                
                if ((char.IsLetterOrDigit(formulaArray[i][0]) || formulaArray[i] == ")"))
                {
                    if (char.IsLetter(formulaArray[i][0]) && double.TryParse(formulaArray[i + 1].ToString(), out test))
                    {

                    }
                    else if (formulaArray[i+1] == ")" || formulaArray[i + 1] == "+" || formulaArray[i + 1] == "*" || formulaArray[i + 1] == "-" || formulaArray[i + 1] == "/")
                    {
                        if (formulaArray[i] == ")")
                        {
                            rightParen++;
                        }
                    }
                    else
                    {
                        throw new FormulaFormatException("The only thing that can follow a number, variable, or opening parenthese is an operator or closing parenthese");
                    }
                }

                if(rightParen > leftParen)
                {
                    throw new FormulaFormatException("There are more closing parentheses than open parenthese at this point");
                }
            }
        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            double result = 0;
            foreach(string s in formulaArray)
            {
                result = 0;
                if (double.TryParse(s, out result))
                {
                    if (operatorStack.Count() != 0 && operatorStack.Peek() == "*")
                    {
                        operatorStack.Pop();
                        valueStack.Push(result * valueStack.Pop());
                    }
                    else if (operatorStack.Count() != 0 && operatorStack.Peek() == "/")
                    {
                        if (valueStack.Peek() != 0)
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() / result);
                        }
                        else
                        {
                            throw new FormulaEvaluationException("There is a division by zero in your formula.");
                        }
                    }

                    else
                    {
                        valueStack.Push(result);
                    }
                }
                else if (s == "+" || s == "-")
                {
                    if (operatorStack.Count() != 0 && operatorStack.Peek() == "+")
                    {
                        operatorStack.Pop();
                        valueStack.Push(valueStack.Pop() + valueStack.Pop());
                    }
                    else
                    {
                        if (operatorStack.Count() != 0 && operatorStack.Peek() == "-")
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() - valueStack.Pop());
                        }

                    }
                    operatorStack.Push(s);
                }

                else if (s == "*" || s == "/" || s == "(")
                {
                    operatorStack.Push(s);
                }

                else if (s == ")")
                {
                    if (operatorStack.Count() != 0 && (operatorStack.Peek() == "+" || s == "-"))
                    {
                        if (operatorStack.Peek() == "+")
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() + valueStack.Pop());
                        }
                        else
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() - valueStack.Pop());
                        }
                    }

                    operatorStack.Pop();

                    if (operatorStack.Count() != 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/"))
                    {
                        if (operatorStack.Peek() == "*")
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() * valueStack.Pop());
                        }
                        else
                        {
                            if (valueStack.Peek() != 0)
                            {
                                operatorStack.Pop();
                                valueStack.Push((valueStack.Pop()/valueStack.Pop()));
                            }
                            else
                            {
                                throw new FormulaEvaluationException("There is a division by zero in your formula.");
                            }
                        }
                    }
                }
                else //start of variable case
                {
                    try {
                        if (s != null && (operatorStack.Count() != 0 && operatorStack.Peek() == "*"))
                        {
                            operatorStack.Pop();
                            valueStack.Push(valueStack.Pop() * lookup(s));
                        }
                        else if (s != null && (operatorStack.Count() != 0 && operatorStack.Peek() == "/"))
                        {
                            if (valueStack.Peek() != 0)
                            {
                                operatorStack.Pop();
                                valueStack.Push(valueStack.Pop() / lookup(s) );
                            }
                            else
                            {
                                throw new FormulaEvaluationException("There is a division by zero in your formula.");
                            }
                        }
                        else if (s != null)
                        {
                            valueStack.Push(lookup(s));
                        }

                        }
                    catch(UndefinedVariableException)
                    {
                        throw new FormulaEvaluationException (s + " is undefined.");
                    }
                }
            }
            if (operatorStack.Count != 0)
            {
                if(operatorStack.Peek() == "+")
                {
                    return valueStack.Pop() + valueStack.Pop();
                }   
                else if(operatorStack.Peek() == "-")
                {
                    return valueStack.Pop() - valueStack.Pop();
                } 
            }

            return valueStack.Pop();
            
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(string formula)
        {
            // Patterns for individual tokens
            string lpPattern = @"\(";
            string rpPattern = @"\)";
            string opPattern = @"[\+\-*/]";
            string varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            string spacePattern = @"\s+";

            // Overall pattern
            string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(string variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(string message) : base(message)
        {
        }
    }
}
