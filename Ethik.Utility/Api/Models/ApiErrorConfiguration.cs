namespace Ethik.Utility.Api.Models;

using System.Collections.Generic;

public class ApiErrorConfiguration
{
    public Dictionary<string, ApiError> Errors { get; set; } = new Dictionary<string, ApiError>();
}

