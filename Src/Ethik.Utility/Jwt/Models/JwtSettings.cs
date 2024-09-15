using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ethik.Utility.Jwt.Models;

public class JwtSettings
{
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int ExpiryMinutes { get; init; }
}
