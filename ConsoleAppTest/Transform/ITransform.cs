using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTest.Transform
{
    public interface ITransform<TFrom, VTo>
    {
        VTo Transform(TFrom from);
    }
}
