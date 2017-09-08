namespace Demo1.Backend.ViewModels
{
    public class ResponseViewModel
    {
        public bool Success { get; set; } = true;

        public bool Failure => !Success;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}