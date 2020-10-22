using BlazorCRUD2.Contracts;
using BlazorCRUD2.DataAccess;
using BlazorCRUD2.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorCRUD2.Pages
{
    public class FetchArticleBase : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; }
        [Inject] IArticleManager articleManager { get; set; }
        [Inject] AppDbContext appDbContext { get; set; }


        private string searchTerm;
        public string SearchTerm
        {
            get { return searchTerm; }
            set { searchTerm = value; FilterRecords(); }
        }

        public List<Article> articleModel;
        Article articleEntity = new Article();


        #region Pagination

        public int totalPages;
        public int totalRecords;
        public int curPage;
        public int pagerSize;
        public int pageSize;
        public int startPage;
        public int endPage;
        public string sortColumnName = "ID";
        public string sortDir = "DESC";

        #endregion

        protected override async Task OnInitializedAsync()
        {

            if (appDbContext.ArticleList is null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseInMemoryDatabase("BlazorCRUD2");
                appDbContext = new AppDbContext(optionsBuilder.Options);
            }
            articleManager.Activate(appDbContext);

            //display total page buttons
            pagerSize = 3;
            pageSize = 5;
            curPage = 1;
            articleModel = await articleManager.ListAll((curPage - 1) * pageSize, pageSize, sortColumnName, sortDir, searchTerm);
            totalRecords = await articleManager.Count(searchTerm);
            totalPages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            SetPagerSize("forward");
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return JS.InvokeVoidAsync("Log", "FetchArticle InitializedAsync").AsTask();
        }


        protected async Task DeleteArticle(int id)
        {
            await articleManager.Delete(id);
            articleModel = await articleManager.ListAll((curPage - 1) * pageSize, pageSize, sortColumnName, sortDir, searchTerm);
        }

        private bool isSortedAscending;
        private string activeSortColumn;

        private async Task<List<Article>> SortRecords(string columnName, string dir)
        {
            return await articleManager.ListAll((curPage - 1) * pageSize, pageSize, columnName, dir, searchTerm);
        }

        public async Task SortTable(string columnName)
        {
            if (columnName != activeSortColumn)
            {
                articleModel = await SortRecords(columnName, "ASC");
                isSortedAscending = true;
                activeSortColumn = columnName;
            }
            else
            {
                if (isSortedAscending)
                {
                    articleModel = await SortRecords(columnName, "DESC");
                }
                else
                {
                    articleModel = await SortRecords(columnName, "ASC");
                }

                isSortedAscending = !isSortedAscending;
            }
            sortColumnName = columnName;
            sortDir = isSortedAscending ? "ASC" : "DESC";
        }

        public string SetSortIcon(string columnName)
        {
            if (activeSortColumn != columnName)
            {
                return string.Empty;
            }
            if (isSortedAscending)
            {
                return "fa-sort-up";
            }
            else
            {
                return "fa-sort-down";
            }
        }

        public async Task refreshRecords(int currentPage)
        {
            articleModel = await articleManager.ListAll((currentPage - 1) * pageSize, pageSize, sortColumnName, sortDir, searchTerm);
            curPage = currentPage;
            this.StateHasChanged();
        }

        public void SetPagerSize(string direction)
        {
            if (direction == "forward" && endPage < totalPages)
            {
                startPage = endPage + 1;
                if (endPage + pagerSize < totalPages)
                {
                    endPage = startPage + pagerSize - 1;
                }
                else
                {
                    endPage = totalPages;
                }
                this.StateHasChanged();
            }
            else if (direction == "back" && startPage > 1)
            {
                endPage = startPage - 1;
                startPage = startPage - pagerSize;
            }
            else
            {
                startPage = 1;
                endPage = totalPages;
            }
        }

        public async Task NavigateToPage(string direction)
        {
            if (direction == "next")
            {
                if (curPage < totalPages)
                {
                    if (curPage == endPage)
                    {
                        SetPagerSize("forward");
                    }
                    curPage += 1;
                }
            }
            else if (direction == "previous")
            {
                if (curPage > 1)
                {
                    if (curPage == startPage)
                    {
                        SetPagerSize("back");
                    }
                    curPage -= 1;
                }
            }
            await refreshRecords(curPage);
        }

        public void FilterRecords()
        {
            endPage = 0;
            this.OnInitializedAsync().Wait();
        }


    }
}
