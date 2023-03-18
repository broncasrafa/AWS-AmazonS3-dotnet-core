﻿using Microsoft.AspNetCore.Http;
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
    /// realiza o upload de arquivos para o S3 bucket especificado
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
}
