using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerRework.Features.Added.Vip
{
    public class Team
    {
        public Player plr { get; set; }
        public RoleTypeId Role { get; set; }
        public bool IsVip { get; set; }
        public Team(Player plr, RoleTypeId Role, bool IsVip = false)
        {
            this.plr = plr;
            this.Role = Role;
            this.IsVip = IsVip;
        }
    }
}
