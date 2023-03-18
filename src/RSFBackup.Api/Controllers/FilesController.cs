using Microsoft.AspNetCore.Mvc;
using RSFBackup.Core.DTO.Files;
using RSFBackup.Core.Interfaces.Repositories;

namespace RSFBackup.Api.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFilesRepository _filesRepository;

    public FilesController(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }


    /// <summary>
    /// Obtem a lista de arquivos do S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    [HttpGet("{bucketName}")]
    public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFilesFromBucketAsync(string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });

        var response = await _filesRepository.ListFilesAsync(bucketName);
        if (response == null || response.Count() == 0)
            return NotFound(new { message = $"Files from S3 Bucket {bucketName} not found" });

        return Ok(response);
    }


    /// <summary>
    /// Realiza o upload de arquivos para o S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    /// <param name="files">arquivo(s) a serem enviados para o S3 bucket</param>
    [HttpPost("{bucketName}")]
    public async Task<ActionResult<AddFileResponse>> AddFilesAsync(string bucketName, IFormFileCollection files)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });

        if (files == null || files.Count == 0)
            return BadRequest(new { message = "The request doesn't contain any files to be uploaded" });

        var response = await _filesRepository.UploadFilesAsync(bucketName, files);
        if (response == null)
            return BadRequest(new { message = $"Unable to upload files to S3 bucket {bucketName}" });

        return Ok(response);
    }


    /// <summary>
    /// Realiza o download do arquivo especificado no S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    /// <param name="filename">nome do arquivo</param>
    /// <returns></returns>
    [HttpGet("{bucketName}/download/{filename}")]
    public async Task<IActionResult> DownloadFileAsync(string bucketName, string filename) 
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });
        if (string.IsNullOrWhiteSpace(filename))
            return BadRequest(new { message = "File name is required" });

        await _filesRepository.DownloadFileAsync(bucketName, filename);

        return Ok(new { message = "Downloaded successfully" });
    }


    /// <summary>
    /// Realiza a remoção do arquivo especificado no S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    /// <param name="filename">nome do arquivo</param>
    /// <returns></returns>
    [HttpDelete("{bucketName}/{filename}")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string filename)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });
        if (string.IsNullOrWhiteSpace(filename))
            return BadRequest(new { message = "File name is required" });

        var response = await _filesRepository.DeleteFileAsync(bucketName, filename);

        return Ok(new { message = "File deleted successfully" });
        // return NoContent();
    }


    /// <summary>
    /// Realiza o upload de um objeto JSON para o S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    /// <param name="request">objeto JSON stringifyed</param>
    [HttpPost("{bucketName}/json-object")]
    public async Task<IActionResult> AddJsonObjectAsync(string bucketName, AddJsonObjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });

        var response = await _filesRepository.AddJsonObjectAsync(bucketName, request);

        return Ok(new { message = "Object created successfully", id = response });
    }


    /// <summary>
    /// Obtem o objeto JSON pelo nome do arquivo no S3 bucket especificado
    /// </summary>
    /// <param name="bucketName">o nome do S3 bucket</param>
    /// <param name="filename">nome do arquivo JSON armazenado no S3 bucket</param>
    [HttpGet("{bucketName}/json-object")]
    public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObjectAsync(string bucketName, [FromQuery] string filename)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return BadRequest(new { message = "S3 Bucket name is required" });
        if (string.IsNullOrWhiteSpace(filename))
            return BadRequest(new { message = "File name is required" });

        var response = await _filesRepository.GetJsonObjectAsync(bucketName, filename);
        return Ok(response);
    }
}
