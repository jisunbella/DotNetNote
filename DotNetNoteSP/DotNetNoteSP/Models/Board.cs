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
        [Display(Name="번호")]
        public int Id { get; set; }

        [Display(Name = "작성자")]
        public string Name { get; set; }

        [Display(Name = "제목")]
        public string Title { get; set; }

        [Display(Name = "작성일")]
        public DateTime PostDate { get; set; }

        [Display(Name = "내용")]
        public string Content { get; set; }

        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        [Display(Name = "파일")]
        public string FileName { get; set; }

        [Display(Name = "파일크기")]
        public int FileSize { get; set; }
    }
}
