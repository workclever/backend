namespace WorkCleverSolution.Services;

public interface IFileUploadService
{
    Task<string> UploadFile(string fileName, string[] directoryToUpload, IFormFile file);
}

public interface IFileUploader
{
    Task<bool> DirExists(string path);
    Task CreateDir(string path);
    Task<string> Process(string fileName, string[] directoryToUpload, IFormFile file);
}

public class RegularFileUploader : IFileUploader
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public RegularFileUploader(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public Task<bool> DirExists(string path)
    {
        return Task.FromResult(Directory.Exists(path));
    }

    public Task CreateDir(string path)
    {
        Directory.CreateDirectory(path);
        return Task.CompletedTask;
    }

    public async Task<string> Process(string fileName, string[] directoryToUpload, IFormFile originalFile)
    {
        var webRootPath = _hostingEnvironment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        if (!await DirExists(webRootPath))
        {
            await CreateDir(webRootPath);
        }

        var computedPath = Path.Combine(webRootPath);
        var accessUrl = "";
        foreach (var dir in directoryToUpload)
        {
            computedPath = Path.Join(computedPath, dir);
            if (!await DirExists(computedPath))
            {
                await CreateDir(computedPath);
            }
        }

        var extension = Path.GetExtension(originalFile.FileName);
        var finalFileName = fileName + extension;
        await using var stream = new FileStream(Path.Combine(computedPath, finalFileName), FileMode.Create);
        await originalFile.CopyToAsync(stream);
        return Path.Combine("/", Path.Combine(directoryToUpload), finalFileName);
    }
}

public class FileUploadService : IFileUploadService
{
    private readonly IFileUploader _fileUploader;

    public FileUploadService(IFileUploader fileUploader)
    {
        _fileUploader = fileUploader;
    }

    public async Task<string> UploadFile(string fileName, string[] directoryToUpload, IFormFile file)
    {
        return await _fileUploader.Process(fileName, directoryToUpload, file);
    }
}