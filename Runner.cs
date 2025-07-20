namespace VWLang
{
    public class Runner
    {
        public LangEnvironment env = new LangEnvironment();
        public void Run(AST input)
        {
            //input.Print();
            Interpret(input);
        }
        private void Interpret(AST input)
        {
            foreach (var expr in input.Root.Children)
            {
                EvaluateExpression(expr);
            }
        }
        private ASTValue? EvaluateExpression(ASTNode expression)
        {
            // assume only ASTExpression
            if (expression is ASTNop)
            {
                return null;
            }
            else if (expression is ASTFunction function)
            {
                if (function.name == "println" || function.name == "print")
                {
                    var typeHandlers = new Dictionary<ASTValue.Type, Action<object>>
                    {
                        { ASTValue.Type.Int, v => Console.Write((int)v + " ") },
                        { ASTValue.Type.Float, v => Console.Write((float)v + " ") },
                        { ASTValue.Type.String, v => Console.Write((string)v + " ") },
                    };
                    foreach (ASTExpression child in function.Children)
                    {
                        ASTValue value = EvaluateExpression(child);
                        if (value is ASTVariableValue) value = EvaluateExpression(value);
                        typeHandlers[value.type](value.value);
                    }
                    if (function.name == "println")
                        Console.WriteLine();
                }
            }
            else if (expression is ASTMathOp mathOp)
            {
                ASTValue left = EvaluateExpression(mathOp.Children[0]);
                ASTValue right = EvaluateExpression(mathOp.Children[1]);
                Dictionary<ASTMathOp.Operation, Func<ASTValue, ASTValue, ASTValue>> ops = new()
                {
                    { ASTMathOp.Operation.Subtract, Subtract },
                    { ASTMathOp.Operation.Add, Add },
                    { ASTMathOp.Operation.Multiply, Multiply },
                    { ASTMathOp.Operation.Divide, Divide },
                    { ASTMathOp.Operation.Equals, EqualsOp },
                    { ASTMathOp.Operation.GreaterThan, GreaterThan },
                    { ASTMathOp.Operation.LessThan, LessThan }
                };
                return ops[mathOp.operation](left, right);
            }
            else if (expression is ASTBranchExpression ex)
            {
                // if - (statement, exprIfTrue, exprIfFalse)
                if ((int)((ASTIntValue)EvaluateExpression(ex.Condition)).value != 0)
                {
                    EvaluateExpression(ex.TrueBranch);
                }
                else
                {
                    EvaluateExpression(ex?.ElseBranch);
                }
            }
            else if (expression is ASTDeclaration decl)
            {
                env.Declare(decl.Name, EvaluateExpression(decl.Expression));
            }
            else if (expression is ASTAssignation assign)
            {
                env.Assign(assign.Name, EvaluateExpression(assign.Expression));
            }
            else if (expression is ASTVariableValue varEx)
            {
                return env.Get((string)(varEx).value);
            }
            else if (expression is ASTBlock)
            {
                foreach (var expr in expression.Children)
                {
                    EvaluateExpression(expr);
                }
            }
            else if (expression is ASTValue value)
            {
                return value;
            }

            return null;
        }

        private ASTValue Add(ASTValue left, ASTValue right)
        {
            if (left.type == ASTValue.Type.Int && right.type == ASTValue.Type.Int)
            {
                return new ASTIntValue((int)left.value + (int)right.value);
            }
            else if (left.type == ASTValue.Type.Float && right.type == ASTValue.Type.Float)
            {
                return new ASTFloatValue((float)left.value + (float)right.value);
            }
            else if (left.type == ASTValue.Type.String && right.type == ASTValue.Type.String)
            {
                return new ASTStringValue((string)left.value + (string)right.value);
            }
            else
            {
                throw new InvalidOperationException("Operands must be of the same type.");
            }
        }
        private ASTValue Multiply(ASTValue left, ASTValue right)
        {
            if (left.type == ASTValue.Type.Int && right.type == ASTValue.Type.Int)
            {
                return new ASTIntValue((int)left.value * (int)right.value);
            }
            else if (left.type == ASTValue.Type.Float && right.type == ASTValue.Type.Float)
            {
                return new ASTFloatValue((float)left.value * (float)right.value);
            }
            else
            {
                throw new InvalidOperationException("Operands must be of the same type.");
            }
        }
        private ASTValue Divide(ASTValue left, ASTValue right)
        {
            if (left.type == ASTValue.Type.Int && right.type == ASTValue.Type.Int)
            {
                return new ASTIntValue((int)left.value / (int)right.value);
            }
            else if (left.type == ASTValue.Type.Float && right.type == ASTValue.Type.Float)
            {
                return new ASTFloatValue((float)left.value / (float)right.value);
            }
            else
            {
                throw new InvalidOperationException("Operands must be of the same type.");
            }
        }
        private ASTValue Subtract(ASTValue left, ASTValue right)
        {
            if (left.type == ASTValue.Type.Int && right.type == ASTValue.Type.Int)
            {
                return new ASTIntValue((int)left.value - (int)right.value);
            }
            else if (left.type == ASTValue.Type.Float && right.type == ASTValue.Type.Float)
            {
                return new ASTFloatValue((float)left.value - (float)right.value);
            }
            else
            {
                throw new InvalidOperationException("Operands must be of the same type.");
            }
        }
        private ASTValue EqualsOp(ASTValue left, ASTValue right)
        {
            if (left.type == ASTValue.Type.Int && right.type == ASTValue.Type.Int)
            {
                return new ASTIntValue((int)left.value == (int)right.value);
            }
            else
            {
                throw new InvalidOperationException("Operands must be of the same type.");
            }
        }
        private ASTValue GreaterThan(ASTValue left, ASTValue right)
        {
            // safely assume ints behave nicely
            return new ASTIntValue(Convert.ToSingle(left.value) > Convert.ToSingle(right.value));
        }
        private ASTValue LessThan(ASTValue left, ASTValue right)
        {
            // safely assume ints behave nicely
            return new ASTIntValue(Convert.ToSingle(left.value) < Convert.ToSingle(right.value));
        }
    }
}