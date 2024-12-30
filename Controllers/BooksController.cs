
using Microsoft.AspNetCore.Mvc;
namespace project.Controllers;
using project.Interfaces;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private  IBookService bookService;
    public BooksController (IBookService bookService)
    {
        this.bookService = bookService;
    }
    [HttpGet]
     public ActionResult<List<Book>> GetAll() =>
             bookService.GetAll();
    
    [HttpGet("{id}")]
    public ActionResult<Book> Get(int id)
    {
            var book = bookService.Get(id);
            if (book == null)
                return NotFound();
            return book;
    }
    [HttpPost] 
    public ActionResult Insert(Book nb)
    {        
       bookService.Add(nb);
       return CreatedAtAction(nameof(Insert), new {id=nb.Id}, nb);
    }  
    [HttpPut("{id}")]
    public ActionResult Update(int id, Book nb)
    { 
        if(id != nb.Id)  
           return BadRequest();

        var existingBook = bookService.Get(id);

        if(existingBook is null)
           return NotFound();
        bookService.Update(nb);
           return NoContent();
    } 
   [HttpDelete("{id}")]
   public ActionResult Delete(int id){
        var foundBook = bookService.Get(id);
         if (foundBook is null) 
           return  NotFound();
        bookService.Delete(id);
        return Content(bookService.Count.ToString());
   }
}