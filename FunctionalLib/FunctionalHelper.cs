using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentLib.FunctionalLib
{
    public static class FunctionHelper
    {

        public static Func<IType1, RType> Compose<IType1, IType2, RType>(Func<IType1, IType2> func1, Func<IType2, RType> func2)
        {
            return (x) => func2(func1(x));
        }

        public static Func<IType1, RType> Compose<IType1, IType2, IType3, RType>(Func<IType1, IType2> func1, Func<IType2, IType3> func2, Func<IType3, RType> func3)
        {
            return (x) => func3(func2(func1(x)));
        }
    }
}
