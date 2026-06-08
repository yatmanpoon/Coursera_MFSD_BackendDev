using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private static readonly List<User> Users =
    [
        new User
        {
            Id = 1,
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada.lovelace@example.com"
        },
        new User
        {
            Id = 2,
            FirstName = "Alan",
            LastName = "Turing",
            Email = "alan.turing@example.com"
        }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        return Ok(Users);
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create(CreateUserRequest request)
    {
        var firstName = request.FirstName?.Trim();
        var lastName = request.LastName?.Trim();
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return BadRequest(new { Message = "First name and last name are required." });
        }

        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
        {
            return BadRequest(new { Message = "A valid email address is required." });
        }

        var emailExists = Users.Any(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

        if (emailExists)
        {
            return Conflict(new { Message = "A user with this email already exists." });
        }

        var user = new User
        {
            Id = Users.Select(u => u.Id).DefaultIfEmpty(0).Max() + 1,
            FirstName = firstName!,
            LastName = lastName!,
            Email = email!
        };

        Users.Add(user);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public ActionResult<User> Update(int id, UpdateUserRequest request)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        var emailBelongsToAnotherUser = Users.Any(u =>
            u.Id != id && string.Equals(u.Email, request.Email, StringComparison.OrdinalIgnoreCase));

        if (emailBelongsToAnotherUser)
        {
            return Conflict(new { Message = "A user with this email already exists." });
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        Users.Remove(user);
        return NoContent();
    }
}

public record CreateUserRequest(string FirstName, string LastName, string Email);

public record UpdateUserRequest(string FirstName, string LastName, string Email);
