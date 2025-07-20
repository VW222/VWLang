using System.Diagnostics;

namespace VWLang
{
    public class Parser
    {
        private TokenList Input;

        public AST Parse(TokenList lexOut)
        {
            Input = lexOut;
            Clean();

            AST ast = new AST();
            ast.Root = (ASTBlock)EvaluateExpression(0);
            ast.Print();
            //Optimize(ast.Root);
            //ast.Print();
            return ast;
        }

        private void Optimize(ASTNode root)
        {
            if (root == null || root.Children == null)
            {
                return;
            }
            for (int i = 0; i < root.Children.Count; i++)
            {
                ASTNode child = root.Children[i];
                Optimize(child);

                if (child is ASTBlock block && block.Children.Count == 1)
                {
                    root.Children[i] = block.Children[0];
                }
                if (child is ASTBranchExpression branch)
                {
                    Optimize(branch.Condition);
                    Optimize(branch.TrueBranch);
                    if (branch.ElseBranch != null)
                        Optimize(branch.ElseBranch);
                }
            }
        }

        // end index IS the semicolon
        private int FindNext(int start, TokenType type)
        {
            for (int i = start; i < Input.TList.Count; ++i)
            {
                if (Input.TList[i].name == type)
                {
                    return i;
                }
            }
            return -1; // make the compiler shut up
        }
        //[Conditional("DEBUG")]
        private void PrintRange(int start, int end)
        {
            for (int i = start; i < end; ++i)
            {
                Console.WriteLine(Input.TList[i].ToString());
            }
            Console.WriteLine();
        }

        // assume start IS the opening parenthesis
        private int FindNextClosingParen(int start)
        {
            int count = 0;
            for (int i = start; ;i++)
            {
                if (i >= Input.TList.Count)
                    Logger.Error("Mismatched parentheses in if statement");

                if (Input.TList[i].name == TokenType.LeftParen)
                    count++;
                else if (Input.TList[i].name == TokenType.RightParen)
                    count--;

                if (count == 0)
                    return i;
            }
        }

        private ASTExpression EvaluateExpression(int startIndex)
        {
            //Input.Print();
            //Console.WriteLine();
            if (Input.TList[startIndex].name == TokenType.LeftCurlyBrace)
            {
                // Block, else not
                Input.TList.RemoveAt(startIndex); // remove first {
                ASTBlock result = new ASTBlock();
                do
                {
                    result.AddChild(EvaluateExpression(startIndex));
                    if (Input.TList[startIndex].name == TokenType.RightCurlyBrace) break;
                } while (Input.TList.Count > startIndex);
                // assume the only thing left is a }
                Input.TList.RemoveAt(startIndex);
                return result;
            }
            else
            {
                if (Input.TList.Count > 3)
                {
                    if (Input.TList[startIndex + 1].name == TokenType.Assign)
                    {
                        ASTAssignation result = new()
                        {
                            Name = Input.TList[startIndex].value
                        };
                        Input.RepRemove(startIndex, 2);
                        result.Expression = EvaluateExpression(startIndex + 1);
                        return result;
                    }
                    else if (Input.TList[startIndex + 2]?.name == TokenType.Declaration)
                    {
                        ASTDeclaration result = new()
                        {
                            Name = Input.TList[startIndex + 1].value,
                            Type = Input.TList[startIndex].value,
                        };
                        Input.RepRemove(startIndex, 3);
                        result.Expression = EvaluateExpression(startIndex);
                        return result;
                    }
                    // if (statement) expr;
                    else if (Input.TList[startIndex].value == "if")
                    {
                        int closing = FindNextClosingParen(startIndex + 1);
                        Input.TList.Insert(closing, new Token(TokenType.Semicolon)); // insert ; so it stops before the closing )

                        var result = new ASTBranchExpression();
                        Input.RepRemove(startIndex, 2); // if, (
                        result.Condition = EvaluateExpression(startIndex);
                        Input.RepRemove(startIndex, 1); // )
                        result.TrueBranch = EvaluateExpression(startIndex);

                        if (Input.TList[startIndex]?.value == "else")
                        {
                            Input.RepRemove(startIndex, 1); // remove the "else"
                            result.ElseBranch = EvaluateExpression(startIndex);
                        }
                        return result;
                    }
                }

                int endIndex = FindNext(startIndex, TokenType.Semicolon);
                Input.TList.RemoveAt(endIndex);
                RPNQueue output = ShuntingYard(startIndex, endIndex);
                Input.RepRemove(startIndex, endIndex - startIndex);
                //output.Print();
                Stack<ASTExpression> nodes = new Stack<ASTExpression>();

                while (output.Count > 0)
                {
                    Token t = output.Dequeue();

                    if (t.name == TokenType.Int || t.name == TokenType.Float || t.name == TokenType.Name || t.name == TokenType.String)
                    {
                        if (t.name == TokenType.Int) nodes.Push(new ASTIntValue(int.Parse(t.value)));
                        else if (t.name == TokenType.Float) nodes.Push(new ASTFloatValue(float.Parse(t.value)));
                        else if (t.name == TokenType.String) nodes.Push(new ASTStringValue(t.value));
                        else if (t.name == TokenType.Name) nodes.Push(new ASTVariableValue(t.value));
                    }
                    else if (Token.IsOp(t.name))
                    {
                        var right = nodes.Pop();
                        var left = nodes.Pop();
                        var op = new ASTMathOp(t);
                        op.AddChild(left);
                        op.AddChild(right);
                        nodes.Push(op);
                    }
                    else if (t.name == TokenType.Function)
                    {
                        var functionNode = new ASTFunction
                        {
                            name = t.value,
                        };

                        while (nodes.Count > 0)
                        {
                            functionNode.AddChild(nodes.Pop());
                        }

                        functionNode.ReverseChildren();
                        nodes.Push(functionNode);
                    }
                }

                if (nodes.Count == 0) return new ASTNop();
                return nodes.Pop();
            }
        }

        private RPNQueue ShuntingYard(int startIndex, int endIndex)
        {
            RPNQueue output = new RPNQueue();
            Stack<Token> operators = new Stack<Token>();

            for (int i = startIndex; i < endIndex; i++)
            {
                var token = Input.TList[i];

                if (token.name == TokenType.Int || token.name == TokenType.Float || token.name == TokenType.Name || token.name == TokenType.String)
                {
                    output.Enqueue(token);
                }
                // else if function
                else if (token.name == TokenType.Function)
                {
                    operators.Push(token);
                }
                else if (Token.IsOp(token.name))
                {
                    while (operators.Count > 0 && operators.Peek().name != TokenType.LeftParen &&
                           ((Token.GetPrecedence(token.name) < Token.GetPrecedence(operators.Peek().name)) ||
                           (Token.GetPrecedence(token.name) == Token.GetPrecedence(operators.Peek().name) && Token.IsLeftAssociative(token.name))))
                    {
                        output.Enqueue(operators.Pop());
                    }
                    operators.Push(token);
                }
                else if (token.name == TokenType.Comma)
                {
                    while (operators.Peek().name != TokenType.LeftParen)
                    {
                        output.Enqueue(operators.Pop());
                    }
                }
                else if (token.name == TokenType.LeftParen)
                {
                    operators.Push(token);
                }
                else if (token.name == TokenType.RightParen)
                {
                    while (operators.Peek().name != TokenType.LeftParen)
                    {
                        // {assert the operator stack is not empty, or else mismatched pars}
                        output.Enqueue(operators.Pop());
                    }
                    operators.Pop();
                }
            }
            while (operators.Count != 0)
            {
                output.Enqueue(operators.Pop());
            }

            return output;
        }

        // strip comments, remove newlines, find functions, strip "" from strings, insert semicolons after if/else (workaround lmao)
        private void Clean()
        {
            Input.TList.Insert(0, new Token(TokenType.LeftCurlyBrace));
            int size = Input.TList.Count;
            for (int i = 0; i < size; i++)
            {
                Token token = Input.TList[i];

                if (token.name == TokenType.Comment)
                {
                    int c = 0;

                    do c++;
                    while (Input.TList[i + c].name != TokenType.NewLine);

                    Input.RepRemove(i, c);
                    size -= c;
                }

                if (token.name == TokenType.Name && Input.TList[i + 1].name == TokenType.LeftParen)
                {
                    token.name = TokenType.Function;
                }
                if (token.name == TokenType.String)
                {
                    token.value = token.value.Substring(1, token.value.Length - 2);
                }
                //if (token.name == TokenType.Keyword)
                //{
                //    if (token.value == "if")
                //    {
                //        Input.TList.Insert(FindNextChar(i, ')') + 1, new Token(TokenType.Semicolon, ";"));
                //    }
                //    else
                //    {
                //        Input.TList.Insert(i + 1, new Token(TokenType.Semicolon, ";"));
                //    }
                //}
            }
            Input.TList.RemoveAll(v => v.name == TokenType.NewLine);
            Input.TList.Add(new Token(TokenType.RightCurlyBrace));
        }
    }
}