using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProject
{
    public interface ICrossDataGet
    {
        void ReloadData (List<Player> players);
        void UpdatePlayer (Player p);
        void RemovePlayer(int id);
        void ReloadData2(List<Club> clubs);
        void RemoveClub(int id);
        void UpdateClub(Club club);
    }
}
