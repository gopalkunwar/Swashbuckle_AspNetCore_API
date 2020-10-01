using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Entity;
using Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Controller performs CRUD operations on TodoItems
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all TodoItems
        /// </summary>
        /// <remarks>
        /// sample request:
        ///     GET /api/todoitems
        ///     
        ///     Sample response
        ///   [  {
        ///     "Id":1,
        ///     "Title":"Car Maintenance",
        ///     "Completed":true
        /// }]
        ///   
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Returns all the TodoItems</response>
        /// <response code="404">If the item is not found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        {
            var todoItem = await _context.TodoItems.ToListAsync();

            if (todoItem == null) return NotFound();

            return Ok(todoItem);
        }

        /// <summary>
        /// Gets a TodoItem by ID
        /// </summary>
        /// <remarks>
        /// 
        ///     sample request:
        ///         GET /api/todoitem/1
        ///         
        /// sample response:
        ///     {
        ///         "Id":1,
        ///         "name":"Car Maintenance",
        ///         "Completed":true
        ///     }
        /// </remarks>
        /// <param name="id">The ID of a TodoItem.</param>
        /// <returns></returns>
        /// <response code="200">Returns the item with the specified ID</response>
        /// <response code="404">If the item is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItem>> GetById(int id)
        {
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x=>x.Id==id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }


        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /api/todoitem
        ///         {
        ///         "id":3,
        ///         "title":"Car Wash",
        ///         "Completed":false
        ///     }
        ///     
        /// Sample response body:
        ///     {
        ///     "id":3,
        ///     "title":"Car Wash",
        ///     "Completed":false
        ///     }
        ///     
        /// Sample response header:
        ///     
        ///     For IIS:
        ///     
        ///     content-type: application/json; charset=utf-8
        ///     date: Thu01 Oct 2020 06:37:41 GMT
        ///     location: http://localhost:58588/api/TodoItem/4 
        ///     server: Microsoft-IIS/10.0 
        ///     transfer-encoding: chunked
        ///     x-powered-by: ASP.NET
        ///      
        /// 
        /// </remarks>
        /// <param name="todoItem"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id=todoItem.Id}, todoItem);

        }

        /// <summary>
        /// Modifies a ToDoItem
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///PUT /api/todoitem/3
        ///         {
        ///         "id":3,
        ///         "title":"Car Wash",
        ///         "Completed":false
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="todoItem"></param>
        /// <returns>A newly updated TodoItem</returns>
        /// <response code="200">Returns all the Todoitems</response>
        /// <response code="400">If the request is invalid</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Update(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id) return BadRequest();

            var toDoItem = await _context.TodoItems.FirstOrDefaultAsync(x=>x.Id==id);
            if (toDoItem == null) return NotFound();
            toDoItem.Title = todoItem.Title;
            toDoItem.Completed = todoItem.Completed;
            _context.TodoItems.Update(toDoItem);
            await _context.SaveChangesAsync();
            return NoContent();
            
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes a TodoItem</returns>
        /// <response code="200"> Returns a deleted TodoItem</response>
        /// <response code="404"> If the item is null</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }
    }
}
