using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProject
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public string PlayerPosition { get; set; }
        public string Picture { get; set; }
        public bool IsPlaying { get; set; }
    }
}
