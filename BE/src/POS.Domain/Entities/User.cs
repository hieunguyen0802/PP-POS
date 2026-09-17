using POS.Domain.Common;

namespace POS.Domain.Entities;

public class User : EntityBase
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    private User() { } // For EF Core
    public User(string username, string passwordHash, string firstName, string lastName, string email, string phoneNumber, UserRole role)
    {
        ValidateUser(username, passwordHash, firstName, lastName, email, phoneNumber);
        Username = username;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public void UpdateUser(string firstName, string lastName, string email, string phoneNumber, UserRole role)
    {
        ValidateUser(firstName: firstName, lastName: lastName, email: email, phoneNumber: phoneNumber);
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public void ChangeActiveStatus()
    {
        IsActive = !IsActive;

    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    private void ValidateUser(string username, string passwordHash, string firstName, string lastName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        ValidateUser(firstName, lastName, email, phoneNumber);  // reuse the shared checks below
    }

    private void ValidateUser(string firstName, string lastName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));
    }
}