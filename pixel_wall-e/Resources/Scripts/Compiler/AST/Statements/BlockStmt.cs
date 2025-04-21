using System;
    using System.Collections.Generic;

    public class BlockStmt : Stmt
    {
        public List<Stmt> Statements {get; }


        public BlockStmt (List<Stmt> Statements)
        {
            this.Statements = Statements;

        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.visitBlockStmt(this);
        }
    }