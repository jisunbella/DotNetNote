using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotNetNote.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            // 첨부된 파일 확인
            if (String.IsNullOrEmpty(board.FileName))
            {
                ViewBag.FileName = "(업로드된 파일이 없습니다.)";
            }
            else
            {           
                // 이미지 미리보기:
                if (Dul.BoardLibrary.IsPhoto(board.FileName))
                {
                    ViewBag.ImageDown = $"<img src=\'/files/{board.FileName}\'><br />";
                }
            }
            return View(board);
        }

        /// <summary>
        /// ImageDown : 완성형(DotNetNote) 게시판의 이미지전용다운 페이지
        /// 이미지 경로를 보여주지 않고 다운로드: 
        ///    대용량 사이트 운영시 직접 이미지 경로 사용 권장(CDN 사용)
        /// /DotNetNote/ImageDown/1234 => 1234번 이미지 파일을 강제 다운로드
        /// <img src="/DotNetNote/ImageDown/1234" /> => 이미지 태그 실행
        /// </summary>
        public IActionResult ImageDown(int id)
        {
            string fileName = "";

            // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에 파일명 숨김)
            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                string strFileName = fileName;
                string strFileExt = Path.GetExtension(strFileName);
                string strContentType = "";
                if (strFileExt == ".gif" || strFileExt == ".jpg"
                    || strFileExt == ".jpeg" || strFileExt == ".png")
                {
                    switch (strFileExt)
                    {
                        case ".gif":
                            strContentType = "image/gif"; break;
                        case ".jpg":
                            strContentType = "image/jpeg"; break;
                        case ".jpeg":
                            strContentType = "image/jpeg"; break;
                        case ".png":
                            strContentType = "image/png"; break;
                    }
                }

                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName))
                {
                    // 이미지 파일 정보 얻기
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(
                        _environment.WebRootPath, "files") + "\\" + fileName);

                    // 이미지 파일 다운로드 
                    return File(fileBytes, strContentType, fileName);
                }

                return Content("http://placehold.it/250x150?text=NoImage");
            }
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
        public async Task<IActionResult> Write(Board board, ICollection<IFormFile> files)
        {
            //파일 업로드 처리 시작
            string fileName = String.Empty;
            int fileSize = 0;

            var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileSize = Convert.ToInt32(file.Length);
                    // 파일명 중복 처리
                    fileName = Dul.FileUtility.GetFileNameWithNumbering(
                        uploadFolder, Path.GetFileName(
                            ContentDispositionHeaderValue.Parse(
                                file.ContentDisposition).FileName.Trim('"')));
                    // 파일 업로드
                    using (var fileStream = new FileStream(
                        Path.Combine(uploadFolder, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            Board b = new Board();

            b.Title = board.Title;
            b.Name = board.Name;
            b.Content = board.Content;
            b.Password = board.Password;
            b.FileName = fileName;
            b.FileSize = fileSize;

            _repository.WriteArticle(b); //데이터 저장

            TempData["Message"] = "데이터가 저장되었습니다.";

            return RedirectToAction("Index"); // 저장 후 리스트 페이지로 이동
        }

        /// <summary>
        /// 게시판 파일 강제 다운로드 기능(/BoardDown/:Id)
        /// </summary>
        public FileResult BoardDown(int id)
        {
            string fileName = "";

            // 넘겨져 온 번호에 해당하는 파일명 가져오기(보안때문에... 파일명 숨김)
            fileName = _repository.GetFileNameById(id);

            if (fileName == null)
            {
                return null;
            }
            else
            {
                // 다운로드 카운트 증가 메서드 호출
                //_repository.UpdateDownCountById(id);

                if (System.IO.File.Exists(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(_environment.WebRootPath, "files") + "\\" + fileName);

                    return File(fileBytes, "application/octet-stream", fileName);
                }

                return null;
            }
        }

        /// <summary>
        /// 게시판 삭제 폼
        /// </summary>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 게시판 삭제 처리
        /// </summary>
        [HttpPost]
        public IActionResult Delete(int id, string Password)
        {
            //if (_repository.DeleteArticle(id,
            //    Common.CryptorEngine.EncryptPassword(Password)) > 0)
            if (_repository.DeleteArticle(id, Password) > 0)
            {
                TempData["Message"] = "데이터가 삭제되었습니다.";

                // 학습 목적으로 삭제 후의 이동 페이지를 2군데 중 하나로 분기
                if (DateTime.Now.Second % 2 == 0)
                {
                    //[a] 삭제 후 특정 뷰 페이지로 이동
                    return RedirectToAction("DeleteCompleted");
                }
                else
                {
                    //[b] 삭제 후 Index 페이지로 이동
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Message = "삭제되지 않았습니다. 비밀번호를 확인하세요.";
            }

            ViewBag.Id = id;
            return View();
        }

        /// <summary>
        /// 게시판 삭제 완료 후 추가적인 처리할 때 페이지
        /// </summary>
        public IActionResult DeleteCompleted()
        {
            return View();
        }

        /// <summary>
        /// 게시판 수정 폼
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.FormType = 1;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            // 기존 데이터를 바인딩
            var note = _repository.GetDetailById(id);

            return View(note);
        }

        /// <summary>
        /// 게시판 수정 처리 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(Board model, int id)
        {
            ViewBag.FormType = 1;
            ViewBag.TitleDescription = "글 수정 - 아래 항목을 수정하세요.";
            ViewBag.SaveButtonText = "수정";

            //string fileName = "";
            //int fileSize = 0;

            //if (previousFileName != null)
            //{
            //    fileName = previousFileName;
            //    fileSize = previousFileSize;
            //}

            ////파일 업로드 처리 시작
            //var uploadFolder = Path.Combine(_environment.WebRootPath, "files");

            //foreach (var file in files)
            //{
            //    if (file.Length > 0)
            //    {
            //        fileSize = Convert.ToInt32(file.Length);
            //        // 파일명 중복 처리
            //        fileName = Dul.FileUtility.GetFileNameWithNumbering(
            //            uploadFolder, Path.GetFileName(
            //                ContentDispositionHeaderValue.Parse(
            //                    file.ContentDisposition).FileName.Trim('"')));
            //        // 파일업로드
            //        using (var fileStream = new FileStream(
            //            Path.Combine(uploadFolder, fileName), FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //    }
            //}

            Board board = new Board();

            board.Id = id;
            board.Name = model.Name;
            board.Title = Dul.HtmlUtility.Encode(model.Title);
            board.Content = model.Content;
            board.Password = model.Password;

            int r = _repository.UpdateArticle(board); // 데이터베이스에 수정 적용
            if (r > 0)
            {
                TempData["Message"] = "수정되었습니다.";
                return RedirectToAction("Detail", new { Id = id });
            }
            else
            {
                ViewBag.ErrorMessage =
                    "업데이트가 되지 않았습니다. 암호를 확인하세요.";
                return View(board);
            }
        }
    }
}