using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2.Schedule.Emergency {
    interface IError {
        Exception GetExeception();
        string GetErrorMessage();
        int GetErrorNumber();
    }
}
