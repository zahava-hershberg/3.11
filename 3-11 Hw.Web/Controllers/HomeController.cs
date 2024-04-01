using _3_11_Hw.Data;
using _3_11_Hw.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace _3_11_Hw.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=FilesDb; Integrated Security=true;";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(string password, IFormFile image)
        {
            var fileName = image.FileName;
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);

            var mgr = new ImageManager(_connectionString);
            var i = new Images()
            {
                Name = fileName,
                Password = password
            };

            int id = mgr.AddImage(i);
            return View(new ImageViewModels
            {
                Image = new Images
                {
                    Id = id,
                    Name = fileName,
                    Password = password
                }
            });
        }

        public IActionResult ViewImage(int id, string password)
        {
            var mgr = new ImageManager(_connectionString);
            var image = mgr.GetImageById(id);
            var vm = new ImageViewModels();
            vm.Image = image;
            vm.ShowImage = false;
            if (password != image.Password)
            {
                vm.Message = "Invalid Password";
            }
            else
            {
                vm.ShowImage = true;
                mgr.SetViews(id);
            }

            List<int> ids = HttpContext.Session.Get<List<int>>("id");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (!ids.Contains(id))
            {
                ids.Add(id);
                HttpContext.Session.Set("id", ids);
            }

            return View(vm);
        }


    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}