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
        /// <returns></returns>
        public List<Board> GetBoards(int page = 1)
        {
            _logger.LogInformation("데이터 출력");
            try
            {
                var parameters = new DynamicParameters(new { Page = page });
                return con.Query<Board>("Select Id, Title, Name, PostDate From Board").ToList(); 
            }
            catch(Exception ex)
            {
                _logger.LogError("데이터 출력 에러: " + ex);
                return null;
            }
        }
    }
}
