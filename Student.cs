using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DesktopUI
{
    public class Student
    {
        [Key]
        [Required]
        public string RegNo { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public byte[] ImageBytes { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public double GPA { get; set; }

        [NotMapped]
        public BitmapImage Image { get; set; }


        public Student(string regNo, string fullName, string department, BitmapImage image, double gpa)
        {
            RegNo = regNo;
            FullName = fullName;
            Department = department;
            GPA = gpa;
            Image = image;
        }



       
        public Student()
        {
        }


        //public String ImagePath
        //{
        //    get { return $"/Images/{Image}"; }
        //}


    }
}
