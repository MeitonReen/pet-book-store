using System.ComponentModel.DataAnnotations;

namespace BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0
{
    public abstract class BaseDataPartRequest
    {
        [Required] public int PartLength { get; set; } = 20;
        [Required] public int PartNumber { get; set; } = 1;
    }
}