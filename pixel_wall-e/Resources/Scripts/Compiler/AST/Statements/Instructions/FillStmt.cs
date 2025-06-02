using System;
    using System.Collections.Generic;

    public class FillStmt : Stmt
    {
    public readonly Token keyword;

    public FillStmt (Token keyword)
        {
        this.keyword = keyword;
    }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.visitFillStmt(this);
        }
    }