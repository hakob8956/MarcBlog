﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace MySite.Models
{
    public static class SeedData
    {

        public static void EnsurePopulated(IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "MySite1",

                            Description = "Description1",
                            Category = "Category1",
                            Text = "Hiiii Fack1",
                            Author = "MyBlog",
                            DateTime = DateTime.Now

                        },
                         new Post
                         {
                             Title = "MySite2",
                             Description = "Description2",
                             Category = "Category2",
                             Text = "Hiiii Fack2",
                             Author = "MyBlog",
                             DateTime = DateTime.Now
                         },
                          new Post
                          {
                              Title = "MySite3",
                              Description = "Description3",
                              Category = "Category3",
                              Text = "Hiiii Fack3",
                              Author = "MyBlog",
                              DateTime = DateTime.Now
                          },
                           new Post
                           {
                               Title = "MySite4",
                               Description = "Description2",
                               Category = "Category2",
                               Text = "Hiiii Fack2",
                               Author = "MyBlog",
                               DateTime = DateTime.Now
                           },
                          new Post
                          {
                              Title = "MySite5",
                              Description = "Description3",
                              Category = "Category3",
                              Text = "Hiiii Fack3",
                              Author = "MyBlog",
                              DateTime = DateTime.Now
                          },
                           new Post
                           {
                               Title = "MySite6",
                               Description = "Description2",
                               Category = "Category2",
                               Text = "Hiiii Fack2",
                               Author = "MyBlog",
                               DateTime = DateTime.Now
                           },
                          new Post
                          {
                              Title = "MySite7",
                              Description = "Description3",
                              Category = "Category3",
                              Text = "Hiiii Fack3",
                              Author = "MyBlog",
                              DateTime = DateTime.Now
                          },
                           new Post
                           {
                               Title = "MySite8",
                               Description = "Description4",
                               Category = "Category4",
                               Text = "Hiiii Fack4",
                               Author = "MyBlog",
                               DateTime = DateTime.Now
                           }

                        );
                    context.SaveChanges();
                }
            }
        }
    }
}
