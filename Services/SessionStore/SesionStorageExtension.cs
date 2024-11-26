using Blazored.SessionStorage;
using System.Text.Json;

namespace ConsolaBlazor.Services.SessionStore
{
    public static class SesionStorageExtension
    {

        public static async Task GuardarStorage<T>(this ISessionStorageService sessionStorageService, string key, T item, TimeSpan expiration) where T : class
        {
            var itemWithExpiration = new ItemWithExpiration<T>
            {
                Item = item,
                Expiration = DateTime.UtcNow.Add(expiration)
            };

            var itemJson = JsonSerializer.Serialize(itemWithExpiration);
            await sessionStorageService.SetItemAsStringAsync(key, itemJson);

        }

        public static async Task<T?> ObtenerStorage<T>(this ISessionStorageService sessionStorageService, string key) where T : class
        {

            var itemJson = await sessionStorageService.GetItemAsStringAsync(key);
            if (itemJson == null)
            {
                return null;
            }

            var itemWithExpiration = JsonSerializer.Deserialize<ItemWithExpiration<T>>(itemJson);


            if (itemWithExpiration == null || itemWithExpiration.Expiration < DateTime.UtcNow)
            {
                await sessionStorageService.RemoveItemAsync(key);
                return null;
            }

            return itemWithExpiration.Item;

        }



        private class ItemWithExpiration<T>
        {
            public T Item { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
