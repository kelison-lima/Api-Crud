using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Person.data;
using Person.Models;

namespace Person.Routes
{
    public static class PersonRoutes
    {
        public static void PersonRoute(this WebApplication app)
        {
            var route = app.MapGroup(prefix: "person");

            route.MapPost("", async (PersonRequest req, PersonContext context) => 
            {

                var person = new PersonModel(req.name);
                await context.AddAsync(person);
                await context.SaveChangesAsync();

             });

            route.MapGet(pattern:"", async (PersonContext  context) =>
            {
                var People = new List<PersonModel>(await context.People.ToListAsync());
                return Results.Ok(People);
            });

            route.MapPut(pattern:"{id:guid}", async (Guid id, PersonRequest req, PersonContext context) =>
            {
                var person = await context.People.FirstOrDefaultAsync(x=> x.Id == id);

                if(person == null)
                    return Results.NotFound();


                person.ChangeName(req.name);
                await context.SaveChangesAsync();
                return Results.Ok();

            });

            route.MapDelete(pattern: "{id:guid}", async (Guid id, PersonContext context) => 
            {
                 var person = await context.People.FirstOrDefaultAsync(x=> x.Id == id);

                if(person == null)
                    return Results.NotFound();

                person.SetInactive();
                await context.SaveChangesAsync();
                return Results.Ok(person);
                    
             });
        }
    }
}