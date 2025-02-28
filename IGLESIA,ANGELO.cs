using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static List<Item> items = new List<Item>();
    private static int idCounter = 1;

    [HttpPost]
    public IActionResult CreateItem([FromBody] Item newItem)
    {
        if (string.IsNullOrEmpty(newItem.Name) || string.IsNullOrEmpty(newItem.Description))
        {
            return BadRequest(new { error = "Name and description are required" });
        }
        newItem.Id = idCounter++;
        items.Add(newItem);
        return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
    }

    [HttpGet]
    public IActionResult GetItems()
    {
        return Ok(items);
    }

    [HttpGet("{id}")]
    public IActionResult GetItem(int id)
    {
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound(new { error = "Item not found" });
        }
        return Ok(item);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateItem(int id, [FromBody] Item updatedItem)
    {
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound(new { error = "Item not found" });
        }
        if (!string.IsNullOrEmpty(updatedItem.Name)) item.Name = updatedItem.Name;
        if (!string.IsNullOrEmpty(updatedItem.Description)) item.Description = updatedItem.Description;
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItem(int id)
    {
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound(new { error = "Item not found" });
        }
        items.Remove(item);
        return NoContent();
    }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}