using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeSpace.BackendServer.Data.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        [Required]
        public string SeoAlias { get; set; }

        [MaxLength(500)]
        public string SeoDescription { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public int? ParentId { get; set; }

        public int? NumberOfTickets { get; set; }
    }
}