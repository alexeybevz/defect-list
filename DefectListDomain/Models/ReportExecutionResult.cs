namespace DefectListDomain.Models
{
    public class ReportExecutionResult<TResult>
    {
        public TResult Result { get; }
        public bool IsSuccess { get; }
        public string Error { get; }

        public ReportExecutionResult(TResult result, bool isSuccess, string error)
        {
            Result = result;
            IsSuccess = isSuccess;
            Error = error;
        }
    }
}