using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MMOServer.Models;
using MMOServer.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// https://entityframework.net/one-to-many-relationship

/* Create Json: http://localhost:49560/character/create
 * https://localhost:44365/character/create
{
"Name":"CharacterName",
"Race":1,
"Realm":2,
"UserId":"f3bc29a4-bdbd-4c77-8a58-200d738107d7"
}

 *
  FString url = FString(TEXT("http://" + RPGAPIPath + "/RPGUser/GetAllCharacters/")) + UserSessionGUID;
  FString ErrorMessage = JsonObject->GetStringField("ErrorMessage");

  if (!ErrorMessage.IsEmpty())
  {
    ErrorGetAllCharacters(*ErrorMessage);
    return;
  }

  TArray<FUserCharacter> UsersCharactersData;

  if (JsonObject->HasField("rows"))
  {
    TArray<TSharedPtr<FJsonValue>> Rows = JsonObject->GetArrayField("rows");

    for (int RowNum = 0; RowNum != Rows.Num(); RowNum++) {
      FUserCharacter tempUserCharacter;
      TSharedPtr<FJsonObject> tempRow = Rows[RowNum]->AsObject();
      tempUserCharacter.CharacterName = tempRow->GetStringField("CharName");
      tempUserCharacter.ClassName = tempRow->GetStringField("ClassName");
      tempUserCharacter.Level = tempRow->GetNumberField("CharacterLevel");
      tempUserCharacter.ZoneName = tempRow->GetStringField("MapName");
      tempUserCharacter.Gender = tempRow->GetNumberField("Gender");
      tempUserCharacter.Gold = tempRow->GetNumberField("Gold");
      tempUserCharacter.Silver = tempRow->GetNumberField("Silver");
      tempUserCharacter.Copper = tempRow->GetNumberField("Copper");
      tempUserCharacter.FreeCurrency = tempRow->GetNumberField("FreeCurrency");
      tempUserCharacter.PremiumCurrency = tempRow->GetNumberField("PremiumCurrency");
      tempUserCharacter.Score = tempRow->GetNumberField("Score");
      tempUserCharacter.XP = tempRow->GetNumberField("XP");

      UsersCharactersData.Add(tempUserCharacter);
    }
  }
 */

namespace MMOServer.Controllers
{
  public class CharacterController : Controller
  {
    private readonly IMMORepository m_Repository;
    private readonly UserManager<Account> m_UserManager;

    public CharacterController(IMMORepository characterRepository,
                               UserManager<Account> userManager)
    {
      this.m_Repository = characterRepository;
      this.m_UserManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CharacterCreateViewModel model)
    {
      var user = await m_UserManager.FindByIdAsync(model.UserId);

      var response = new CharacterCreateResponseViewModel();
      response.ErrorMessage = String.Empty;

      if (user == null)
      {
        response.ErrorMessage = "Invalid UserId";
        return Json(response);
      }

      // Check name
      bool NameAlreadyTaken = m_Repository.GetAllCharacters().Any(item => item.Name == model.Name);

      if (NameAlreadyTaken)
      {
        response.ErrorMessage = "Name Already taken";
        return Json(response);
      }

      Character character = new Character()
      {
        Name = model.Name,
        AccountSlot = model.AccountSlot,
        Gender = model.Gender,
        Race = model.Race,
        Class = model.Class,
        Realm = model.Realm,
        Account = user
      };

      m_Repository.Add(character);

      return Json(response);
    }

    // GetAllCharacters?userID=
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllCharacters(string userId)
    {
      var response = new CharacterGetAllResonseViewModel();
      response.Characters = m_Repository.GetAllCharacters().Where(character => character.CurrentAccountId == userId);

      if (response.Characters != null)
      {
        response.ErrorMessage = String.Empty;
      }
      else
      {
        response.ErrorMessage = "No Characters Found";
      }

      return Json(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCharacter(string characterName)
    {

      var character = m_Repository.GetAllCharacters().FirstOrDefault(item => item.Name == characterName);

      if (character != null)
      {
        //response.ErrorMessage = String.Empty;
      }
      else
      {
        //response.ErrorMessage = "No Characters Found";
      }

      return Json(character);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Delete(string characterName)
    {
      var res = m_Repository.GetAllCharacters().FirstOrDefault(item => item.Name == characterName);

      var response = new CharacterDeleteResponseViewModel();

      if (res != null)
      {
        m_Repository.Delete(res.CharacterId);
        response.ErrorMessage = String.Empty;
      }
      else
      {
        response.ErrorMessage = "NoSuchCharacter";
      }


      return Json(response);
    }
  }
}
