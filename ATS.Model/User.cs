﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Model
{
    [Table("Users")]
    [Index(nameof(Email),IsUnique = true)]

    public class User : BaseEntity
    {
        [Required(ErrorMessage = "Email is required"), MaxLength(50), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Password is Required"), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(10), MinLength(10)]
        public string ContactNo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual EmployeeDetail EmployeeDetail { get; set; }

        [ForeignKey("EmployeeDetails")]
        public long? EmployeeDetailsId { get; set; }

        public virtual Role Role { get; set; }

        [ForeignKey("Roles")]
        public long RoleId { get; set; }
    }
}
