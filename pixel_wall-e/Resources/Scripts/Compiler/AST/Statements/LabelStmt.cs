using System;
    using System.Collections.Generic;

    public class LabelStmt : Stmt
    {
        public Token Name {get; }


        public LabelStmt (Token Name)
        {
            this.Name = Name;

        }

        public override void Accept(IStatementVisitor visitor)
        {
            visitor.visitLabelStmt(this);
        }
    }