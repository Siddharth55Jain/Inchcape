namespace TodoApi
{
    public class AWSSecretManager
    {
        //GetSecret()
        //{
        //should use secret manager to get the connection string from AWS Secret Manager
        //}
        public static string ConnString { get; private set; } = "Data Source=todos.db";
    }
}
