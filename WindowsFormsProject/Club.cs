using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProject
{
    public class Club
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; }
        public string Origin { get; set; }
        public string League { get; set; }
        public int Playerid { get; set; }

        internal static void Add(Club club)
        {
            throw new NotImplementedException();
        }
    }
}
