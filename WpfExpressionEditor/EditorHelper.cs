using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor
{
    public static class EditorHelper
    {
        public const string Or = "Or";
        public const string And = "And";
        public const string Not = "Not";
        public const string Equal = "==";
        public const string NotEqual = "!=";
        public const string StartsWith = "StartsWith";
        public const string EndsWith = "EndsWith";
        public const string Contains = "Contains";
        public const string Brackets = "Brackets";

        public readonly static ImmutableDictionary<Enum, string> errorMessages = new Dictionary<Enum, string>()
        {
                { Operations.AddLogicalOperator, "After 'And' or 'Or' an expression should come e.g. Direction == 'X'!" },
                { Operations.LogicalOperatorReplace, "You can replace '==' with '!=' or the other way around!" },
                { Operations.LogicalOperatorInsert, "Trying to insert 'And' or 'Or' wrongly!Insertion has been revoked!" },
                { Operations.FunctionOperatorAfterInserted, "An expression inserted within an existing fomula should stand between 'And' or 'Or'!\n Do not forget to complete your formula!" },
                { Operations.AddRelationalOperator, "Expression with '==' and '!=' should be used like e.g. Direction == 'XYZ' and come after 'And' or 'Or'!" },
                { Operations.RelationalOperatorInsert, "Trying to insert '==' and '!=' wrongly! Insertion has been revoked!" },
                { Operations.InsertBrackets, "The selected text in the editor is not a valid expression! Please change your selection!" },
                { Operations.AddStringFunction, "'StartsWith', 'EndsWith' and 'Contains' can be used with fields (e.g. Direction.Contains('X'))\n and should come after 'And' or 'Or'!\n" },
                { Operations.StringFunctionInsert, "Trying to insert 'StartsWith', 'EndsWith' or 'Contains' wrongly! Insertion has been revoked!" },
                { Operations.FieldReplace, "Check if you correctly selected the field to be replaced!" },
                { Operations.ValueReplace, "Check if you correctly selected the value to be replaced!" },
                { Operations.NotFunctionInsert, "You should insert 'Not' before a field with (StartsWith, EndsWith or Contains)!\n Your selection should look like e.g. Direction.Contains(\"XYZ\")!"},
                { Operations.NotFunctionAfterAndOr, "'Not' can stand after 'And' or 'Or'!"},
                { Operations.AddValue, "Trying to add 'Value' wrongly! Insertion has been revoked!" }
        }.ToImmutableDictionary();

        private readonly static ImmutableDictionary<string, string> ReplaceString = new Dictionary<string, string>()
        {
            { "OrElse", Or},
            { "AndAlso", And},
            { "GreaterThanOrEqual", ""},
            { "GreaterThan", ""},
            { "Param_0.", ""},
            { "Call", ""},
            { "Lambda : Param_0 =>", ""},
            { "Lambda", ""}
        }.ToImmutableDictionary();

        public static string Replace(string input)
        {
            string str = input;
            if (!string.IsNullOrEmpty(str))
            {
                ReplaceString.ToList().ForEach(pair => str = str.Replace(pair.Key, pair.Value));
            }

            return str;
        }
    }
}
