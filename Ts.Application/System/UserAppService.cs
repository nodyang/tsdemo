using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Furion.DynamicApiController;
using Ts.EntityFramework.Core;
namespace Ts.Application
{
    public class UserAppService:IDynamicApiController
    {
        private IUserService _userService;
        public UserAppService(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<User> GetAll()
        {
          return  _userService.GetAll();
        }

        public int AddUser()
        {

            _userService.Add();
            return 1;
        }
    }
}