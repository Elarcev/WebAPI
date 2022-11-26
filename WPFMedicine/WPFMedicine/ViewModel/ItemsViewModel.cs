using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using WPFMedicine.Model;
using System.Net.Http.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFMedicine.ViewModel
{
    public class ItemsViewModel : INotifyPropertyChanged
    {
        static HttpClient client = new HttpClient();

        public List<Item> Items = new List<Item>();

        public Item _selectedItem { get; set; }



        public Item SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ConnectionData()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://localhost:7182/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Uri> CreateItemAsync(Item item)
        {
                HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/items", item);
                response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        public async Task<Item> GetItemAsync(int from = 1 ,int to = 50) //для получения объектов в диапазон (перегрузка метода)
        {
            Item item = null;
            for(int i = from; i < to; i++)
            {
                HttpResponseMessage response = await client.GetAsync($"api/items/{i}");
                if (response.IsSuccessStatusCode)
                {
                    item = await response.Content.ReadAsAsync<Item>();
                    Items.Add(item);
                }
            }

            return item;
        }

        public async Task<Item> GetItemAsync(int id) //для поиска одного объекта (перегрузка метода)
        {
            Item item = null;
                HttpResponseMessage response = await client.GetAsync($"api/items/{id}");
                if (response.IsSuccessStatusCode)
                {
                    item = await response.Content.ReadAsAsync<Item>();
                }
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/items", item);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated item from the response body.
            item = await response.Content.ReadAsAsync<Item>();
            return item;
        }

        public async Task<HttpStatusCode> DeleteItemAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/items/{id}");
            return response.StatusCode;
        }

       
    }
}
