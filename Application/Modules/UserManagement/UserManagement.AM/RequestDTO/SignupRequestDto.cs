using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.AM.RequestDTO
{
    public sealed class SignupRequestDto
    {
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string ConfirmPassword { get; init; }

    }
}
