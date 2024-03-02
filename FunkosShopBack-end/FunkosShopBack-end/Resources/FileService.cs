namespace FunkosShopBack_end.Resources
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAsync(Stream stream, string name)
        {
            using MemoryStream streamAux = new MemoryStream();
            await stream.CopyToAsync(streamAux);
            byte[] bytes = streamAux.ToArray();

            return await SaveAsync(bytes, name);
        }

        public async Task<string> SaveAsync(byte[] bytes, string name)
        {
            string directory = Path.Combine("wwwroot", "images");
            Directory.CreateDirectory(directory);

            string absolutePath = Path.Combine(directory, name);
            await File.WriteAllBytesAsync(absolutePath, bytes);

            string relativePath = $"images/{name}";

            return relativePath;
        }
    }
}
