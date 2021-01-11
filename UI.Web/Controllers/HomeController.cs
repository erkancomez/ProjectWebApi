using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UI.Web.Models;

namespace UI.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var httpClient = new HttpClient();

            var responseMessage = await httpClient.GetAsync("http://localhost:50959/api/categories");

            var jsonString = await responseMessage.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(jsonString);
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            using var httpClient = new HttpClient();
            var jsonCategory = JsonConvert.SerializeObject(category);
            StringContent content = new StringContent(jsonCategory,Encoding.UTF8,"application/json");
            var responseMessage = await httpClient.PostAsync("http://localhost:50959/api/categories",content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Bir sorun oluştu");

            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync("http://localhost:50959/api/categories/"+id);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonCategory = await responseMessage.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<Category>(jsonCategory);
                return View(category);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            using var httpClient = new HttpClient();
            var jsonCategory = JsonConvert.SerializeObject(category);
            var content = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.PutAsync("http://localhost:50959/api/categories/", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Bir sorun oluştu");

            return View(category);
        }

        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var bytes = stream.ToArray();

            ByteArrayContent fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            MultipartFormDataContent formData = new MultipartFormDataContent
            {
                { fileContent, "file", file.FileName }
            };

            using var httpClient = new HttpClient();
            var responseMessage = await httpClient.PostAsync("http://localhost:50959/api/categories/upload", formData);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Bir hata oluştu");
            return View(file);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

}
