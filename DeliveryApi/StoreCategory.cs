using System.Text;
using System.Text.Json;


namespace storecategoryapi
{
    public static class StoreCategoryService
    {
        static string firebaseDatabaseUrl = "https://deliveryapp-f7365-default-rtdb.firebaseio.com/";
        static string firebaseDatabaseDocument = "StoreCategory";
        static readonly HttpClient client = new HttpClient();
        public static async Task<Category> Add(Category category) {
            category.Id = Guid.NewGuid().ToString("N");
            string categoryJsonString = JsonSerializer.Serialize(category);
            var payload =  new StringContent(categoryJsonString, Encoding.UTF8, "application/json");
            string url = $"{firebaseDatabaseUrl}" +
                        $"{firebaseDatabaseDocument}/" +
                        $"{category.Id}.json";

            var httpResponseMessage = await client.PutAsync(url, payload);

            if(httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<Category>(contentStream);
                return result;
            }

            return null;

        }
    

    public static async Task<Category> GetById(string id)
    {
        string url = $"{firebaseDatabaseUrl}" +
                        $"{firebaseDatabaseDocument}/" +
                        $"{id}.json";

        var httpResponseMessage = await client.GetAsync(url);

        if (httpResponseMessage.IsSuccessStatusCode)
            {
            var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
            if (contentStream != null && contentStream != "null")
                {
                var result = JsonSerializer.Deserialize<Category>(contentStream);

                return result;
                }               
            }

        return null;
    }

    public static async Task<List<Category>> GetAll()
        {
            string url = $"{firebaseDatabaseUrl}" +
                        $"{firebaseDatabaseDocument}.json";

            var httpResponseMessage = await client.GetAsync(url);
            List<Category> entries = new List<Category>();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                if (contentStream != null && contentStream != "null")
                {
                    var result = JsonSerializer.Deserialize<Dictionary<string, Category>>(contentStream);

                    entries = result.Select(x => x.Value).ToList();
                }
            }

            return entries;

        }

    public static async Task<string> DeleteById(string id)
        {
            string url = $"{firebaseDatabaseUrl}" +
                        $"{firebaseDatabaseDocument}/" +
                        $"{id}.json";

            var httpResponseMessage = await client.DeleteAsync(url);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                if(contentStream == "null")
                {
                    return "Deleted";
                }
            }

            return null;
        }

    public static async Task<Category> Update(Category category, string id)
        {
            category.Id = id;
            string categoryJsonString = JsonSerializer.Serialize(category);

            var payload = new StringContent(categoryJsonString, Encoding.UTF8, "application/json");

            string url = $"{firebaseDatabaseUrl}" +
                        $"{firebaseDatabaseDocument}/" +
                        $"{id}.json";

            var httpResponseMessage = await client.PutAsync(url, payload);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();

                if (contentStream != null && contentStream != "null")
                {
                    var result = JsonSerializer.Deserialize<Category>(contentStream);

                    return result;
                }         
            }

            return null;
            }

        }

    public class Category 
    {
        public string Id { get ; set;}
        public string Name {get ; set;}
        public string Image {get ; set;}
    }
}