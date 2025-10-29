

using Application.DTO;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace _4Module.Controllers
{

    /// <summary>
    /// Base Review Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]

    public class ReviewController : ControllerBase
    {

        private readonly IProductReviewService _productReview;

        public ReviewController(IProductReviewService productReview) { _productReview = productReview; }



        [HttpGet("review/{id:length(24)}")]
        public async Task<ActionResult<ProductReviewResponseDTO>> GetById([FromRoute]  string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) {
                return BadRequest("Invalid Id");
            }
            var review = await _productReview.GetByIdAsync(id);
            if (review == null) { return    NotFound(); }
            
            return Ok(review);
        }

        [HttpPost("create-review/")]
        public async Task<ActionResult<ProductReviewResponseDTO>> CreateReview([FromBody] CreateProductReviewDTO productReview)
        {
            await _productReview.CreateAsync(productReview);
            return Ok(productReview);
        }
        [HttpGet("products/{id:guid}")]
        public async Task<ActionResult<IEnumerable<ProductReviewResponseDTO>>> GetAllReviewsById([FromRoute] Guid id)
        {
            var reviews = await _productReview.GetByProductAsync(id);
            return Ok(reviews);

        }
    }
}
