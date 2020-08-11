using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using MoviesService.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesService.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IQueryBus _queryBus;

        public UserController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet, Route("{userId}/movies")]
        public async Task<ActionResult<GetMoviesByUserIdQuery.Result>> GetMoviesByUserId([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetMoviesByUserIdQuery.Query
            {
                UserId = userId
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
