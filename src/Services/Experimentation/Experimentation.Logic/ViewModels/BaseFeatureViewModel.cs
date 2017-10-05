using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Experimentation.Logic.ViewModels
{
    public class BaseFeatureViewModel
    {
        [Required(AllowEmptyStrings = false, 
            ErrorMessage = "A feature name cannot be an empty string and must be a valid meaningful unique name.")]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int FriendlyId { get; set; }

        public List<string> BucketList { get; set; }
    }
}