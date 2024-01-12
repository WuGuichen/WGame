public partial class MethodDefine
{
    private void BindMath()
    {
        // 点乘
        SetMethod("Math_Dot", (list, interpreter) =>
        {
            var l = list[0];
            var r = list[1];
            if (r.Type == l.Type)
            {
                if (r.Type == TYPE_TABLE)
                {
                    var def = interpreter.Definition;
                    var lR = def.GetTable(r.Value);
                    var lL = def.GetTable(l.Value);
                    if (lR.Count == lL.Count)
                    {
                        float res = 0;
                        for (int i = 0; i < lR.Count; i++)
                        {
                            res += MultiNumber(lR[i], lL[i], def);
                        }

                        interpreter.SetRetrun(res);
                        return;
                    }
                    else
                    {
                        throw WLogger.ThrowError("请确保运算数据一致");
                    }
                }
            }

            throw WLogger.ThrowError("点乘运算错误");
        });
    }
}
