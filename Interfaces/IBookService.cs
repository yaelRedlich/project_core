
using System.Collections.Generic;
using System.Linq;
namespace project.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAll();

        Book ?Get(int id);

        void Add(Book book);

        void Delete(int id);

        void Update(Book book);

        int Count { get;}  //
    }
}