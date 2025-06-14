using System;
    using System.Collections.Generic;

    public class SizeStmt : Stmt
    {
    public readonly Token keyword;

    public Expression K {get; }


        public SizeStmt (Expression K, Token keyword)
        {
            this.K = K;
        this.keyword = keyword;
    }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.visitSizeStmt(this);
        }
    }