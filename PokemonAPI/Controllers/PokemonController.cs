using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PokemonAPI.Controllers
{
    public class PokemonController : Controller
    {
        private readonly PokemonDAL _pokemonDAL;
        private readonly PokemonDbContext _pokemonContext;

        public PokemonController()
        {
            _pokemonDAL = new PokemonDAL();
            _pokemonContext = new PokemonDbContext();
        }


        public IActionResult SearchPokemon()
        {
            return View();
        }

        public IActionResult WhosThatPokemon()  //easter egg nod to the show
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SearchPokemonByName(string searchName)  //searches the API for Pokemon by name with validation in case name is not found
        {
            if (searchName == null)
            {
                ViewBag.Message = "That's not a real Pokémon!  Try again!";
                return View("SearchPokemon");
            }
            
            var pokemon = await _pokemonDAL.GetPokemonByName(searchName);

            if (pokemon != null)
            {
                return View("SearchResults", pokemon);
            }
            else
            {
                ViewBag.Message = "That's not a real Pokémon!  Try again!";
                return View("SearchPokemon");
            }
            
        }

        public async Task<IActionResult> SearchPokemonById(int id) //searches the API for Pokemon by Id
        {
            var pokemon = await _pokemonDAL.GetPokemonById(id);

            return View("SearchResults", pokemon);
        }

        public IActionResult UpdatePokemon(int id) 
        {
            FavoritePokemon pokemon = _pokemonContext.FavoritePokemon.Find(id);
            if (pokemon == null)
            {
                return RedirectToAction("Favorites");
            }
            else
            {
                return View(pokemon);
            }
        }


        public IActionResult SaveChanges(FavoritePokemon updatedName) //saves changes when user adds nickname to favorited pokemon
        {
            FavoritePokemon pokemonName = _pokemonContext.FavoritePokemon.Find(updatedName.Id);
            pokemonName.Nickname = updatedName.Nickname;

            _pokemonContext.Entry(pokemonName).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _pokemonContext.Update(pokemonName);
            _pokemonContext.SaveChanges();

            return RedirectToAction("Favorites");
        }

        [Authorize]
        public IActionResult Favorites() //navigates to favorite pokemon view with filtering if a user is logged in
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (id != null && id != "")
            {
                List<FavoritePokemon> myPokemon = _pokemonContext.FavoritePokemon.Where(x => x.UserId == id).ToList();

                return View(myPokemon);
            }
            else
            {
                List<FavoritePokemon> myPokemon = _pokemonContext.FavoritePokemon.ToList();

                return View(myPokemon);
            }
        }
      
        [Authorize]
        public async Task<IActionResult> AddPokemon(int id) //adds and saves favorited pokemon to the database
        {
            string activeUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value; //finds the user id of the logged in user

            var newFav = new FavoritePokemon();
            var newpokemon =await  _pokemonDAL.GetPokemonById(id);

            string typeConcat = "";
            string statConcat = "";
            string spriteConcat = "";

            for (int i = 0; i < newpokemon.types.Length; i++) //turns types information into a comma separated value to store in db
            {
                typeConcat += newpokemon.types[i].type.name;
                if (i != newpokemon.types.Length - 1)
                {
                    typeConcat += ",";
                }
            }

            for (int i = 0; i < newpokemon.stats.Length; i++) //turns stats information into a comma separated value to store in db
            {
                statConcat += newpokemon.stats[i].stat.name;
                statConcat += ": ";
                statConcat += newpokemon.stats[i].base_stat;
                if (i != newpokemon.stats.Length - 1)
                {
                    statConcat += ",";
                }
            }

            spriteConcat += newpokemon.sprites.front_default + "," + newpokemon.sprites.front_shiny; //turns sprites information into a comma separated value to store in db

            newFav.Name = newpokemon.name;
            newFav.PokemonId = newpokemon.id;
            newFav.Height = newpokemon.height.ToString();
            newFav.Sprite = spriteConcat;
            newFav.Type = typeConcat;
            newFav.Weight = newpokemon.weight.ToString();
            newFav.BaseExp = newpokemon.base_experience.ToString();
            newFav.Stats = statConcat;
            newFav.UserId = activeUserId;
           
            if (ModelState.IsValid)
            {
                _pokemonContext.FavoritePokemon.Add(newFav);
                _pokemonContext.SaveChanges(); 
            }
            return RedirectToAction("Favorites");
        }

        public IActionResult DeletePokemon(int id) //removes pokemon from the favorites list
        {
            var deletePokemon = _pokemonContext.FavoritePokemon.Find(id);
            if (deletePokemon != null)
            {
                _pokemonContext.FavoritePokemon.Remove(deletePokemon);
                _pokemonContext.SaveChanges();
            }
            return RedirectToAction("Favorites");

        }
    }
}
