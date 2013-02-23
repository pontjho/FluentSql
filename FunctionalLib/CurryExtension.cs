using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalLib
{
    public static class CurryExtension
    {
        public static Action<Input1> Curry<Input1, Input2>(Action<Input1, Input2> inputFunction, Input2 parameter)
        {
            return (x) => inputFunction(x, parameter);
        }

        public static Func<RType> Curry<IType, RType>(Func<IType, RType> inputFunction, IType parameter)
        {
            return () => inputFunction(parameter);
        }

        public static Func<IType2, RType> Curry<IType1, IType2, RType>(Func<IType1, IType2, RType> inputFunction, IType1 parameter)
        {
            return (x) => inputFunction(parameter, x);
        }

        public static Func<IType1, RType> Curry2<IType1, IType2, RType>(Func<IType1, IType2, RType> inputFunction, IType2 parameter)
        {
            return (x) => inputFunction(x, parameter);
        }
    }
}
