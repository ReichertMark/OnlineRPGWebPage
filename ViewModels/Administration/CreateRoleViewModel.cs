﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.ViewModels.Administration
{
  public class CreateRoleViewModel
  {
    [Required]
    public string RoleName { get; set; }
  }
}
