using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TGNS.Core.Data;
using TGNS.Portal.Classes;

namespace TGNS.DevRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverAdminJsonUpdater = new ServerAdminJsonUpdater();
            serverAdminJsonUpdater.Update();
        }
    }
}
