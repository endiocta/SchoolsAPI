using System;
using System.Collections.Generic;

namespace SchoolsAPI.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string NomorInduk { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? BirthPlace { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? StandardId { get; set; }

    public virtual Standard? Standard { get; set; }

    public virtual ICollection<StudentAddress> StudentAddresses { get; set; } = new List<StudentAddress>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
