using Microsoft.AspNetCore.Mvc;
using RSFBackup.Core.DTO.Bucket;
using RSFBackup.Core.Interfaces.Repositories;

namespace RSFBackup.Api.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }


        /// <summary>
        /// Obter a lista de S3 buckets
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ListS3BucketsResponse>>> GetBucketsAsync()
        {
            var response = await _bucketRepository.GetAllBucketsAsync();
            if (response == null || response.Count() == 0)
                return NotFound(new { message = "S3 Buckets not found" });

            return Ok(response);
        }


        /// <summary>
        /// Criar um S3 bucket, de acordo com as regras de nomenclatura de buckets S3
        /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        /// </summary>
        [HttpPost()]
        public async Task<ActionResult<CreateS3BucketResponse>> CreateS3BucketAsync([FromBody] CreateS3BucketRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.BucketName)) 
                return BadRequest(new { message = "S3 Bucket name is required" });

            var bucketExists = await _bucketRepository.IsBucketS3ExistAsync(request.BucketName);
            if (bucketExists)
                return BadRequest(new { message = $"S3 Bucket with name {request.BucketName} already exists" });

            var result = await _bucketRepository.CreateBucketAsync(request.BucketName);
            if (result == null)
                return BadRequest(new { message = "Unable to create S3 bucket" });

            return Ok(result);
        }


        /// <summary>
        /// Remover um S3 bucket pelo nome
        /// </summary>
        /// <param name="bucketName">o nome do S3 bucket</param>
        [HttpDelete("{bucketName}")]
        public async Task<ActionResult<bool>> DeleteBucketAsync(string bucketName)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                return BadRequest(new { message = "S3 Bucket name is required" });

            var response = await _bucketRepository.DeleteBucketAsync(bucketName);
            return Ok(response);
        }
    }
}
