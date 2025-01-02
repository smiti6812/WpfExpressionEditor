using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor
{
    public static class ExpressionEditorHelper<T>
    {
        public static (Expression<Func<T, bool>> Exp, bool CheckOk, string Msg, Func<T, bool> Func) ParseStringToExpression(string filter)
        {
            try
            {
                filter = "c => " + filter;
                Expression<Func<T, bool>> exp = DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, filter, new object[0]);
                Func<T, bool> func = exp.Compile();
                return (exp, true, "", func);
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message, null);
            }
        }

        public static (bool checkResult, bool noError) EvaluateQuery(T item, string filter)
        {
            (_, bool checkOk, _, Func<T, bool> func) = ParseStringToExpression(filter);
            IList<T> items = new List<T>() { item };
            bool checkResult = checkOk && items.Any(func);
            return (checkResult, checkOk);
        }

        public static Func<T, bool> GetQueryFunc(string filter)
        {
            filter = "c => " + filter;
            Expression<Func<T, bool>> exp = DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, filter, new object[0]);
            return exp.Compile();
        }

        public static Dictionary<string, string> GetPropertiesAndValuesOfGenericType(T Item)
        {
            if (Item == null)
            {
                return new Dictionary<string, string>();
            }

            Type t = Item.GetType();
            PropertyInfo[] arrPropertyInfo = t.GetProperties();
            var propCollection = new Dictionary<string, string>();
            foreach (PropertyInfo prop in arrPropertyInfo)
            {
                object propVal = prop.GetValue(Item, null);
                if (propVal != null)
                {
                    propCollection.Add(prop.Name, propVal.ToString());
                }
            }

            return propCollection;
        }

        public static string CheckFormulaAndReturnError(string expression, Enum errorKey) => !CheckFormula(expression).checkOk ? EditorHelper.errorMessages[errorKey] : "";

        public static (bool checkOk, string msg) CheckFormula(string formula)
        {
            (_, bool checkOk, string msg, _) = ParseStringToExpression(formula);
            return (checkOk, msg);
        }

        public static bool CheckEndsOrStartsWithAndOr(string exprText)
        {
            string expr = exprText.Trim();
            return expr.EndsWith(EditorHelper.Or) || expr.EndsWith(EditorHelper.And) ||
                expr.StartsWith(EditorHelper.Or) || expr.StartsWith(EditorHelper.And);
        }

        public static bool CheckLogicalOperators(IEnumerable<string> keys, string exprText)
        {
            return !exprText.EndsWith(EditorHelper.Or) && !exprText.EndsWith(EditorHelper.And) &&
                !exprText.EndsWith(EditorHelper.Equal) && !exprText.EndsWith(EditorHelper.NotEqual)
                 && !keys.Any(exprText.EndsWith) && !string.IsNullOrEmpty(exprText);
        }

        public static bool CheckFieldAndValuesNotEmpty(string selectedField, string propertyValue) => !string.IsNullOrEmpty(selectedField) && propertyValue != null;

        public static (string exprText, string errorText) CanInsertBrackets(string exprText, int selectionStart, int selectionLength)
        {
            string errorText = CheckFormulaAndReturnError(exprText.Substring(selectionStart, selectionLength),
              Operations.InsertBrackets);
            string newExprText = string.IsNullOrEmpty(errorText)
                ? exprText.Insert(selectionStart, "(").Insert(selectionStart + selectionLength + 1, ")")
                : exprText;

            return (newExprText, errorText);
        }

        public static (string exprText, string errorText) CanAddNotFunction(string _function, int selectionStart, int selectionLength, string expressionText)
        {
            if (selectionLength >= 0 || (selectionStart > 0 && selectionStart < expressionText.Length))
            {
                string exprText = expressionText.Insert(selectionStart, _function);
                return TestExpression(exprText, expressionText, Operations.NotFunctionInsert);
            }
            else if (string.IsNullOrEmpty(expressionText) || CheckEndsOrStartsWithAndOr(expressionText))
            {
                string errorText = "";
                return (string.IsNullOrEmpty(expressionText) ? $"{_function}{expressionText}" : $"{expressionText}{_function}", errorText);
            }
            else
            {
                return (expressionText, CheckFormulaAndReturnError("", Operations.NotFunctionAfterAndOr));
            }
        }

        public static (string exprText, string errorText) CanAddLogicalOperator(string _operator, string expressionText, int selectionStart, int selectionLength, IEnumerable<string> keys)
        {
            string selection = expressionText.Substring(selectionStart, selectionLength).Trim();
            if (selectionLength > 0 && (selection == EditorHelper.And || selection == EditorHelper.Or))
            {
                return (ReplacePartOfString(_operator.Trim(), expressionText, selectionStart, selectionLength),
                    CheckFormulaAndReturnError(expressionText, Operations.NotFunctionAfterAndOr));
            }
            else if (selectionStart > 0 && selectionStart < expressionText.Length)
            {
                return TestExpression(expressionText.Insert(selectionStart, _operator), expressionText, Operations.LogicalOperatorInsert);
            }
            else if (CheckLogicalOperators(keys, expressionText))
            {
                string exprText = $"{expressionText}{_operator}";
                string errorText = CheckFormulaAndReturnError(exprText, Operations.AddLogicalOperator);
                return (exprText, errorText);
            }
            else
            {
                return (expressionText, CheckFormulaAndReturnError(expressionText, Operations.AddLogicalOperator));
            }
        }

        public static (string exprText, string errorText) TestExpression(string exprText, string origExpressionText, Enum errorMsg)
        {
            string errorText = CheckFormulaAndReturnError(exprText, errorMsg);
            return (!string.IsNullOrEmpty(errorText) ? origExpressionText : exprText, errorText);
        }

        public static string ReplacePartOfString(string replacingString, string expressionText, int selectionStart, int selectionLength)
        {
            int length = selectionLength > expressionText.Length ? 0 : expressionText.Length - (selectionStart + selectionLength);
            int newSelectionLength = selectionLength > expressionText.Length ? expressionText.Length : selectionLength;
            string returnExpressionText = expressionText;
            try
            {
                returnExpressionText = $"{expressionText.Substring(0, selectionStart)}{replacingString}" +
                $"{expressionText.Substring(selectionStart + newSelectionLength, length)}";
                return returnExpressionText;
            }
            catch (ArgumentOutOfRangeException)
            {
                return returnExpressionText;
            }
        }

        public static (string exprText, string errorText) AppendAtTheEnd(string operatorFunction, Enum errorMsg, string expressionText, IEnumerable<string> keys)
        {
            if (keys.Any(expressionText.EndsWith))
            {
                return !string.IsNullOrEmpty(expressionText) ? ($" {expressionText}{operatorFunction}", "") : ($"{expressionText}{operatorFunction}".Trim(), "");
            }
            else
            {
                return (expressionText, CheckFormulaAndReturnError(expressionText, errorMsg));
            }
        }

        public static (string exprText, string errorText) InsertOperatorFunctionWithinOrAtTheEndOfFormula(string expressionText, string exprText, int selectionStart, Enum errorMsgAfterInsert, Enum errorMsgInsert, Enum errorMsg)
        {
            if (selectionStart > 0 && selectionStart < expressionText.Length)
            {
                string exprTextLeft = expressionText.Substring(0, selectionStart);
                string exprTextRight = expressionText.Substring(selectionStart, expressionText.Length - selectionStart);
                if (CheckEndsOrStartsWithAndOr(exprTextLeft) || CheckEndsOrStartsWithAndOr(exprTextRight))
                {
                    return (expressionText.Insert(selectionStart, " (" + exprText + ") "), EditorHelper.errorMessages[errorMsgAfterInsert]);
                }
                else
                {
                    exprText = expressionText.Insert(selectionStart, " (" + exprText + ") ");
                    return TestExpression(exprText, expressionText, errorMsgInsert);
                }
            }
            else
            {
                exprText = !string.IsNullOrEmpty(expressionText) ? $"{expressionText} ({exprText})" : $"{expressionText}({exprText})";
                string errorText = CheckFormulaAndReturnError(exprText, errorMsg);
                return (exprText, errorText);
            }
        }

        public static (string exprText, string errorText) CanAddRelationalOperator(string _operator, string selectedField, string selectedPropertyValue, int selectionStart, int selectionLength, string expressionText, IEnumerable<string> keys)
        {
            string selection = expressionText.Substring(selectionStart, selectionLength).Trim();
            string exprText = $"{selectedField}{_operator}\"{selectedPropertyValue}\"";
            if (selectionLength > 0 && (selection == EditorHelper.Equal || selection == EditorHelper.NotEqual))
            {
                return (ReplacePartOfString(_operator.Trim(), expressionText, selectionStart, selectionLength), "");
            }
            else
            {
                return CheckFieldAndValuesNotEmpty(selectedField, selectedPropertyValue)
                    ? InsertOperatorFunctionWithinOrAtTheEndOfFormula(expressionText, exprText, selectionStart, Operations.FunctionOperatorAfterInserted,
                      Operations.RelationalOperatorInsert, Operations.AddRelationalOperator)
                    : AppendAtTheEnd(_operator, Operations.AddRelationalOperator, expressionText, keys);
            }
        }

        public static (string exprText, string errorText) CanAddStringFunction(string _function, string selectedField, string selectedPropertyValue, int selectionStart, int selectionLength, string expressionText, IEnumerable<string> keys)
        {
            string exprText = $"{selectedField}.{_function}";
            exprText = exprText + "(\"" + selectedPropertyValue + "\")";
            string selection = expressionText.Substring(selectionStart, selectionLength).Trim();
            if (selectionLength > 0 && (selection == EditorHelper.EndsWith || selection == EditorHelper.Contains || selection == EditorHelper.StartsWith))
            {
                return (ReplacePartOfString(_function, expressionText, selectionStart, selectionLength), "");
            }
            else
            {
                return CheckFieldAndValuesNotEmpty(selectedField, selectedPropertyValue)
                    ? InsertOperatorFunctionWithinOrAtTheEndOfFormula(expressionText, exprText, selectionStart, Operations.FunctionOperatorAfterInserted,
                      Operations.StringFunctionInsert, Operations.AddStringFunction)
                    : AppendAtTheEnd($".{_function}" + "(\"\")", Operations.AddStringFunction, expressionText, keys);
            }
        }

        public static (string exprText, string errorText) CanAddValue(string value, string expressionText, int selectionStart, int selectionLength)
        {
            string expressionTextTrimed = expressionText.Trim();
            if (selectionLength > 0)
            {
                string exprText = ReplacePartOfString(value, expressionTextTrimed, selectionStart, selectionLength);
                return TestExpression(exprText, expressionTextTrimed, Operations.ValueReplace);
            }
            else if (expressionTextTrimed.EndsWith("(\"\")"))
            {
                expressionTextTrimed = expressionTextTrimed.Substring(0, expressionTextTrimed.Length - 4) + "(\"" + value + "\")";
                return (expressionTextTrimed, "");
            }
            else if (expressionTextTrimed.EndsWith(EditorHelper.Equal) || expressionText.EndsWith(EditorHelper.NotEqual))
            {
                expressionTextTrimed += " \"" + value + "\"";
                return (expressionTextTrimed, "");
            }
            else
            {
                return (expressionTextTrimed, CheckFormulaAndReturnError(expressionTextTrimed, Operations.AddValue));
            }
        }

        public static (string exprText, string errorText) CanAddField(string item, string expressionText, int selectionStart, int selectionLength)
        {
            string expressionTextTrimed = expressionText.Trim();
            if (selectionLength > 0)
            {
                string exprText = ReplacePartOfString(item, expressionTextTrimed, selectionStart, selectionLength);
                return TestExpression(exprText, expressionTextTrimed, Operations.FieldReplace);
            }
            else if (expressionTextTrimed.EndsWith(EditorHelper.And) || expressionTextTrimed.EndsWith(EditorHelper.Or) || expressionTextTrimed.EndsWith(EditorHelper.Not) ||
                    string.IsNullOrEmpty(expressionTextTrimed))
            {
                expressionTextTrimed += string.IsNullOrEmpty(expressionTextTrimed) ? item : " " + item;
                return (expressionTextTrimed, "");
            }
            else
            {
                return (expressionTextTrimed, "");
            }
        }
    }
}
