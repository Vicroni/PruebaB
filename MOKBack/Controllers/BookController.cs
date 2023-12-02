using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MOKBack.Models;
using System.Data.SqlClient;

namespace MOKBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private SqlConnection con;

        public BookController(IConfiguration configuration) {
            this.con = new SqlConnection(configuration.GetConnectionString("Conexion"));
            this.con.Open();
        }
        [HttpGet]
        // GET: book
        // Trae todos los libros
        public IEnumerable<Book> Index()
        {
            List<Book> books = new List<Book>();

            SqlCommand command = new SqlCommand("exec sp_GetAllBooks", this.con);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Book book = new Book(Convert.ToInt32(reader["BookId"]),
                                         Convert.ToString(reader["Title"]),
                                         Convert.ToString(reader["Author"]),
                                         Convert.ToString(reader["Genre"]),
                                         Convert.ToDateTime(reader["PublishDate"]));
                    books.Add(book);
                }
            }
            return books;
        }

        [HttpGet]
        [Route("details/{id?}")]
        // GET: book/details/5
        // Trae un libro por id
        public Book Details(int id)
        {
            Book book = new Book();
            SqlCommand command = new SqlCommand("exec sp_GetBookByID @Id", this.con);
            command.Parameters.AddWithValue("@Id", id);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    book = new Book(Convert.ToInt32(reader["BookId"]),
                                       Convert.ToString(reader["Title"]),
                                       Convert.ToString(reader["Author"]),
                                       Convert.ToString(reader["Genre"]),
                                       Convert.ToDateTime(reader["PublishDate"]));
                }
            }
            return book;
        }

        // POST: book/create
        // Crea un libro
        [HttpPost]
        [Route("create")]
        public String Create([FromBody]Book book)
        {
            SqlCommand command = new SqlCommand("exec sp_InsertBook @Title, @Author, @PublishDate, @Genre", this.con);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            string s = book.PublishDate.ToString();
            command.Parameters.AddWithValue("@PublishDate", book.PublishDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Genre", book.Genre);
            command.ExecuteNonQuery();
            return "Creado con exito";
        }

        // POST: book/update/5
        // Edita un libro
        [HttpPost]
        [Route("update/{id?}")]
        public String Update(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] Book book)
        {
            SqlCommand command = new SqlCommand("exec sp_UpdateBook @Id, @Title, @Author, @PublishDate, @Genre", this.con);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@Author", book.Author);
            string s = book.PublishDate.ToString();
            command.Parameters.AddWithValue("@PublishDate", book.PublishDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@Genre", book.Genre);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return "El id ingresado no existe";
            }
            return "Actualizado con exito";
        }

        [HttpGet]
        [Route("delete/{id?}")]
        // GET: book/delete/5
        // Borra un libro
        public String Delete(int id)
        {
            SqlCommand command = new SqlCommand("exec sp_DeleteBook @Id", this.con);
            command.Parameters.AddWithValue("@Id", id);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex)
            {
                return "El id ingresado no existe";
            }
            return "Borrado con exito";
        }
    }
}
