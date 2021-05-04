using System.Collections;
using System.Collections.Generic;
using Ts.EntityFramework.Core;
namespace Ts.Application
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();

        void Add();
    }
}