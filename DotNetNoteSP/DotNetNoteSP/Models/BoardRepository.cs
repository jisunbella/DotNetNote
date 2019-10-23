using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    public class BoardRepository : IBoardRepository
    {
        private IConfiguration _config;
        private SqlConnection con;
        private ILogger<BoardRepository> _logger; // 로그를 표시하거나 저장

        /// <summary>
        /// 환경변수와 로그 개체 주입
        /// </summary>
        public BoardRepository(IConfiguration config, ILogger<BoardRepository> logger)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
        }

        /// <summary>
        /// 게시판 리스트
        /// </summary>
        /// <param name="page">페이지 번호</param>
        public List<Board> GetBoards(int page)
        {
            _logger.LogInformation("데이터 출력");
            try
            {
                var parameters = new DynamicParameters(new { Page = page });
                return con.Query<Board>("[SP_GetList]", parameters, commandType: CommandType.StoredProcedure).ToList(); 
            }
            catch(Exception ex)
            {
                _logger.LogError("데이터 출력 에러: " + ex);
                return null;
            }
        }

        /// <summary>
        /// 글 상세보기
        /// </summary>
        /// <param name="id">글 번호</param>
        public Board GetDetailById(int id)
        {
            var parameters = new DynamicParameters(new { Id = id });
            //return con.Query<Board>("SELECT Id, Title, Name, PostDate, Content FROM Board WHERE Id = @Id", parameters).SingleOrDefault();
            return con.Query<Board>("[SP_GetDetail]", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        /// <summary>
        /// 글 작성하기
        /// </summary>
        public void WriteArticle(Board board)
        {
            _logger.LogInformation("데이터 입력");
            try
            {
                var p = new DynamicParameters();

                p.Add("@Title", value: board.Title, dbType: DbType.String);
                p.Add("@Name", value: board.Name, dbType: DbType.String);
                p.Add("@Content", value: board.Content, dbType: DbType.String);
                p.Add("@Password", value: board.Password, dbType: DbType.String);
                p.Add("@FileName", value: board.FileName, dbType: DbType.String);
                p.Add("@FileSize", value: board.FileSize, dbType: DbType.Int32);

                con.Execute("[SP_WriteArticle]", p, commandType: CommandType.StoredProcedure);
                
                
            }
            catch(System.Exception ex)
            {
                _logger.LogError("데이터 입력 에러: " + ex);
            }
        }

        /// <summary>
        /// 파일 이름 가져오기
        /// </summary>
        public string GetFileNameById(int id) => con.Query<string>("SELECT FileName FROM Board WHERE Id = @Id", new { Id = id }).SingleOrDefault();

        /// <summary>
        /// 글 삭제 
        /// </summary>
        public int DeleteArticle(int id, string password) =>
             con.Execute("[SP_DeleteArticle]", new { Id = id, Password = password }, commandType: CommandType.StoredProcedure);

        /// <summary>
        /// 글 수정
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public int UpdateArticle(Board board)
        {
            int r = 0;
            _logger.LogInformation("데이터 수정");
            try
            {
                var p = new DynamicParameters();

                p.Add("@Id", value: board.Id, dbType: DbType.Int32);
                p.Add("@Title", value: board.Title, dbType: DbType.String);
                p.Add("@Name", value: board.Name, dbType: DbType.String);
                p.Add("@Content", value: board.Content, dbType: DbType.String);
                p.Add("@Password", value: board.Password, dbType: DbType.String);
                p.Add("@FileName", value: board.FileName, dbType: DbType.String);
                p.Add("@FileSize", value: board.FileSize, dbType: DbType.Int32);

                r = con.Execute("[SP_UpdateArticle]", p, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("데이터 수정 에러: " + ex);
            }
            return r;
        }


    }
}
