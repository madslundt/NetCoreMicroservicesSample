using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using ReviewsService.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IQueryBus _queryBus;

        public UserController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet, Route("{userId}/reviews")]
        public async Task<ActionResult<GetReviewsByUserIdQuery.Result>> GetReviewsByMovieId([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var query = new GetReviewsByUserIdQuery.Query
            {
                UserId = userId
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
