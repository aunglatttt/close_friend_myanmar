using System;
using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.Models;

public class TokenCredentail
{
    [Key]
    public int Id { get; set; }

    public string DeviceId { get; set; }
    public string Token { get; set; }
}
