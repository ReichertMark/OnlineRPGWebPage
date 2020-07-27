using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMOServer.Models;
using MMOServer.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMOServer.Controllers
{
  public class InventoryController : Controller
  {
    private readonly IMMORepository m_Repository;

    public InventoryController(IMMORepository repository)
    {
      m_Repository = repository;
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody]InventoryItemAddViewModel model)
    {
      var response = String.Empty;
      var character = m_Repository.GetAllCharacters().FirstOrDefault(item => item.Name == model.CharacterName);

      if (character == null)
      {
        response = "Invalid UserId";
      }

      InventoryItem addItem = new InventoryItem()
      {
        Character = character,
        TemplateId = model.TemplateId,
        SlotPosition = model.SlotPosition,
        StackSize = model.StackSize
      };

      await m_Repository.AddItem(addItem);

      return Json(response);
    }

    // GetAllCharacters?characterName=
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllItems(string characterName)
    {
      var response = new InventoryItemGetAllResponseViewModel();
      response.ErrorMessage = String.Empty;

      var character = m_Repository.GetAllCharacters().FirstOrDefault(item => item.Name == characterName);

      if (character != null)
      {
        response.InventoryItems = m_Repository.GetAllInventoryItems().Where(item => item.CurrentOwnerId == character.CharacterId);
      }
      else
      {
        response.ErrorMessage = "No Character found";
      }

      return Json(response);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult DeleteItem(long id)
    {
      m_Repository.DeleteItem(id);

      var response = String.Empty;

      return Json(response);
    }

  }
}
