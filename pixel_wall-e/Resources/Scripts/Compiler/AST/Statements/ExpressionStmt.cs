using System;
    using System.Collections.Generic;

    public class ExpressionStmt : Stmt
    {
        public Expression Expr {get; }


        public ExpressionStmt (Expression Expr)
        {
            this.Expr = Expr;

        }

        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.visitExpressionStmt(this);
        }
    }