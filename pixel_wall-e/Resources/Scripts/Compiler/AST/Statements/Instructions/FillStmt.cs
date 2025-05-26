using System;
    using System.Collections.Generic;

    public class FillStmt : Stmt
    {
    public readonly Token keyword;

    public FillStmt (Token keyword)
        {
        this.keyword = keyword;
    }

        public override void Accept<T>(IStatementVisitor<T> visitor)
        {
            visitor.visitFillStmt(this);
        }
    }