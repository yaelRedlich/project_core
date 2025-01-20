
using project;
using project.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
namespace project_core.Services
{
   public class BookService :IBookService
    {
        List<Book> listbooks { get; }
        int nextId = 4;
        public BookService()
        {
            listbooks = new List<Book>
            {
              new Book { Id = 1, Name = "math" ,Category=CategoryBooks.Textbook},
              new Book { Id = 2, Name = "With a nation builder", Category = CategoryBooks.History},
              new Book { Id = 3, Name = "The country", Category = CategoryBooks.Philosophy}
            };
        }

        public List<Book> GetAll() => listbooks;

        //public Book? Get(int id) => listbooks.FirstOrDefault(b => b.Id == id);
        public Book Get(int id)
        {
           var book = listbooks.FirstOrDefault(b => b.Id == id);
           if (book == null)
         {
           throw new IndexOutOfRangeException($"Book with ID {id} does not exist.");
         }
            return book;
        }
        public void Add(Book book)
        {
            book.Id = nextId++;
            listbooks.Add(book);
        }

        public void Delete(int id)
        {
            var book = Get(id);
            if(book is null)
                return;
            listbooks.Remove(book);
        }

        public void Update(Book book)
        {
            var index = listbooks.FindIndex(b => b.Id == book.Id);
            if(index == -1)
                return;

            listbooks[index] = book;
        }

        public int Count { get =>  listbooks.Count(); }
    }
    public static class BookServiceHelper
    {
        public static void AddPizzaService(this IServiceCollection services)
        {
            services.AddSingleton<IBookService , BookService>();    
        }
    }
    
}
 