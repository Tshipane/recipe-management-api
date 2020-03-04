namespace RecipeManagement.Api.Infrastructure.Models.Errors
{
    public class BadRequestError
    {
        public BadRequestError(string error)
        {
            Error = error;
        }

        public string Error { get; }
    }
}