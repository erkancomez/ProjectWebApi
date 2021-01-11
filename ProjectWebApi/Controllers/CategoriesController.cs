using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApi.DAL.Context;
using ProjectWebApi.DAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebApi.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            using var context = new ProjectWebApiContext();
            return Ok(context.Categories.ToList());
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using var context = new ProjectWebApiContext();
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPut]
        public IActionResult UpdateCategory(Category category)
        {
            using var context = new ProjectWebApiContext();
            var updatedCategory = context.Find<Category>(category.Id);
            if (updatedCategory == null)
                return NotFound();
            updatedCategory.Name = category.Name;
            context.Update(updatedCategory);
            context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var context = new ProjectWebApiContext();
            var deletedCategory = context.Categories.Find(id);
            if (deletedCategory == null)
            {
                return NotFound();
            }
            context.Remove(deletedCategory);
            context.SaveChanges();
            return NoContent();
        }
        [HttpPost]
        public IActionResult Post(Category category)
        {
            using var context = new ProjectWebApiContext();
            context.Categories.Add(category);
            context.SaveChanges();
            return Created("", category);
        }

        [HttpGet("{id}/blogs")]
        public IActionResult GetWithBlogsById(int id)
        {
            using var context = new ProjectWebApiContext();
            var categoryWithBlogs = context.Categories.Where(I => I.Id == id).Include(I => I.Blogs).ToList();
            if (categoryWithBlogs==null)
            {
                return NotFound();
            }
            return Ok(categoryWithBlogs);
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file.ContentType == "image/jpeg")
            {
                var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents/" + newFileName);
                var stream = new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);
                return Created("", file);
            }
            return BadRequest();
        }

    }
}
