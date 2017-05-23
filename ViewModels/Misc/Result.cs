namespace ItsyBits.ViewModels {

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">The title of the result</param>
        /// <param name="message">The message of the result</param>
        /// <param name="status">The status of the result</param>
        public Result(string title, string message, ResultStatus status = ResultStatus.Neutral) {
            Title = title;
            Message = message;
            Status = status;
        }
    }
}