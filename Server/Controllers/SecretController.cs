﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class SecretController: Controller
    {
        [Authorize]
        public string Index()
        {
            return "Api secret message";
        }
    }
}
