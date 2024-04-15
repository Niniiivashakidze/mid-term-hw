using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class CommunitiesRepository : ICommunitiesRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunitiesRepository(ApplicationDbContext applcationDBContext)
        {
            _context = applcationDBContext;
        }
        public async Task<PagedList<Community>> GetCommunities(int page, int pageSize, string? searchTerm, string? SortTerm, bool? isAscending = true)
        {
            var communities = _context.Communities.AsQueryable();

            if (isAscending == false)
            {
                communities = communities.OrderByDescending(GetSortExpression(searchTerm));
            }
            else
            {
                communities = communities.OrderBy(GetSortExpression(searchTerm));
            }


            if (!string.IsNullOrEmpty(searchTerm))
            {
                communities = communities.Where(community =>
                     community.Name.Contains(searchTerm) || community.Description.Contains(searchTerm));
            }

            return await PagedList<Community>.CreateAsync(communities, page, pageSize);
        }

        public Expression<Func<Community, object>> GetSortExpression(string? sortTerm)
        {
            sortTerm = sortTerm?.ToLower();
            return sortTerm switch
            {
                "CreatedAt" => community => community.CreateAt,
                "PostsCount" => community => community.Posts.Count(),
                "subscribersCount" => community => community.Subscribers.Count(),
                "id" => community =>community.Id,
                _ => community => community.Id,
            };
        }
    }
}
