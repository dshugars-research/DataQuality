using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualitySemWebLib
{
    public interface IViolationMessageStore
    {
        void AddViolationMessage(string msg);
    }
}
