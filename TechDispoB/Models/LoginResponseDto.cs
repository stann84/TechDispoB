﻿namespace TechDispoB.Models
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
    }
}
