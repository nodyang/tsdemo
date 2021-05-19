using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Ts.Core
{
    public class DtoModel : IValidatableObject
    {
        [Required]
        [StringLength(2)]
        
        public string Title { get; set; }

        // 你的验证逻辑
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title.Length>3)
            {
                yield return new ValidationResult(
                    "不能太长"
                    ,new[] { nameof(Title) }  // 验证失败的属性
                );
            }
        }
    }
}