using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPanel
{
    internal class CompareParametersInterface : IComparer<Parameter>
    {
        public int Compare(Parameter x, Parameter y)
        {
            return String.Compare(x.Definition.Name, y.Definition.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
