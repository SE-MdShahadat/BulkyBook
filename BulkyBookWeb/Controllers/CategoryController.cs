using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Typesense;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            return View(objCategoryList);
        }
        //GET
        public IActionResult Create()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id==0) 
            {
                return NotFound();
            }
            var categoryFromDb = _db.Categories.Find(id);
            //var categoryFromDbFirst =_db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryFromDbFirst =_db.Categories.FirstOrDefault(u=>u.Id==id);
            if(categoryFromDb==null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = _db.Categories.Find(id);
            //var categoryFromDbFirst =_db.Categories.FirstOrDefault(u=>u.Id==id);
            //var categoryFromDbFirst =_db.Categories.FirstOrDefault(u=>u.Id==id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var categoryFromDb = _db.Categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

        //Typesense

        ////Connection test to local host
        public async Task<IActionResult> TypesenseConnectionTest()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8108/");

            try
            {
                HttpResponseMessage response = await client.GetAsync("health");
                response.EnsureSuccessStatusCode(); // Throw an exception if not successful
                string responseBody = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseBody);
                return Ok(responseBody);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
        ////Create Demo Collection server
        public async Task<IActionResult> TypesenseCreateCollection()
        {
            
            string collectionSchema = @"{
              ""name"": ""Books_03"",
              ""fields"": [
                {""name"": ""id_03"", ""type"": ""string"", ""facet"": false},
                {""name"": ""title_03"", ""type"": ""string"", ""facet"": false},
                {""name"": ""author_03"", ""type"": ""string"", ""facet"": true},
                {""name"": ""publication_date_03"", ""type"": ""int64"", ""facet"": true},
                {""name"": ""genres_03"", ""type"": ""string[]"", ""facet"": true}
              ],
              ""default_sorting_field"": ""publication_date_03"",
              ""default_sorting_order"": ""desc""
            }";
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8108/collections");
            request.Content = new StringContent(collectionSchema, Encoding.UTF8, "application/json");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");

            var response = await httpClient.SendAsync(request);
            return Json(response);


        }
        ////Create Demo Collection server
        public async Task<IActionResult> TypesenseGetCollection()
        {

            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8108/collections");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");

            var response = await httpClient.SendAsync(request);
            var content="";
            if (response.IsSuccessStatusCode)
            {
                 content = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(content); // Or do something else with the list of collections
            }
            else
            {
                // Handle failed response here
            }
            return Json(content);


        }

        ////Try by typesense client
        //public async Task<IActionResult> CreateTypesenseCollection(ITypesenseClient typesenseClient)
        //{

        //    var schema = new Schema(
        //    "Addresses",
        //    new List<Field>
        //    {
        //        new Field("id", FieldType.Int32, false),
        //        new Field("houseNumber", FieldType.Int32, false),
        //        new Field("accessAddress", FieldType.String, false, true),
        //        new Field("metadataNotes", FieldType.String, false, true, false),
        //    },
        //    "houseNumber");

        //    var createCollectionResult = await typesenseClient.CreateCollection(schema);
        //    return Ok(createCollectionResult);

        //}
    }
}
