using BulkyBookWeb.Data;
using BulkyBookWeb.Models.TypesenseModel;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BulkyBookWeb.Controllers
{
    public class TypesenseCollectionController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TypesenseCollectionController(ApplicationDbContext db)
        {
            _db = db;
        }

        ////local hosted server connection test 
        public async Task<IActionResult> ConnectionTestWithServer()
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

        //get all collection
        public async Task<IActionResult> GetCollection()
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8108/collections");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");

            var response = await httpClient.SendAsync(request);
            var content = "";
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle failed response here
            }
            return Json(content);
        }

        ////create collection by static data
        public async Task<IActionResult> CreateCollection()
        {

            string collectionSchema = @"{
              ""name"": ""Books1"",
              ""fields"": [
                {""name"": ""id"", ""type"": ""string"", ""facet"": false},
                {""name"": ""title"", ""type"": ""string"", ""facet"": false},
                {""name"": ""title2"", ""type"": ""string"", ""facet"": false},
                {""name"": ""author"", ""type"": ""string"", ""facet"": true},
                {""name"": ""publication_date"", ""type"": ""int64"", ""facet"": true},
                {""name"": ""genres"", ""type"": ""string[]"", ""facet"": true}
              ],
              ""default_sorting_field"": ""publication_date"",
              ""default_sorting_order"": ""desc""
            }";
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8108/collections");
            request.Content = new StringContent(collectionSchema, Encoding.UTF8, "application/json");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");

            var response = await httpClient.SendAsync(request);
            return Json(response);
        }

       
        ////create collection by model list
        public async Task<IActionResult> CreateCollectionByModel()
        {
            IEnumerable<Category> objCategoryList = _db.Categories;
            string schema = GetCollectionSchema(objCategoryList, "categoryName").ToString();
            //string json = JsonConvert.SerializeObject(schema);
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8108/collections");
            request.Content = new StringContent(schema, Encoding.UTF8, "application/json");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");
            var response = await httpClient.SendAsync(request);
            return Json(response);
        }

        public static CollectionSchema GetCollectionSchema<T>(IEnumerable<T> models, string collectionName)
        {
            var properties = typeof(T).GetProperties();
            var fields = new List<FieldSchema>();

            foreach (var prop in properties)
            {
                var type = prop.PropertyType.Name.ToLower();

                if (type == "int32")
                {
                    type = "int";
                }

                fields.Add(new FieldSchema
                {
                    Name = prop.Name,
                    Type = type
                });
            }

            return new CollectionSchema
            {
                Name = collectionName,
                Fields = fields
            };
        }



        ////document
        //search document
        public async Task<IActionResult> SearchDocument(string search)
        {
            HttpClient httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8108/collections/Product/documents/search?q="+search+ "&query_by=name");
            request.Headers.Add("X-TYPESENSE-API-KEY", "xyz");
            var response = await httpClient.SendAsync(request);
            var content = "";
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle failed response here
            }
            return Json(content);
        }

    }
}

