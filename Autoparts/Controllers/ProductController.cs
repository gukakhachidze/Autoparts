using Autoparts.Interfaces;
using Autoparts.Models;
using Autoparts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Autoparts.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IPhotoService _photoService;
        public ProductController(IProductRepository productRepository, IPhotoService photoService)
        {
            _productRepository = productRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _productRepository.GetAll();
            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _productRepository.GetByIdAsync(id);
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel productVM)
        {
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(productVM.Image);
                var product = new Product
                {
                    Title = productVM.Title,
                    Description = productVM.Description,
                    Image = result.Url.ToString(),
                    Price = productVM.Price,
                };

                _productRepository.Add(product);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload faild");
            }

            return View(productVM);
            
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
				return RedirectToAction("Index");
			}
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return View("Error");

            var productVM = new EditProductViewModel
            {
                Title = product.Title,
                Description = product.Description,
                URL = product.Image,
                Price = product.Price,
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditProductViewModel productVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit product");
                return View("Edit", productVM);
            }

            var userProduct = await _productRepository.GetByIdNoTrackingAsync(id);
            if (userProduct != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userProduct.Image);
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Could not delete photo");
                    return View(productVM);
                }
                
                var photoResult = await _photoService.AddPhotoAsync(productVM.Image);

                var produt = new Product
                {
                    Id = id,
                    Title = productVM.Title,
                    Description = productVM.Description,
                    Image = photoResult.Url.ToString(),
                    Price = productVM.Price,
                };

                _productRepository.Update(produt);
                return RedirectToAction("Index");
            }
            else
            {
                return View(productVM);
            }
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var productDetails = await _productRepository.GetByIdAsync(id);
            if (productDetails == null) return View("Error");
            return View(productDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productDetails = await _productRepository.GetByIdAsync(id);
            if (productDetails == null) return View("Error");
            
            _productRepository.Delete(productDetails);
            return RedirectToAction("Index");
        }
    }
}
