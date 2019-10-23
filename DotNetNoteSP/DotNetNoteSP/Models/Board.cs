using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    /// <summary>
    /// Board 테이블과 일대일 매핑되는 클래스
    /// </summary>
    public class Board
    {
        [Display(Name="No.")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Date")]
        public DateTime PostDate { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "File")]
        public string FileName { get; set; }

        [Display(Name = "FileSize")]
        public int FileSize { get; set; }
    }
}
