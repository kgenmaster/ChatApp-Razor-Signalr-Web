using ACE_MVC.DataService;
using Microsoft.AspNetCore.Mvc;

[Route("api/chat")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly UserService _userService;

    public ChatController(UserService userService)
    {
        _userService = userService;
    }


    [HttpGet("addUser")]
    public IActionResult AddUserToContactList([FromQuery] string addedContact, string userName)
    {
        if (addedContact == userName)
        {
            return BadRequest(new { success = false, message = "You cannot add yourself as a contact." });
        }
        var contact = _userService.GetUserByUsername(addedContact);
        var DuplicateContact = _userService.DuplicateContact (userName, addedContact);
        Console.WriteLine(DuplicateContact);

        if (contact != null && DuplicateContact == false)
        {
            _userService.SaveContactToDataBase(contact, userName);
            return Ok(new { success = true, message = "Contact added successfully.", contact });
        }

        return NotFound(new { success = false, message = "Error Adding Contact." });
    }
    


    [HttpGet("getContacts")]
    public async Task<IActionResult> GetContactsList ([FromQuery] string clientname)
    {
        
        var ContactData = await _userService.FetchUserContacts(clientname);

        if (ContactData == null)
        {
            return NotFound("User not found or error occurred.");
        }

        return Ok(ContactData);
    }

    [HttpGet("getMessages")]
    public async Task<IActionResult> GetMessages ([FromQuery] string clientname, string contactname)
    {
        var MessageData = await _userService.FetchUserMessages(clientname, contactname);


        if (MessageData == null)
        {
            return NotFound("User not found or error occurred.");
        }

        return Ok(MessageData);
    }
}
