using System;
using System.Collections.Generic;
using Furion.DependencyInjection;

namespace Ts.Application
{
    public class SystemService : ISystemService, ITransient
    {
        public string GetDescription()
        {
            return "让 .NET 开发更简单，更通用，更流行。";

        }

        public class Stu
        {
            public string Name { get; set; }
        }
        public class Ts
        {
            public List<Stu> Stus { get; set; }
        }
    }
}