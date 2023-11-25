using System.Collections.Generic;

namespace WGame.Trigger
{
    public class WtCondition
    {
        public virtual bool Evaluate()
        {
            return false;
        }

        public static WtCondition Always = new WtConditionAlways();
        public static WtCondition Never = new WtConditionNever();
    }

    // public static class WtConditionExtension
    // {
    //     public static WtCondition And(this WtCondition left, WtCondition right)
    //     {
    //         return new WtConditionAnd(left, right);
    //     }
    //     
    //     public static WtCondition Or(this WtCondition left, WtCondition right)
    //     {
    //         return new WtConditionOr(left, right);
    //     }
    //     
    //     public static WtCondition Not(this WtCondition left)
    //     {
    //         return new WtConditionNot(left);
    //     }
    // }
    
    public class WtConditionAlways : WtCondition
    {
        public override bool Evaluate()
        {
            return true;
        }
    }
    
    public class WtConditionNever : WtCondition
    {
        public override bool Evaluate()
        {
            return false;
        }
    }

    public class WtConditionAnd : WtCondition
    {
        private readonly WtCondition _left;
        private readonly WtCondition _right;

        public WtConditionAnd(WtCondition left, WtCondition right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate()
        {
            return _left.Evaluate() && _right.Evaluate();
        }
    }

    public class WtConditionOr : WtCondition
    {
        private readonly WtCondition _left;
        private readonly WtCondition _right;

        public WtConditionOr(WtCondition left, WtCondition right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate()
        {
            return _left.Evaluate() || _right.Evaluate();
        }
    }

    public class WtConditionNot : WtCondition
    {
        private readonly WtCondition _condition;

        public WtConditionNot(WtCondition condition)
        {
            _condition = condition;
        }

        public override bool Evaluate()
        {
            return !_condition.Evaluate();
        }
    }
}