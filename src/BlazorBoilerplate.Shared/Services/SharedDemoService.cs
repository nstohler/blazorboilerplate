using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.LibLog.Logging;

namespace BlazorBoilerplate.Shared.Services
{
    internal class SharedDemoService : ISharedDemoService
    {
        private static readonly ILog Logger = LogProvider.For<SharedDemoService>();

        public DateTime GetDate()
        {
            var date = DateTime.Now;

            Logger.InfoFormat("current date is {date}", date);

            return date;
        }
    }
}
