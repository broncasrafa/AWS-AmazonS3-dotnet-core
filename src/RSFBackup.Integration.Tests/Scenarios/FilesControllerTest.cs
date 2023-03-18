using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RSFBackup.Core.DTO.Files;
using RSFBackup.Core.DTO.Bucket;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Newtonsoft.Json;


namespace RSFBackup.Integration.Tests.Scenarios;

[Collection("api")]
public class FilesControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    private const string _bucketName = "testS3Bucket";
    private const string _filename = "integration_test.jpg";
    private const string _pathToSave = $@"C:\temp\";
    private const string _baseEnpointUriBucketController = "api/bucket";
    private const string _baseEnpointUriFilesController = "api/files";


    public FilesControllerTest(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAWSService<IAmazonS3>(new AWSOptions
                {
                    DefaultClientConfig =
                    {
                        ServiceURL = "http://localhost:9003"
                    },
                    Credentials = new BasicAWSCredentials("FAKE", "FAKE")
                });
            });
        }).CreateClient();

        Task.Run(CreateBucketAsync).Wait();
    }

    private async Task CreateBucketAsync()
    {
        await _httpClient.PostAsJsonAsync(_baseEnpointUriBucketController, new CreateS3BucketRequest { BucketName = _bucketName });
    }

    private async Task<HttpResponseMessage> UploadFileToS3BucketAsync()
    {
        string path = string.Concat(_pathToSave, _filename);
        var file = File.Create(path);
        HttpContent fileStreamContent = new StreamContent(file);

        var formData = new MultipartFormDataContent
        {
            { fileStreamContent, "files", _filename }
        };

        var endpoint = $"{_baseEnpointUriFilesController}/{_bucketName}";
        var response = await _httpClient.PostAsync(endpoint, formData);

        fileStreamContent.Dispose();
        formData.Dispose();

        return response;
    }

    [Fact]
    public async Task When_AddFiles_Returned_Ok()
    {
        var response = await UploadFileToS3BucketAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_ListFiles_endpoint_is_hit_our_result_is_not_null()
    {
        await UploadFileToS3BucketAsync();

        var response = await _httpClient.GetAsync($"{_baseEnpointUriFilesController}/{_bucketName}");

        ListFilesResponse[] result;
        using (var content = response.Content.ReadAsStringAsync())
        {
            result = JsonConvert.DeserializeObject<ListFilesResponse[]>(await content);
        }

        Assert.NotNull(result);
    }

    [Fact]
    public async Task When_DownloadFiles_endpoint_is_hit_we_are_returned_ok_status()
    {
        await UploadFileToS3BucketAsync();

        var endpoint = $"{_baseEnpointUriFilesController}/{_bucketName}/download/{_filename}";
        var response = await _httpClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_DeleteFile_endpoint_is_hit_we_are_returned_ok_status()
    {
        await UploadFileToS3BucketAsync();

        var endpoint = $"{_baseEnpointUriFilesController}/{_bucketName}/{_filename}";
        var response = await _httpClient.DeleteAsync(endpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_AddJsonObject_endpoint_is_hit_we_are_returned_ok_status()
    {
        var jsonObjectRequest = new AddJsonObjectRequest
        {
            Id = Guid.NewGuid(),
            Data = "Test-Data",
            TimeSent = DateTime.UtcNow
        };

        var endpoint = $"{_baseEnpointUriFilesController}/{_bucketName}/json-object";
        var response = await _httpClient.PostAsJsonAsync(endpoint, jsonObjectRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
