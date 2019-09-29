using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class FuncExtensions
    
    {
        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f)
        {
            

            Func<A, R> g = null;
            g = f(a => g(a));
            return g;
        }
    }
}
