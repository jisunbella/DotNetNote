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

        public IActionResult Detail(int id)
        {
            //넘어온 id에 해당하는 레코드 읽어서 board에 바인딩
            var board = _repository.GetDetailById(id);

            string content = board.Content;
            ViewBag.Content = content;

            return View(board);
        }
    }
}