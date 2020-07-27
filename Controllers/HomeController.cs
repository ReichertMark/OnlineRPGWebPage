using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MMOServer.Models;
using MMOServer.Security;
using MMOServer.ViewModels;
using MMOServer.ViewModels.Home;

namespace MMOServer.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly IMMORepository _context;
    private readonly IDataProtector protector;

    public HomeController(IMMORepository context,
                          IDataProtectionProvider dataProtectionProvider,
                          DataProtectionPurposeStrings dataProtectionPurposeStrings)
    {
      _context = context;

      this.protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.CharacterIdRouteValue);
    }

    [AllowAnonymous]
    public ViewResult Index()
    {
      var model = _context.GetAllCharacters();
      return View(model);
    }

    [AllowAnonymous]
    public ViewResult Details(long id)
    {
      Character character = _context.GetCharacter(id);
    
      if (character == null)
      {
        Response.StatusCode = 404;
        return View("UserAccountNotFound", id);
      }
    
      HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
      {
        Character = character,
        PageTitle = "User Account Details"
      };
    
      return View(homeDetailsViewModel);
    }

    // GET: api/TodoItems
    [HttpGet]
    public ViewResult Create()
    {
      return View();
    }

    // GET: api/TodoItems
    [HttpGet]
    public ViewResult Edit(long id)
    {
       Character item = _context.GetCharacter(id);
      // 
      //       UserAccountEditViewModel editItem = new UserAccountEditViewModel()
      //       {
      //         Id = item.Id,
      //         Name = item.Name,
      //         Email = item.Email,
      //         AccountType = item.AccountType,
      //         ExistingImagePath = item.ImagePath
      //       };
      // 
      return View(item);//View(editItem);
    }

    // GET: api/TodoItems
    [HttpPost]
    public IActionResult Edit(CharacterEditViewModel item)
    {
//       if (ModelState.IsValid)
//       {
//         Character character = _context.GetCharacter(item.Id);
//         character.Name = item.Name;
//         character.Email = item.Email;
//         character.AccountType = item.AccountType;
// 
//         if (item.Photo != null)
//         {
//           if (item.ExistingImagePath != null)
//           {
//             string filePath = Path.Combine(_hostingEnv.WebRootPath, "images", item.ExistingImagePath);
//             System.IO.File.Delete(filePath);
//           }
// 
//           character.ImagePath = ProcessUploadedFile(item);
//         }
// 
//         _context.Update(character);
//         return RedirectToAction("index");
//       }

      return View();
    }

    private string ProcessUploadedFile(CharacterCreateViewModel item)
    {
      string uniqueFileName = null;
//       if (item.Photo != null)
//       {
//         string uploadFolder = Path.Combine(_hostingEnv.WebRootPath, "images");
//         uniqueFileName = Guid.NewGuid().ToString() + "_" + item.Photo.FileName;
//         string filePath = Path.Combine(uploadFolder, uniqueFileName);
// 
//         using (var fileStream = new FileStream(filePath, FileMode.Create))
//         {
//           item.Photo.CopyTo(fileStream);
//         }
//       }

      return uniqueFileName;
    }

    // GET: api/TodoItems
    [HttpPost]
    public IActionResult Create(CharacterCreateViewModel item)
    {
//       if (ModelState.IsValid)
//       {
//         string uniqueFileName = ProcessUploadedFile(item);
// 
//         Character character = new Character()
//         {
//           Name = item.Name,
//           Email = item.Email,
//           AccountType = item.AccountType,
//           ImagePath = uniqueFileName
//         };
//           
//         _context.Add(character);
//         return RedirectToAction("Details", new { id = character.Id });
//       }

      return View();
    }

  }
}
