using System.ComponentModel.DataAnnotations;

namespace Experimentation.Logic.ViewModels
{
    public class FeatureViewModel : BaseFeatureViewModel
    {
        [Required(AllowEmptyStrings = false,
            ErrorMessage = "A feature ID cannot be an empty string and must be a valid meaningful unique ID.")]
        public string Id { get; set; }
    }
}