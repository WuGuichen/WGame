public partial class WLangParser
{
    public partial class ParametersDefContext
    {
        public string[] GetParams()
        {
            var res = new string[children.Count / 2 + 1];
            int c = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                var idCtx = children[i] as PrimaryIDContext;
                if (idCtx != null)
                {
                    res[c++] = idCtx.i.Text;
                }
            }

            return res;
        }
    }

    public partial class ExprMethodContext
    {
        public override T GetChild<T>(int i)
        {
            if (children == null || i < 0 || i >= children.Count)
            {
                return default(T);
            }

            int j = -1;
            // what element have we found with ctxType?
            for (int ii =0; ii < ChildCount; ii++)
            {
                var o = children[ii];
                if (o is T)
                {
                    j++;
                    if (j == i)
                    {
                        return (T) o;
                    }
                }
            }

            return default(T);
        }
    }
    
    public partial class PrimaryIDContext
    {
        public override string GetText()
        {
            return i.Text;
        }
    }

    public partial class PrimarySTRINGContext
    {
        private string trimText;

        public string TrimText
        {
            get
            {
                if (trimText == null)
                {
                    trimText = i.Text.Trim('"');
                }

                return trimText;
            }
        }
    }
}
