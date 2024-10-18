using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Data
{
    //DbContext로부터 상속받지 않고 IdentityDbContext로 부터 상속받는다
    // IdentityDbContext 는 모든 user table를 포함하고 있다
    public class AppDbContext: IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }
    }
}
