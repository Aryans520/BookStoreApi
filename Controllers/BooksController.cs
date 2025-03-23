using BookStoreApi.Data;
using BookStoreApi.Dtos;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStoreApi.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreApiDbContext _context;

        public BooksController(BookStoreApiDbContext context)
        {
            _context = context;
        }
        // GET: api/books
        [Route("api/[controller]")]
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        //{
        //    return await _context.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category).ToListAsync();
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Categories = b.BookCategories.Select(bc => new CategoryDto
                    {
                        Id = bc.Category.Id,
                        Name = bc.Category.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(books);
        }
        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return book;
        }
        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
            
        }
        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
