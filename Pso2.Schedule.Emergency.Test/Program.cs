using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pso2.Schedule.Emergency;
namespace Pso2.Schedule.Emergency.Test {
    class Program {
        static void Main(string[] args) {
            ParsePso2WebData pso2 = new ParsePso2WebData();
            var data =  pso2.ImportFromWebSite();
        }
    }
}
