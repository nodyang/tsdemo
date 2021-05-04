using System;
using System.Collections.Generic;
using Furion.DatabaseAccessor;
using Furion.DependencyInjection;
using Ts.EntityFramework.Core;
namespace Ts.Application
{
    public class UserService:IUserService,ITransient
    {

        private IRepository<User> _repository;
        public UserService(IRepository<User> repository)
        {
            _repository = repository;
        }
        public IEnumerable<User> GetAll()
        {
            return _repository.Entities.AsQueryable();
        }
        public void Add()
        {
            for (int i = 0; i < 100; i++)
            {
                _repository.Insert(new User()
                {
                    Age = i,
                    CreatedTime = DateTimeOffset.Now,
                    IsDeleted = false,
                    Name = i.ToString(),
                    TenantId = Guid.NewGuid(),
                    UpdatedTime = DateTimeOffset.Now
                });
            }
        }
    }
}