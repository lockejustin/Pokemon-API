using System;
using System.Collections.Generic;

namespace PokemonAPI.Models
{
    public partial class FavoritePokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public int? PokemonId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Stats { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BaseExp { get; set; }
        public string Sprite { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
