using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Ts.EntityFramework.Core;
namespace Ts.Application
{
    public class BlogAppService:IDynamicApiController
    {
        private IRepository<Blog> _repository;
        public BlogAppService(IRepository<Blog> repository)
        {
            _repository = repository;
        }

        public IEnumerable<BlogModel> GetAll()
        {
            var dto = _repository.Entities.Include(x => x.Posts).AsQueryable();
            return dto.Adapt<List<BlogModel>>();;
        }

        public async Task TestU()
        {
            var dto = await _repository.Entities.FindAsync(1);
            dto.Name = DateTime.Now.ToLongDateString();
            //   dto = blogModel.Adapt<Blog>();
            //  _repository.Context.Entry(dto).State = EntityState.Modified;
            // await _repository.UpdateAsync(dto);


        }

    }
}