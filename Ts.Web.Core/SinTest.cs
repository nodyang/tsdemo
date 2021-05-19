using System;
using Furion.DependencyInjection;
namespace Ts.Web.Core
{
    public class SinTest:ISingleton
    {
        public SinTest()
        {
            Name = DateTime.Now.ToString();
        }
        public string Name { get; set; }
        
    }
}