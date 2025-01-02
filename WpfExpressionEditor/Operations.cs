using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExpressionEditor
{
    public enum Operations
    {
        AddLogicalOperator,
        LogicalOperatorReplace,
        LogicalOperatorInsert,
        AddRelationalOperator,
        RelationalOperatorInsert,
        FunctionOperatorAfterInserted,
        InsertBrackets,
        AddStringFunction,
        StringFunctionInsert,
        FieldReplace,
        ValueReplace,
        NotFunctionInsert,
        NotFunctionAfterAndOr,
        AddValue
    }
}
