using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetNote.Models
{
    public interface IBoardRepository
    {
        //Board 페이지에서 사용
        List<Board> GetBoards(int page);
        
    }
}
