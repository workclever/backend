namespace WorkCleverSolution.Controllers
{
    public class ServiceResult
    {
        public bool Succeed { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }
        
        public ServiceResult()
        {
            Message = "OK";
        }
        
        public ServiceResult BeFail()
        {
            Succeed = false;
            return this;
        }
        
        public ServiceResult BeSuccess()
        {
            Succeed = true;
            return this;
        }
        
        public ServiceResult SetMessage(string message)
        {
            Message = message;
            return this;
        }
    }
}