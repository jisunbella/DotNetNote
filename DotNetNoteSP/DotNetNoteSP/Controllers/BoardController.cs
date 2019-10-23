using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetNote.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Controllers
{
    public class BoardController : Controller
    {
        private IHostingEnvironment _environment; //환경 변수
        private IBoardRepository _repository; //게시판 리포지토리

        /// <summary>
        /// 현재 보여줄 페이지 번호
        /// </summary>
        public int PageIndex { get; set; } = 1;

        public BoardController(IHostingEnvironment environment, IBoardRepository repository)
        {
            _environment = environment;
            _repository = repository;
        }

        /// <summary>
        /// 게시판 리스트
        /// </summary>
        /// <returns>Index.cshtml</returns>
        public IActionResult Index()
        {
            IEnumerable<Board> board;
            board = _repository.GetBoards(1);

            return View(board);
        }

        /// <summary>
        /// 글 상세보기
        /// </summary>
        /// <param name="id">글 번호</param>
        public IActionResult Detail(int id)
        {
            //넘어온 id에 해당하는 레코드 읽어서 board에 바인딩
            var board = _repository.GetDetailById(id);

            string content = board.Content;
            ViewBag.Content = content;

            return View(board);
        }

        /// <summary>
        /// 글쓰기
        /// </summary>
        [HttpGet]
        public IActionResult Write()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Write(Board board)
        {
            Board b = new Board();

            b.Title = board.Title;
            b.Name = board.Name;
            b.Content = board.Content;
            b.Password = board.Password;

            _repository.WriteArticle(b); //데이터 저장

            TempData["Message"] = "데이터가 저장되었습니다.";

            return RedirectToAction("Index"); // 저장 후 리스트 페이지로 이동
        }

    }
}