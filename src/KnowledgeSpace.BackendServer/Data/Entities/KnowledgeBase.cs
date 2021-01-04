using KnowledgeSpace.BackendServer.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KnowledgeSpace.BackendServer.Data.Entities
{
    [Table("KnowledgeBases")]
    public class KnowledgeBase : IDateTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Range(1, Double.PositiveInfinity)]
        public int CategoryId { get; set; }

        [MaxLength(500)]
        [Required]
        public string Title { get; set; }

        [MaxLength(500)]
        [Required]
        [Column(TypeName = "varchar(500)")]
        public string SeoAlias { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string Environment { get; set; }

        [MaxLength(500)]
        public string Problem { get; set; }

        public string StepToReproduce { get; set; }

        [MaxLength(500)]
        public string ErrorMessage { get; set; }

        [MaxLength(500)]
        public string Workaround { get; set; }

        public string Note { get; set; }

        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string OwnerUserId { get; set; }

        public string Labels { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? NumberOfComments { get; set; }

        public int? NumberOfVotes { get; set; }

        public int? NumberOfReports { get; set; }

        public int? ViewCount { get; set; }
    }
}