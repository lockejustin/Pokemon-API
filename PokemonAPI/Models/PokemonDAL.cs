using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokemonAPI.Models
{
    public class PokemonDAL
    {
        public HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");

            return client;
        }

        public async Task<Pokemon> GetPokemonByName(string searchName) //searches api for pokemon by name
        {
            var client = GetClient();
            var response = await client.GetAsync($"pokemon/{searchName}");

            if (response.IsSuccessStatusCode == true)
            {
                 var pokemon = await response.Content.ReadAsAsync<Pokemon>();
                 return pokemon;
            }
            else
            {
                return null;
            }
        }
        public async Task<Pokemon> GetPokemonById(int id) //searches api for pokemon by id number
        {
            var client = GetClient();
            var response = await client.GetAsync($"pokemon/{id.ToString()}");
            var pokemon = await response.Content.ReadAsAsync<Pokemon>();

            return pokemon;
        }
    }
}
