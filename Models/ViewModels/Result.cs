namespace ItsyBits.Models.ViewModels {

    /// <summary>
    /// The different states of the result
    /// </summary>
    public enum ResultStatus {
        Error, Warning, Neutral, Success
    }

    /// <summary>
    /// Result shown to the user after an action
    /// </summary>
    public class Result {

        /// <summary>
        /// The status of the result
        /// </summary>
        public ResultStatus Status { get; private set; }

        /// <summary>
        /// Title of the result
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The message of the result
        /// </summary>
        public string Message { get; private set; }

        public Result(string title, string message, ResultStatus status = ResultStatus.Neutral) {
            Title = title;
            Message = message;
            Status = status;
        }
    }
}