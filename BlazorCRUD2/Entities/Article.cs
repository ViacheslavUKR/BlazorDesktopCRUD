using System.ComponentModel.DataAnnotations;

namespace BlazorCRUD2.Entities
{
    public class Article
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
    }


}
