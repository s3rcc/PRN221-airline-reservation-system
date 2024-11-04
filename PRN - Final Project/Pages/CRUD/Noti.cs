namespace PRN___Final_Project.Pages.CRUD
{
    public class Noti
    {
        public static string Msg { get; set; } = string.Empty;
        public static bool IsSuccess { get; set; } 

        //public static void Add(string msg) => Msg += msg;

        public static string GetMsg()
        {
            var msg = Msg;
            Msg = string.Empty;

            return msg;
        }

        public static void SetSuccess(string msg)
        {
            Msg += msg;
            IsSuccess = true;
        }

        public static void SetFail(string msg)
        {
            Msg += msg;
            IsSuccess = false;
        }

        public static void SetByResult(string action, string model, string rs)
        {
            if (rs == null)
                Noti.SetSuccess($"{action} {model} success!");
            else
                Noti.SetFail(rs);
        }
    }
}
