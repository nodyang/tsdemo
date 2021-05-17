using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.EntityFrameworkCore;
using Ts.Core;
using Ts.EntityFramework.Core;
namespace Ts.Application
{
    public class UserAppService:IDynamicApiController
    {
        private readonly IUserService _userService;

        private IRepository<Blog> _repository;

        private IRepository<User> _userRepository;
        public UserAppService(IUserService userService, IRepository<Blog> repository, IRepository<User> userRepository)
        {
            _userService = userService;
            _repository = repository;
            _userRepository = userRepository;
        }

        public string TestA(DtoModel dtoModel)
        {
            return dtoModel.Title;
        }
        
        public IEnumerable<User> GetAll()
        {
          return  _userService.GetAll();
        }

        public async Task Update()
        {
            var user = await _userRepository.Where(x => x.Id == 100).FirstOrDefaultAsync();
            
        }
        
        public class  UserDto
        {
         
        }

        public int AddUser()
        {

            _userService.Add();
            return 1;
        }

        public void TestAdd()
        {
            _repository.Insert(new Blog()
            {
                Name = "test",
                TenantId = Guid.NewGuid(),
                CreatedTime = DateTimeOffset.Now,
                UpdatedTime = DateTimeOffset.Now,
                IsDeleted = false,
                Posts = new List<Post>()
                {
                    new Post()
                    {
                        Title = "a",
                        CreatedTime = DateTimeOffset.Now,
                        Content = "yw",
                        IsDeleted = false
                    },
                    new Post()
                    {
                        Title = "ab",
                        CreatedTime = DateTimeOffset.Now,
                        Content = "y2w",
                        IsDeleted = false
                    }
                }
            });
        }

        public Blog GetBlog()
        {
           // throw new Exception("你好 异常");
            return _repository.Entities.Include(x=>x.Posts).FirstOrDefault(x => x.Id == 1);
        }
    }
}