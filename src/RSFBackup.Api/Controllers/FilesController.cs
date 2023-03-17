using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSFBackup.Core.Interfaces.Repositories;

namespace RSFBackup.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesRepository _filesRepository;

        public FilesController(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }
    }
}
