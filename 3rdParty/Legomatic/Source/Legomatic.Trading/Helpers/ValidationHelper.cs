using System.ComponentModel.DataAnnotations;

namespace Legomatic.Trading
{
    public static class ValidationHelper
    {
        public static void ValidateObject<T>(T item)
        {
            var context = new ValidationContext(item, null, null);

            Validator.ValidateObject(item, context, true);
        }
    }
}
