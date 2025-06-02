using System;
    using System.Collections.Generic;

    public class BlockStmt : Stmt
    {
        public List<Stmt> Statements {get; }


        public BlockStmt (List<Stmt> Statements)
        {
            this.Statements = Statements;

        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.visitBlockStmt(this);
        }
    }