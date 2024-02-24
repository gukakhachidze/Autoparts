namespace Autoparts.ViewModels
{
    public class EditProductViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }
        public string? URL { get; set; }
    }
}
