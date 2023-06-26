namespace webapi.Jwt
{
    public interface IJwtAuthentication
    {
        public string Authenticate(long cardNo);
    }
}
