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
    public class ExpressionEditorHelper<T>
    {        
        public (Expression<Func<T, bool>> Exp, bool ChekOk, string Msg) ParseStringToExpression(string filter)
        {
            Expression<Func<T, bool>> exp = null;
            Func<T, bool> func;
            try
            {
                filter = "c => " + filter;
                exp = DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, filter, new object[0]);
                func = exp.Compile();
            }
            catch (Exception ex)
            {               
                return (exp, false, ex.Message);
            }

            return (exp, true, "");
        }
        public Dictionary<string, string> GetPropertiesAndValuesOfGenericType(T Item)
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
                if (propVal != null && propVal.ToString() != "")
                {
                    propCollection.Add(prop.Name, propVal.ToString());
                }
            }

            return propCollection;
        }

        public static bool CheckParenthesis(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return false;
            }

            var stack = new Stack<char>();

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    stack.Push(expression[i]);
                }
                else if (expression[i] == ')')
                {
                    _ = stack.Pop();
                }
            }

            return stack.Count == 0;
        }
    }
}
