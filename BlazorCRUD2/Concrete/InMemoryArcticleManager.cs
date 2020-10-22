using BlazorCRUD2.Contracts;
using BlazorCRUD2.DataAccess;
using BlazorCRUD2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorCRUD2.Linq;
using System.Runtime.CompilerServices;

namespace BlazorCRUD2.Concrete
{
    public class InMemoryArcticleManager : IArticleManager

    {

        AppDbContext _appDbContext;
        public void Activate(AppDbContext appDbContext)
        {

            _appDbContext = appDbContext;
        }

        public Task<int> Count(string search)
        {
            return Task.FromResult(_appDbContext.ArticleList.Count());
        }

        public Task<int> Create(Article article)
        {
            _appDbContext.ArticleList.Add(article);
            _appDbContext.SaveChanges();
            return Task.FromResult(article.ID);
        }

        public Task<int> Delete(int Id)
        {
            var A = _appDbContext.ArticleList.Where(X => X.ID == Id).FirstOrDefault();
            _appDbContext.ArticleList.Remove(A);
            _appDbContext.SaveChanges();
            return Task.FromResult(Id);
        }

        public Task<Article> GetById(int Id)
        {
            var A = _appDbContext.ArticleList.Where(X => X.ID == Id).FirstOrDefault();
            return Task.FromResult(A);
        }

        public Task<List<Article>> ListAll(int skip, int take, string orderBy, string direction, string search)
        {
            //return Task.FromResult(_appDbContext.ArticleList.ToList());
            return Task.FromResult(_appDbContext.ArticleList
                .Where(X => string.IsNullOrEmpty(search) ? true : X.Title.Contains(search))
                .Skip(skip)
                .Take(take)
                .AsEnumerable()
                .OrderByWithDirection( orderBy,direction)
                .ToList());
        }

        public Task<int> Update(Article article)
        {
            var A = _appDbContext.ArticleList.Where(X => X.ID == article.ID).FirstOrDefault();
            A.Title = article.Title;
            _appDbContext.SaveChanges();
            return Task.FromResult(A.ID);

        }
    }
}
