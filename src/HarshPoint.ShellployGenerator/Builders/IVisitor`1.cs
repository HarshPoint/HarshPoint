using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator.Builders
{
    public interface IVisitor<T>
    {
        T Visit(T value);
    }
}
