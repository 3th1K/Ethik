using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ethik.Utility.Jwt.Models;

public class JwtSettings
{
    public string SecretKey { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int ExpiryMinutes { get; init; }
}
