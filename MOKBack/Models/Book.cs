using System.ComponentModel;

namespace MOKBack.Models
{
    public class Book
    {
        public Book(){}
            public Book(int id, string title, string author, string genre, DateTime publishDate)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            PublishDate = publishDate;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public DateTime PublishDate { get; set; }
    }
}