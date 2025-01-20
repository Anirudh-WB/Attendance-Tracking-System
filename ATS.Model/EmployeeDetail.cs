using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Model
{
    [Table("EmployeeDetails")]
    public class EmployeeDetail : BaseEntity
    {
        public virtual User User { get; set; }

        [ForeignKey("Users"), Required(ErrorMessage = "User ID is required")]
        public long UserId { get; set; }
        public string? EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is Required"), MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is Required"), MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        public virtual Designation Designation { get; set; }

        [ForeignKey("Designations")]
        public long DesignationId { get; set; } = 1;
        public virtual Gender Gender { get; set; }

        [ForeignKey("Genders")]
        public long GenderId { get; set; } = 2;

        public string ProfilePic { get; set; } = string.Empty;

        public byte[] FaceEncoding { get; set; } = [];
    }
}