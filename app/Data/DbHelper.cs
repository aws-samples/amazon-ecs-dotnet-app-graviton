using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace MvcMovie.Data
{
    public static class DbHelper
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            UpdateDatabase(serviceProvider);
            RegisterUsers(serviceProvider);
            InitializeMovies(serviceProvider);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
             using (var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>()))
            {
                context.Database.Migrate();
            }
        }

        private static void RegisterUsers(IServiceProvider serviceProvider)
        {
            using (var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>()))
            {
                // Look for any movies.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                var appUser = new AppUser
                {
                    UserName = "admin"
                };

                userManager.CreateAsync(appUser, "Pass@word1").GetAwaiter().GetResult();
            }
        }

        private static void InitializeMovies(IServiceProvider serviceProvider)
        {
            using (var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>()))
            {
                // Look for any movies.
                if (context.Movie.Any())
                {
                    return;   // DB has been seeded
                }

                context.Movie.AddRange(GetMovies());
                context.SaveChanges();
            }
        }
        
        private static Movie[] GetMovies()
        {
            return new[]
            {
                new Movie
                    {
                        Title = "Parasite",
                        Year = 2019,
                        Director = "Bong Joon-ho",
                        Genre = "Comedy Thriller"
                    },

                    new Movie
                    {
                        Title = "Green Book",
                        Year = 2018,
                        Director = "Peter Farrelly",
                        Genre = "Comedy Drama"
                    },

                    new Movie
                    {
                        Title = "The Shape Of Water",
                        Year = 2017,
                        Director = "Guillermo del Toro",
                        Genre = "Drama Thriller"
                    },

                    new Movie
                    {
                        Title = "Moonlight",
                        Year = 2016,
                        Director = "Barry Jenkins",
                        Genre = "Drama"
                    },

                    new Movie
                    {
                        Title = "Spotlight",
                        Year = 2015,
                        Director = "Tom McCarthy",
                        Genre = "Drama History"
                    },

                    new Movie
                    {
                        Title = "Birdman",
                        Year = 2014,
                        Director = "Alejandro Gonzalez Inarritu",
                        Genre = "Drama Romance"
                    },

                    new Movie
                    {
                        Title = "12 Years a Slave",
                        Year = 2013,
                        Director = "Steve McQueen",
                        Genre = "Drama History"
                    },

                    new Movie
                    {
                        Title = "Argo",
                        Year = 2012,
                        Director = "Ben Affleck",
                        Genre = "Drama Thriller"
                    },

                    new Movie
                    {
                        Title = "The Artist",
                        Year = 2011,
                        Director = "Michel Hazanavicius",
                        Genre = "Drama Romance"
                    },

                    new Movie
                    {
                        Title = "The King's Speech",
                        Year = 2010,
                        Director = "Tom Hooper",
                        Genre = "Drama History"
                    }
            }; 
        }
    }
}