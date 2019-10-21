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


        public IActionResult Index()
        {
            IEnumerable<Board> board;
            board = _repository.GetBoards(1);

            return View(board);
        }
    }
}