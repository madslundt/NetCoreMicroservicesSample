using Infrastructure.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using ReviewsService.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewsService.Controllers
{
    [Route("api/movies")]
    public class MovieController : Controller
    {
        private readonly IQueryBus _queryBus;

        public MovieController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet, Route("{movieId}/reviews")]
        public async Task<ActionResult<GetReviewsByMovieIdQuery.Result>> GetReviewsByMovieId([FromRoute] Guid movieId, CancellationToken cancellationToken)
        {
            var query = new GetReviewsByMovieIdQuery.Query
            {
                MovieId = movieId
            };

            var result = await _queryBus.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
