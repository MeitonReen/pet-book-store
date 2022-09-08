﻿using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.Profile.V1_0_0.Delete
{
    public class DeleteRequest
    {
        [Required] public Guid UserId { get; set; }
    }
}