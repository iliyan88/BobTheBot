using RJ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public class Recipient : Entity<int>
    {
        private Recipient()
        {

        }

        protected Recipient(int id) : base(id)
        {
        }

        public Recipient(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}
